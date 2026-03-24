using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StratFlux.Backtesting
{
    public class Order
    {
        private int _orderSize;
        private TradeType _tradeType;
        private decimal? _takeProfit;
        private decimal? _stopLoss;

        public Order(int? orderSize)
        {
            // A null order size is used to indicate an order to close all trades
            if (orderSize == null)
            {
                _tradeType = TradeType.CloseAllActiveTrades;
                _orderSize = 0;
            }
            else
            {
                // Otherwise, this will be a positional order depending on the order size
                _orderSize = (int)orderSize;

                if (orderSize == 0)
                {
                    _tradeType = TradeType.ResetPosition;
                }
                else if (orderSize > 0)
                {
                    _tradeType = TradeType.LongPosition;
                }
                else
                {
                    // Order size is multiplied by -1 so that the result is positive
                    _orderSize = (int)orderSize * -1;
                    _tradeType = TradeType.ShortPosition;
                }
            }
        }

        // There are two different constructors for an order - one for a positional change and one for a take profit/stop loss order
        // This is the take profit/stop loss order constructor
        public Order(int orderSize, decimal takeProfit, decimal stopLoss, bool isPercentage)
        {
            _orderSize = orderSize;

            // Both the take profit and stop loss must be positive, so this makes sure their value is 0 or above
            if (takeProfit < 0)
            {
                _takeProfit = takeProfit * -1;
            }
            else
            {
                _takeProfit = takeProfit;
            }

            if (stopLoss < 0)
            {
                _stopLoss = stopLoss * -1;
            }

            // A negative order size indicates a short position while a positive order size indicates a long position
            if (orderSize > 0)
            {
                // isPercentage is a boolean value which indicates whether or not the type of take profit/stop loss order is absolute or percentage based.
                // Based on the boolean, the appropriate trade type is selected
                if (isPercentage)
                {
                    _tradeType = TradeType.LongTPSLP;
                }
                else
                {
                    _tradeType = TradeType.LongTPSLA;
                }
            }
            else
            {
                // The same check happens here as is shown above
                if (isPercentage)
                {
                    _tradeType = TradeType.ShortTPSLP;
                }
                else
                {
                    _tradeType = TradeType.ShortTPSLA;
                }
            }
        }

        // This function returns a trade based on the open price, current time, and other data contained in the order object
        public Trade FillOrder(decimal openPrice, DateTime currentTime)
        {
            // As the trade type is already set, the order size no longer has to be negative and it shouldn't be in order for the calculations in the trade object to be correct
            int orderSize = _orderSize;

            if (orderSize < 0)
            {
                orderSize = orderSize * -1;
            }

            // If the trade is positional, not much data needs to be passed in except the open price, order size, current time and trade type
            if (_tradeType == TradeType.LongPosition || _tradeType == TradeType.ShortPosition)
            {
                return new Trade(openPrice, orderSize, currentTime, _tradeType);
            }
            else
            {
                // If the order is not positional, then it is a take profit/stop loss trade. Therefore, the take profit and stop loss are set to default values of 20 and 10
                // just in case the stored values are null. If they aren't null, their values are retrieved and used to return a newly created Trade object.
                decimal takeProfit = 20;
                decimal stopLoss = 10;

                if (_takeProfit != null && _stopLoss != null)
                {
                    takeProfit = (decimal)_takeProfit;
                    stopLoss = (decimal)_stopLoss;
                }

                return new Trade(openPrice, takeProfit, stopLoss, orderSize, currentTime, _tradeType);
            }
        }

        // Returns the trade type
        public TradeType GetTradeType()
        {
            return _tradeType;
        }

        // This function will return the position based on what the order is trying to do
        public int? GetPosition()
        {
            // If the trade is a long positional trade, the order size is the position
            if (_tradeType == TradeType.LongPosition)
            {
                return _orderSize;
            }
            else if (_tradeType == TradeType.ShortPosition)
            {
                // If it is a short positional trade, then the position is negative
                return _orderSize * -1;
            }
            else if (_tradeType == TradeType.ResetPosition)
            {
                // If it is a reset position order, then the position is 0
                return 0;
            }
            else
            {
                // Otherwise, the order is not positional so null should be returned
                return null;
            }
        }
    }
}
