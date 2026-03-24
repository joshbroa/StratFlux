using Alpaca.Markets;
using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class MFINode : Node
    {
        private int _lookbackPeriod;
        private bool _hasStarted;
        private decimal _currentValue;
        private decimal _averagePositiveMoneyFlow;
        private decimal _averageNegativeMoneyFlow;

        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public MFINode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Value, NodeName.MFI)
        {
            _hasStarted = false;
        }

        // Lookback period needed for money flow index is stored
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

        // This will create the money flow index when the required data is available
        public override void CreateFirstValue(IBar[] requiredData)
        {
            // First the typical prices are calculated for each data point
            decimal[] typicalPrices = new decimal[_lookbackPeriod];

            for (int i = 0; i < typicalPrices.Length; i++)
            {
                typicalPrices[i] = (requiredData[i].High + requiredData[i].Low + requiredData[i].Close) / 3;
            }

            // Over the lookback period, positive and negative money flow will be tracked
            decimal positiveMoneyFlow = 0;
            decimal negativeMoneyFlow = 0;

            // This loops over one less than the amount of prices as there will be one loop for each difference of prices which is one less than the number of prices
            for (int i = 0; i < typicalPrices.Length - 1; i++)
            {
                // The difference in the current typical price weighted by its volume and the next weighted by its volume is calculated
                decimal difference = typicalPrices[i + 1] * requiredData[i + 1].Volume - typicalPrices[i] * requiredData[i].Volume;

                // A positive difference is positive money flow while a negative difference is negative money flow
                if (difference > 0)
                {
                    positiveMoneyFlow = positiveMoneyFlow + difference;
                }
                else
                {
                    // Negative money flow is the cummulative value of the losses over the lookback period multiplied by the volume at each of the ticks which is a positive value.
                    // The difference in this case will be negative so must be multiplied by -1
                    negativeMoneyFlow = negativeMoneyFlow + (difference * -1);
                }
            }

            // Average positive and negative money flows are calculated so that money flow ratio and money flow index can be calculated
            _averagePositiveMoneyFlow = positiveMoneyFlow / _lookbackPeriod;
            _averageNegativeMoneyFlow = negativeMoneyFlow / _lookbackPeriod;

            decimal moneyFlowRatio;

            // As dividing by the average negative money flow is part of the calculation for money flow ratio, it must be checked that its value is not 0 or less
            if (_averageNegativeMoneyFlow <= 0)
            {
                // Average negative money flow should at least be 0 so will be reset to 0 just in case it went a bit below
                _averageNegativeMoneyFlow = 0;

                // A large money flow ratio will indicate little to no sales
                moneyFlowRatio = 999;
            }
            else
            {
                // Otherwise, money flow ratio will be calculated normally
                moneyFlowRatio = _averagePositiveMoneyFlow / _averageNegativeMoneyFlow;
            }

            // Money flow index is calculated
            _currentValue = 100 - (100 / (1 + moneyFlowRatio));

            // Value is updated so the node knows that it has a value now
            _hasStarted = true;
        }

        // The oldest price difference in the lookback period will require the two oldest values in the lookback period while the new price difference requires the two most recent
        public override int[] GetRequiredIndexes(int currentIndex)
        {
            return new int[] { currentIndex, currentIndex - 1, currentIndex - _lookbackPeriod + 1, currentIndex - _lookbackPeriod };
        }

        // The new MFI can be calculated by updating the stored average positive and negative money flow and then re-calculating the money flow ratio and then money flow index
        public override void UpdateValue(IBar[] requiredData)
        {
            // First, the typical prices for each of the required data points is calculated
            decimal[] typicalPrices = new decimal[requiredData.Length];

            for (int i = 0; i < typicalPrices.Length; i++)
            {
                typicalPrices[i] = (requiredData[i].High + requiredData[i].Low + requiredData[i].Close) / 3;
            }

            // Old difference is the difference between the two oldest values multiplied by volume while the new difference is the difference between the two most recent
            // values multiplied by volume.
            decimal oldDifference = typicalPrices[2] * requiredData[2].Volume - typicalPrices[3] * requiredData[3].Volume;
            decimal newDifference = typicalPrices[0] * requiredData[0].Volume - typicalPrices[1] * requiredData[1].Volume;

            // Positive difference is positive money flow while negative difference is negative money flow. The old difference will be divided by lookback period so it is on
            // the same scale as the average gain and then subtracted so the old difference is removed entirely from the average positive or negative money flow depending on
            // if the difference is positive or negative.
            if (oldDifference > 0)
            {
                _averagePositiveMoneyFlow = _averagePositiveMoneyFlow - (oldDifference / _lookbackPeriod);
            }
            else
            {
                _averageNegativeMoneyFlow = _averageNegativeMoneyFlow - (oldDifference * -1 / _lookbackPeriod);
            }

            // A similar process happens for updating the average positive or negative money flow values for the new difference value. This time, after being normalised, the
            // difference will be added onto the appropriate average.
            if (newDifference > 0)
            {
                _averagePositiveMoneyFlow = _averagePositiveMoneyFlow + (newDifference / _lookbackPeriod);
            }
            else
            {
                _averageNegativeMoneyFlow = _averageNegativeMoneyFlow + ((newDifference * -1) / _lookbackPeriod);
            }

            // Then, the new money flow ratio can be calculated
            decimal newMoneyFlowRatio;

            // As dividing by the average negative money flow is part of the calculation for money flow ratio, it must be checked that its value is not 0 or less
            if (_averageNegativeMoneyFlow <= 0)
            {
                // Average negative money flow should at least be 0 so will be reset to 0 just in case it went a bit below
                _averageNegativeMoneyFlow = 0;

                // In the case that there is no loss, a large money flow ratio will indicate little to no sales
                newMoneyFlowRatio = 999;
            }
            else
            {
                // Otherwise, money flow ratio will be calculated normally
                newMoneyFlowRatio = _averagePositiveMoneyFlow / _averageNegativeMoneyFlow;
            }

            // New money flow index is calculated
            _currentValue = 100 - (100 / (1 + newMoneyFlowRatio));
        }

        // To evaluate value of node, stored value is returned
        public override decimal EvaluateNum(decimal[] numInputs, bool[] boolInputs)
        {
            return _currentValue;
        }
    }
}
