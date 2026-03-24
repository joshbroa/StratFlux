using Alpaca.Markets;
using NuGet.Packaging.Signing;
using StratFlux.ModelEnums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StratFlux.Backtesting
{
    public class Trade
    {
        private decimal _initialPrice;
        private int _orderSize;
        private DateTime _initialTimeStamp;
        private TradeType _tradeType;
        private decimal? _takeProfit;
        private decimal? _stopLoss;

        public Trade(decimal price, int orderSize, DateTime currentTime, TradeType tradeType)
        {
            _initialPrice = price;
            _orderSize = orderSize;
            _initialTimeStamp = currentTime;
            _tradeType = tradeType;
        }

        public Trade(decimal price, decimal takeProfit, decimal stopLoss, int orderSize, DateTime currentTime, TradeType tradeType)
        {
            _initialPrice = price;
            _orderSize = orderSize;
            _initialTimeStamp = currentTime;
            _tradeType = tradeType;
            _takeProfit = takeProfit;
            _stopLoss = stopLoss;
        }

        // This function returns how much capital the trade costs when it goes through
        public double GetInitialCost(CommissionFeeType commissionFeeType, decimal commissionFeeAmount)
        {
            decimal cost;

            // If the trade has a short position, the capital will increase the moment the trade goes through so the result is multiplied by -1;
            if (_tradeType == TradeType.ShortPosition || _tradeType == TradeType.ShortTPSLA || _tradeType == TradeType.ShortTPSLP)
            {
                cost = -1 * _initialPrice * _orderSize;
            }
            else
            {
                cost = _initialPrice * _orderSize;
            }

            // The commission fee amount must be added on
            if (commissionFeeType == CommissionFeeType.Absolute)
            {
                return (double)(cost + commissionFeeAmount);
            }
            else
            {
                return (double)(cost * (1 + (commissionFeeAmount / 100)));
            }
        }

        // This function returns whether or not the trade is ready to sell depending on the current state of the trade
        public bool ReadyToSell(decimal closePrice)
        {
            // If the trade is a positional trade, logic outside of this class decides whether or not it should be sold so false is returned here
            if (_tradeType == TradeType.LongPosition || _tradeType == TradeType.ShortPosition)
            {
                return false;
            }
            else
            {
                // The current profit is compared to the profit and loss thresholds for being sold. Depending on the comparison, either true or false will be returned
                decimal profit = CalculateProfit(closePrice);
                decimal profitThreshold = GetProfitThreshold();
                decimal lossThreshold = GetLossThreshold();

                if (profit >= profitThreshold)
                {
                    return true;
                }
                else if (profit <= lossThreshold)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // This function returns the amount of time the trade was active with the DateTime input parameter being the datetime to use as the time the trade stopped being active
        public TimeSpan GetHoldingPeriod(DateTime endTime)
        {
            return endTime - _initialTimeStamp;
        }

        // This function will return the value of the trade at the time it sold
        public double Sell(decimal openPrice)
        {
            // If the trade has a short position, its value must be negative because the trader will be paying back what they owe
            if (_tradeType == TradeType.ShortPosition || _tradeType == TradeType.ShortTPSLA || _tradeType == TradeType.ShortTPSLP)
            {
                return (double)(-1 * _orderSize * openPrice);
            }
            else
            {
                return (double)(_orderSize * openPrice);
            }
        }

        public double CalculateCommissionFee(decimal price, CommissionFeeType commissionFeeType, double commissionFeeAmount)
        {
            // If the commission fee type is absolute, then the commission fee will just be the given commission fee amount
            if (commissionFeeType == CommissionFeeType.Absolute)
            {
                return (double)commissionFeeAmount;
            }
            else
            {
                // In order to calculate the percentage commission fee, only the magnitude of the value of the trade is needed
                decimal currentValue = _orderSize * price;

                // Then, the percentage is applied to the value
                return (double)currentValue * (commissionFeeAmount / 100);
            }
            
        }

        // This will return the value of the trade which can be used to work out equity
        public double GetValue(decimal currentPrice)
        {
            // If the position is short, the value will be negative as capital is owed
            if (_tradeType == TradeType.ShortPosition || _tradeType == TradeType.ShortTPSLA || _tradeType == TradeType.ShortTPSLP)
            {
                return (double)(-1 * _orderSize * currentPrice);
            }
            else
            {
                // Otherwise, the value is the current price multiplied by the order size
                return (double)(_orderSize * currentPrice);
            }
        }

        // This function returns whether or not this trade is a winning trade or losing trade
        public bool WinningTrade(decimal currentPrice)
        {
            if (CalculateProfit(currentPrice) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // This function returns the trade type of the trade
        public TradeType GetTradeType()
        {
            return _tradeType;
        }

        // This function can be used to calculate the profit if the trade were to be sold at a specified price
        public decimal CalculateProfit(decimal currentPrice)
        {
            // For any short trades, the profit will be the current sell value subtracted from the initial cost whereas it is normally initial cost subtracted from sell value
            if (_tradeType == TradeType.ShortPosition || _tradeType == TradeType.ShortTPSLA || _tradeType == TradeType.ShortTPSLP)
            {
                return _orderSize * (_initialPrice - currentPrice);
            }
            else
            {
                return _orderSize * (currentPrice - _initialPrice);
            }
        }

        private decimal GetProfitThreshold()
        {
            if (_takeProfit == null) { return 0; }

            if (_tradeType == TradeType.LongTPSLA || _tradeType == TradeType.ShortTPSLA)
            {
                return _initialPrice * _orderSize + (decimal)_takeProfit;
            }
            else
            {
                return (1 + ((decimal)_takeProfit / 100)) * _initialPrice * _orderSize;
            }
        }

        private decimal GetLossThreshold()
        {
            if (_stopLoss == null) { return 0; }

            if (_tradeType == TradeType.LongTPSLA || _tradeType == TradeType.ShortTPSLA)
            {
                return _initialPrice * _orderSize - (decimal)_stopLoss;
            }
            else
            {
                return (1 - ((decimal)_stopLoss / 100)) * _initialPrice * _orderSize;
            }
        }
    }
}
