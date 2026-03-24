using Alpaca.Markets;
using Humanizer.Localisation.TimeToClockNotation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using StratFlux.Data;
using StratFlux.Data.Migrations;
using StratFlux.Models;
using StratFlux.Services;
using System.Drawing.Text;
using System.Reflection.Metadata;

namespace StratFlux.Backtesting
{
    public class BacktestingEngine
    {
        // These variables will be defined in the constructor via the backtesting manager
        private ApplicationDbContext _dbContext;
        private UserManager<StratUser> _userManager;
        private AlpacaDataService _dataService;
        private StratUser _user;
        private Strategy? _strategy;
        private BacktestingSettings? _settings;

        public BacktestingEngine(ApplicationDbContext dbContext, UserManager<StratUser> userManager, AlpacaDataService dataService, string userId, string strategyId, string backtestingSettingsId)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _dataService = dataService;

            _user = _userManager.FindByIdAsync(userId).Result;

            if (_dbContext.Strategies != null)
            {
                _strategy = _dbContext.Strategies.SingleOrDefault(strategy => strategy.Id == strategyId);
            }
            
            if (_dbContext.BacktestingSettings != null)
            {
                _settings = _dbContext.BacktestingSettings.SingleOrDefault(settings => settings.Id == backtestingSettingsId);
            }
        }
        public async Task<(bool, string)> RunBacktest(string generalResultsName)
        {
            // If strategy or settings are null and don't exist, return error and tell user the error
            if (_strategy == null) { return (false, "The selected strategy could not be found. Are you sure it exists?"); }
            if (_settings == null) { return (false, "The selected backtesting settings could not be found. Are you sure it exists?"); }

            // If strategy or settings don't belong to user, tell user and don't continue
            if (!VerifyStrategyBelongsToUser()) { return (false, "The strategy you selected does not belong to you."); }
            if (!VerifySettingsBelongsToUser()) { return (false, "The backtesting settings you selected does not belong to you."); }

            // If the time frame start is not before the time frame end, then no data can be retrieved so it is invalid
            if (DateTime.Compare(_settings.TimeFrameStart, _settings.TimeFrameEnd) >= 0) { return (false, "Invalid time frame. Make sure the time frame starts before it ends."); }

            // A new strategy object is constructed using the json string converted into a JObject (from Newtonsoft)
            StrategyClass strategyObject = new StrategyClass(JObject.Parse(_strategy.StrategyJson));

            // If the strategy is not valid and cannot be put through the backtester, the user will be notified of this
            try
            {
                strategyObject.MapNodes();
            }
            catch (Exception exception)
            {
                return (false, $"The strategy is not valid and cannot be run. Are all the inputs and outputs connected? Here is the exact error message:\n\n{exception.Message}");
            }

            // Next, the data must be retrieved (OHLCV data from AlpacaMarkets API)
            IBar[]? data;
            try
            {
                data = await _dataService.GetData(_settings.StockToTrade, _settings.TimeFrameStart, _settings.TimeFrameEnd, _settings.TimeResolution);
            }
            catch (Exception exception)
            {
                return (false, $"There was an error retrieving the market data. Here is the exact error message:\n\n{exception.Message}");
            }

            // Null data is used to indicate an invalid time frame
            if (data == null) { return (false, "Could not retrieve the data from the specified times. Maybe try changing the time frame."); }

            // The value nodes in the strategy will then be started
            try
            {
                strategyObject.StartValueNodes(data);
            }
            catch (Exception exception)
            {
                return (false, $"The value nodes in the strategy could not get values due to an error. Here is the error message:\n\n{exception.Message}");
            }

            // Past this point, if the database context for general results doesn't exist, the results cannot be saved
            if (_dbContext.GeneralResults == null) { return (false, "The results could not be saved because an error with the database context occurred."); }

            // Now, the general results must be created so we can get an Id to refer to while creating charts and indicators
            GeneralResults newResult = new GeneralResults();

            // Linked to user
            newResult.User = _user;
            newResult.UserId = _user.Id;

            // Saved in database
            _dbContext.GeneralResults.Add(newResult);

            await _dbContext.SaveChangesAsync();

            // Id retrieved
            string generalResultsId = newResult.Id;

            // All values to be tracked for the result is stored here
            double capital = _settings.InitialCapital;
            double currentEquity = _settings.InitialCapital;
            double maxDrawdown = _settings.InitialCapital;
            double totalCommissionFees = 0;
            List<Trade> trades = new List<Trade>();
            List<Order> orders = new List<Order>();
            List<int> tradeSellList = new List<int>();
            List<TimeSpan> holdingPeriods = new List<TimeSpan>();
            List<double> equitiesOverTime = new List<double>();
            List<double> returns = new List<double>();
            int winningTrades = 0;
            int losingTrades = 0;

            // For each tick of data (starting from 50), the strategy will return a list of any new orders or position updates
            for (int tick = 50; tick < data.Length; tick++)
            {
                // First, all trades to be sold are sold at the open price
                for (int i = 0; i < tradeSellList.Count(); i++)
                {
                    int tradeIndex = tradeSellList[i];

                    // In order to be sold, the capital will be changed based on the value of the trade
                    capital = capital + trades[tradeIndex].Sell(data[tick].Open);

                    // Commission fee amount is calculated
                    double commissionFee = trades[tradeIndex].CalculateCommissionFee(data[tick].Open, _settings.CommissionFeeType, _settings.CommissionFee);

                    // Add to total for general results
                    totalCommissionFees = totalCommissionFees + commissionFee;

                    // Deduct fee from capital
                    capital = capital - commissionFee;

                    // Return/Loss is recorded from the calculate profit function
                    returns.Add((double)trades[tradeIndex].CalculateProfit(data[tick].Open));

                    // Holding period of trade is recorded so the average holding period can be calclulated at end
                    holdingPeriods.Add(trades[tradeIndex].GetHoldingPeriod(data[tick].TimeUtc));

                    // It is recorded whether or not the trade made profit or not
                    if (trades[tradeIndex].WinningTrade(data[tick].Open))
                    {
                        winningTrades++;
                    }
                    else
                    {
                        losingTrades++;
                    }

                    // The trade is removed from the trades list. It is ok to remove it via index because the tradeSellList stores indexes from highest to lowest
                    trades.RemoveAt(tradeIndex);

                    // As each index will be unique, it can be removed via Remove()
                    tradeSellList.Remove(tradeIndex);
                }
                
                // If the capital is below 0, then the backtesting stops because it is no longer possible to trade
                if (capital < 0)
                {
                    break;
                }

                // Then, all outstanding orders are filled
                foreach (Order order in orders)
                {
                    Trade trade = order.FillOrder(data[tick].Open, data[tick].TimeUtc);

                    // New capital = current capital - cost of filling order - commission fee
                    capital = capital - trade.GetInitialCost(_settings.CommissionFeeType, (decimal)_settings.CommissionFee);

                    // Calculate commission fee
                    double commissionFee = trade.CalculateCommissionFee(data[tick].Open, _settings.CommissionFeeType, _settings.CommissionFee);
                    capital = capital - commissionFee;

                    // Add to total commission fees for general results
                    totalCommissionFees = totalCommissionFees + commissionFee;

                    // Add trade to trades list
                    trades.Add(trade);
                }

                // If the capital is below 0, then the backtesting stops because it is no longer possible to trade
                if (capital < 0)
                {
                    break;
                }

                // The list is reset with each tick so that the same trade isn't attempted to be removed twice
                tradeSellList = new List<int>();

                // Each trade is checked whether or not they are ready to sell. If so, they get added to the sell list
                for (int i = 0; i < trades.Count(); i++)
                {
                    if (trades[i].ReadyToSell(data[tick].Close))
                    {
                        tradeSellList.Add(i);
                    }
                }

                // The strategy will run for the current tick and return any new orders. If there is an error, it is not possible to continue further so it is returned
                try
                {
                    orders = strategyObject.RunTick(data, tick);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return (false, $"The strategy could not run on the {tick}th tick. Here is the exact error message:\n\n{exception.Message}");
                }

                /*
                If any of the orders are positional, all the current positional trades must be added to the sell list.
                Additionally, if the order is to reset the position, then the order itself must be removed so it doesn't become a trade.
                Finally, any order to close all trades will cause every active trade to be added to the trade sell list.
                */
                bool isPositional = false;
                bool closeAllTrades = false;
                List<int> removeOrderIndexes = new List<int>();

                // Each order is checked
                for (int i = 0; i < orders.Count(); i++)
                {
                    TradeType orderTradeType = orders[i].GetTradeType();

                    // If the order is positional, isPositional is checked to true
                    if (orderTradeType == TradeType.LongPosition || orderTradeType == TradeType.ShortPosition || orderTradeType == TradeType.ResetPosition)
                    {
                        isPositional = true;
                    }

                    // Furthermore, if it is specifically reset position, it needs to be taken out of the order list as to not become a trade itself.
                    // Its index is then added to a list which records the indexes of orders to be removed. These indexes will be used to remove the orders from the order list.
                    if (orderTradeType == TradeType.ResetPosition)
                    {
                        removeOrderIndexes.Add(i);
                    }
                    else if (orderTradeType == TradeType.CloseAllActiveTrades)
                    {
                        // If the order is to close all active trades, then closeAllTrades is set to true. The order should not become a trade,
                        // so it too is added to the remove order index list
                        removeOrderIndexes.Add(i);
                        closeAllTrades = true;
                    }
                }

                // The list is sorted and reversed so when removing the elements by index, the remaining indexes will not change
                removeOrderIndexes.Sort();
                removeOrderIndexes.Reverse();

                // Each instructional order is removed by going through the reset order indexes
                foreach (int removeOrderIndex in removeOrderIndexes)
                {
                    orders.RemoveAt(removeOrderIndex);
                }

                // If an order has been given to close all trades, then every trade is added to the trade sell list
                if (closeAllTrades)
                {
                    for (int i = 0; i < trades.Count(); i++)
                    {
                        if (!tradeSellList.Contains(i))
                        {
                            tradeSellList.Add(i);
                        }
                    }
                }
                else if (isPositional)
                {
                    // If there is an order which is positional, in all cases, any positional trades must be closed
                    for (int i = 0; i < trades.Count(); i++)
                    {
                        TradeType tradeType = trades[i].GetTradeType();

                        // If the trade type is positional, it is sold
                        if (tradeType == TradeType.LongPosition || tradeType == TradeType.ShortPosition)
                        {
                            if (!tradeSellList.Contains(i))
                            {
                                tradeSellList.Add(i);
                            }
                        }
                    }
                }

                // The list is ordered and then reversed so that as each trade is removed, it will be highest index first. This means that no indexes need to be calculated
                tradeSellList.Sort();
                tradeSellList.Reverse();

                // Finally, the current equity is evaluated and if it is lower than it has been so far, it will become the new max drawdown
                currentEquity = capital;

                // Equity is evaluated by starting with the current capital and then adding on the value of each active trade
                foreach (Trade trade in trades)
                {
                    currentEquity = currentEquity + trade.GetValue(data[tick].Close);
                }

                // Current equity is added onto list of equities over time
                equitiesOverTime.Add(currentEquity);

                // If current equity is less then max drawdown, it becomes the new max drawdown
                if (currentEquity < maxDrawdown)
                {
                    maxDrawdown = currentEquity;
                }
            }

            // Each of the general results fields are filled in from the data available after running the back test
            newResult.ResultsName = generalResultsName;

            newResult.InitialEquity = _settings.InitialCapital;

            newResult.FinalEquity = equitiesOverTime[equitiesOverTime.Count() - 1];

            newResult.TotalClosedTrades = winningTrades + losingTrades;

            newResult.WinningTrades = winningTrades;

            newResult.LosingTrades = losingTrades;

            newResult.StockTraded = _settings.StockToTrade;

            newResult.TotalCommissionAmount = totalCommissionFees;

            newResult.TimeFrameStart = _settings.TimeFrameStart;

            newResult.TimeFrameEnd = _settings.TimeFrameEnd;

            newResult.TimeResolution = _settings.TimeResolution;

            newResult.UnrealisedReturnLoss = equitiesOverTime[equitiesOverTime.Count() - 1] - _settings.InitialCapital;

            newResult.NetReturnLoss = capital - _settings.InitialCapital;

            newResult.AverageReturnLoss = CalculateAverageReturnLoss(returns);

            newResult.AverageHoldingPeriod = CalculateAverageHoldingPeriod(holdingPeriods);

            newResult.StandardDeviationOverTime = CalculateStandardDeviation(equitiesOverTime);

            newResult.MaxDrawDown = maxDrawdown;

            // The changes made to newResult are saved before the general results Id is sent back to the user
            await _dbContext.SaveChangesAsync();

            return (true, generalResultsId);
        }

        // This function makes sure that the strategy does actually belong to the user
        private bool VerifyStrategyBelongsToUser()
        {
            if (_strategy != null)
            {
                if (_strategy.UserId == _user.Id)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        // This function makes sure the settings do actually belong to the user
        private bool VerifySettingsBelongsToUser()
        {
            if (_settings != null)
            {
                if (_settings.UserId == _user.Id)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        // This function calculates the average holding period from a list of holding periods
        private TimeSpan CalculateAverageHoldingPeriod(List<TimeSpan> holdingPeriods)
        {
            // Each time period is summed to one total
            TimeSpan holdingPeriodTotal = TimeSpan.Zero;

            // If the are no holding periods, there is no average to calculate so a timespan of zero is returned
            if (holdingPeriods.Count() <= 0) { return TimeSpan.Zero; }

            foreach (TimeSpan holdingPeriod in holdingPeriods)
            {
                holdingPeriodTotal = holdingPeriodTotal + holdingPeriod;
            }

            // This is then averaged out by dividing the total number of ticks by the number of holding periods and converting from ticks back into the TimeSpan format
            return TimeSpan.FromTicks(holdingPeriodTotal.Ticks / holdingPeriods.Count());
        }

        // This function will work out the standard deviation over time of all the equities
        private double CalculateStandardDeviation(List<double> equitiesOverTime)
        {
            // If there is no data, standard deviation cannot be calculated
            if (equitiesOverTime.Count() == 0) { return 0; }

            // Number of equities is the number of items in the list and both the sum and square sum start from 0
            double numberOfEquities = equitiesOverTime.Count();
            double sum = 0;
            double squareSum = 0;

            // The sum is calclulated by adding each equity and the square sum is calculated by adding the square of each equity
            foreach (double equity in equitiesOverTime)
            {
                sum = sum + equity;

                squareSum = squareSum + (equity * equity);
            }

            // Average is calclulated by the sum divided by the number of items
            double averageEquity = sum / numberOfEquities;

            // Variation is then calculated which is just standard deviation squared so square rooting this value gives the standard deviation
            double variance = (squareSum - (numberOfEquities * averageEquity * averageEquity)) / numberOfEquities;

            double standardDeviation = Math.Sqrt(variance);

            return standardDeviation;
        }

        // Calculates the average return/loss over the trading period
        private double CalculateAverageReturnLoss(List<double> returns)
        {
            double sum = 0;

            // If there are no returns, there is no average to calculate so 0 should be returned
            if (returns.Count() <= 0) { return 0; }

            foreach (double profitLoss in returns)
            {
                sum = sum + profitLoss;
            }

            return sum / returns.Count();
        }
    }
}
