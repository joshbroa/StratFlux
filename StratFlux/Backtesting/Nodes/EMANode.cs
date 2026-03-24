using Alpaca.Markets;
using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class EMANode : Node
    {
        private int _lookbackPeriod;
        private bool _hasStarted;
        private decimal _currentValue;
        private decimal _weightedMultiplier;

        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public EMANode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Value, NodeName.EMA)
        {
            _hasStarted = false;
        }

        // Lookback period for simple moving average is stored
        public override void MapData()
        {
#nullable disable
            int lookbackPeriod = (int)_jsonData["lookback_period"];
#nullable enable

            // For exponential moving average, minimum amount of values is 3
            _lookbackPeriod = NormaliseValue(lookbackPeriod, 3, 50);

            // Weighted multiplier for EMA can be created via lookback period
            _weightedMultiplier = 2 / (_lookbackPeriod + 1);
        }

        // Returns if the value has started yet
        public override bool HasValueStarted()
        {
            return _hasStarted;
        }

        // Gets range of values which covers the lookback period
        public override int[] GetRequiredIndexesForStart(int currentIndex)
        {
            return new int[] { currentIndex };
        }

        // First value will just be the first given closing price because EMA is a continuous function
        public override void CreateFirstValue(IBar[] requiredData)
        {
            _currentValue = requiredData[0].Close;

            _hasStarted = true;
        }

        // Only current index is needed for exponential moving average due to being a continuous function
        public override int[] GetRequiredIndexes(int currentIndex)
        {
            return new int[] { currentIndex };
        }

        // Exponential moving average = new value * weighted multiplier + old average * (1 - weighted multiplier)
        // This works because 0 < weighted multiplier <= 1
        public override void UpdateValue(IBar[] requiredData)
        {
            _currentValue = requiredData[0].Close * _weightedMultiplier + _currentValue * (1 - _weightedMultiplier);
        }

        // To evaluate value of node, stored exponential moving average is returned
        public override decimal EvaluateNum(decimal[] numInputs, bool[] boolInputs)
        {
            return _currentValue;
        }
    }
}
