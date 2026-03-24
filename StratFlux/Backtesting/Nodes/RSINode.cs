using Alpaca.Markets;
using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class RSINode : Node
    {
        private int _lookbackPeriod;
        private bool _hasStarted;
        private decimal _currentValue;
        private decimal _averageGain;
        private decimal _averageLoss;

        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public RSINode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Value, NodeName.RSI)
        {
            _hasStarted = false;
        }

        // Lookback period needed for relative strength index is stored
        public override void MapData()
        {
#nullable disable
            int lookbackPeriod = (int)_jsonData["lookback_period"];
#nullable enable

            _lookbackPeriod = NormaliseValue(lookbackPeriod, 1, 50);
        }

        // Returns if the value has started yet
        public override bool HasValueStarted()
        {
            return _hasStarted;
        }

        // Gets range of values which covers the lookback period
        public override int[] GetRequiredIndexesForStart(int currentIndex)
        {
            int[] requiredIndexes = new int[_lookbackPeriod];

            // If current index is 7 and lookback period is 3, indexes 5, 6 and 7 must be requested
            for (int i = 0; i < requiredIndexes.Length; i++)
            {
                requiredIndexes[i] = currentIndex - requiredIndexes.Length + 1 + i;
            }

            return requiredIndexes;
        }

        // This will create the relative strength index when the required data is available
        public override void CreateFirstValue(IBar[] requiredData)
        {
            // First the typical prices are calculated for each data point
            decimal[] typicalPrices = new decimal[_lookbackPeriod];

            for (int i = 0; i < typicalPrices.Length; i++)
            {
                typicalPrices[i] = (requiredData[i].High + requiredData[i].Low + requiredData[i].Close) / 3;
            }

            // Over the lookback period, gain and loss will be tracked
            decimal gain = 0;
            decimal loss = 0;

            // This loops over one less than the amount of prices as there will be one loop for each difference of prices which is one less than the number of prices
            for (int i = 0; i < typicalPrices.Length - 1; i++)
            {
                // The difference in the current typical price and the next is calculated
                decimal difference = typicalPrices[i + 1] - typicalPrices[i];
                
                // A positive difference is a gain while a negative difference is a loss
                if (difference > 0)
                {
                    gain = gain + difference;
                }
                else
                {
                    // Loss is the cummulative value of the losses over the lookback period which is a positive value. The difference in this case will be negative
                    // so must be multiplied by -1
                    loss = loss + (difference * -1);
                }
            }

            // Average gain and loss are calculated so that relative strength and relative strength index can be calculated
            _averageGain = gain / _lookbackPeriod;
            _averageLoss = loss / _lookbackPeriod;

            decimal relativeStrength;

            // As dividing by the average loss is part of the calculation for relative strength, it must be checked that its value is not 0 or less
            if (_averageLoss <= 0)
            {
                // Average loss should at least be 0 so will be reset to 0 just in case it went a bit below
                _averageLoss = 0;

                // A large relative strength will indicate little to no sales
                relativeStrength = 999;
            }
            else
            {
                // Otherwise, relative strength will be calculated normally
                relativeStrength = _averageGain / _averageLoss;
            }

            // Relative strength index is calculated
            _currentValue = 100 - (100 / (1 + relativeStrength));

            // Value is updated so the node knows that it has a value now
            _hasStarted = true;
        }

        // The oldest price difference in the lookback period will require the two oldest values in the lookback period while the new price difference requires the two most recent
        public override int[] GetRequiredIndexes(int currentIndex)
        {
            return new int[] { currentIndex, currentIndex - 1, currentIndex - _lookbackPeriod + 1, currentIndex - _lookbackPeriod };
        }

        // The new RSI can be calculated by updating the stored average gain and loss and then re-calculating the relative strength and then relative strength index
        public override void UpdateValue(IBar[] requiredData)
        {
            // First, the typical prices for each of the required data points is calculated
            decimal[] typicalPrices = new decimal[requiredData.Length];

            for (int i = 0; i < typicalPrices.Length; i++)
            {
                typicalPrices[i] = (requiredData[i].High + requiredData[i].Low + requiredData[i].Close) / 3;
            }

            // Old difference is the difference between the two oldest values while the new difference is the difference between the two most recent values.
            decimal oldDifference = typicalPrices[2] - typicalPrices[3];
            decimal newDifference = typicalPrices[0] - typicalPrices[1];

            // A positive difference is a gain while a negative difference is a loss. The old difference will be divided by lookback period so it is on the same scale as the
            // average gain and then subtracted so the old difference is removed entirely from the average gain or loss depending on if the difference is positive or negative.
            if (oldDifference > 0)
            {
                _averageGain = _averageGain - (oldDifference / _lookbackPeriod);
            }
            else
            {
                _averageLoss = _averageLoss - (oldDifference * -1 / _lookbackPeriod);
            }
            
            // A similar process happens for updating the average gain or average loss values for the new difference value. This time, after being normalised, the difference
            // will be added onto the appropriate average.
            if (newDifference > 0)
            {
                _averageGain = _averageGain + (newDifference / _lookbackPeriod);
            }
            else
            {
                _averageLoss = _averageLoss + ((newDifference * -1) / _lookbackPeriod);
            }

            // Then, the new relative strength can be calculated
            decimal newRelativeStrength;

            // As dividing by the average loss is part of the calculation for relative strength, it must be checked that its value is not 0 or less
            if (_averageLoss <= 0)
            {
                // Average loss should at least be 0 so will be reset to 0 just in case it went a bit below
                _averageLoss = 0;

                // In the case that there is no loss, a large relative strength will indicate little to no sales
                newRelativeStrength = 999;
            }
            else
            {
                // Otherwise, relative strength will be calculated normally
                newRelativeStrength = _averageGain / _averageLoss;
            }

            // New relative strength index is calculated
            _currentValue = 100 - (100 / (1 + newRelativeStrength));
        }

        // To evaluate value of node, stored value is returned
        public override decimal EvaluateNum(decimal[] numInputs, bool[] boolInputs)
        {
            return _currentValue;
        }
    }
}
