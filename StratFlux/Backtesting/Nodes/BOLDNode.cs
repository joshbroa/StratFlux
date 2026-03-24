using Alpaca.Markets;
using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class BOLDNode : Node
    {
        private int _lookbackPeriod;
        private bool _hasStarted;
        private decimal _currentValue;
        private decimal _standardDeviations;
        private decimal _squareSum;
        private decimal _average;

        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public BOLDNode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Value, NodeName.BOLD)
        {
            _hasStarted = false;
        }

        // Lookback period and number of standard deviations for bollinger band is stored
        public override void MapData()
        {
#nullable disable
            int lookbackPeriod = (int)_jsonData["lookback_period"];

            decimal standardDeviations = (decimal)_jsonData["standard_deviations"];
#nullable enable

            _lookbackPeriod = NormaliseValue(lookbackPeriod, 1, 50);

            _standardDeviations = standardDeviations;
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

        // This follows the standard deviation equation for the standard deviation and then uses that value to get the lower bollinger band based on user's parameters
        public override void CreateFirstValue(IBar[] requiredData)
        {
            decimal sum = 0;

            // First, sum of typical price and sum of the squares of typical prices are taken where the typical price is the average of the high, low and close values
            foreach (IBar bar in requiredData)
            {
                decimal typicalPrice = (bar.High + bar.Low + bar.Close) / 3;

                sum = sum + typicalPrice;
                _squareSum = _squareSum + typicalPrice * typicalPrice;
            }

            // Average is calculated and used along with the square sum to calculate standard deviation of values
            _average = sum / _lookbackPeriod;

            decimal variance = (_squareSum - _lookbackPeriod * (_average * _average)) / _lookbackPeriod;

            decimal standardDeviation = (decimal)Math.Sqrt((double)variance);

            // Using standard deviation, lower bollinger band can be created
            _currentValue = _average - standardDeviation * _standardDeviations;

            // Value is updated so the node knows that it has a value now
            _hasStarted = true;
        }

        // The index that will be outside of the lookback period and the current index will be required
        public override int[] GetRequiredIndexes(int currentIndex)
        {
            return new int[] { currentIndex, currentIndex - _lookbackPeriod };
        }

        // Here, average and square sum values are updated and the new values are used to calculate the new standard deviation and therefore, the new bollinger band value
        public override void UpdateValue(IBar[] requiredData)
        {
            // Typical prices are calculated
            decimal newTypicalPrice = (requiredData[0].High + requiredData[0].Low + requiredData[0].Close) / 3;
            decimal oldTypicalPrice = (requiredData[1].High + requiredData[1].Low + requiredData[1].Close) / 3;

            // New average and square sum values are calclulated
            _average = _average - (oldTypicalPrice / _lookbackPeriod) + (newTypicalPrice / _lookbackPeriod);

            _squareSum = _squareSum - (oldTypicalPrice * oldTypicalPrice) + (newTypicalPrice * newTypicalPrice);

            // The updated values are used to calcluate standrad deviation
            decimal variance = (_squareSum - _lookbackPeriod * (_average * _average)) / _lookbackPeriod;

            decimal standardDeviation = (decimal)Math.Sqrt((double)variance);

            // Bollinger band value is updated
            _currentValue = _average - standardDeviation * _standardDeviations;
        }

        // To evaluate value of node, stored value is returned
        public override decimal EvaluateNum(decimal[] numInputs, bool[] boolInputs)
        {
            return _currentValue;
        }
    }
}
