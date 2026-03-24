using Alpaca.Markets;
using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class SMANode : Node
    {
        private int _lookbackPeriod;
        private bool _hasStarted;
        private decimal _currentValue;

        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public SMANode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Value, NodeName.SMA)
        {
            _hasStarted = false;
        }

        // Lookback period for simple moving average is stored
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

        // Takes average of the given values and sets _hasStarted to true
        public override void CreateFirstValue(IBar[] requiredData)
        {
            decimal sum = 0;

            foreach (IBar bar in requiredData)
            {
                sum = sum + bar.Close;
            }

            _currentValue = sum / _lookbackPeriod;

            _hasStarted = true;
        }

        /*
        For a moving average, the value to be removed and value to be added need to be requested. This is because if the average
        is for indexes 5, 6 and 7, then the updated value will be the average over 6, 7 and 8. Therefore, the value of index 6
        can be subtracted from the value of index 8 and divided by the lookback period. Adding this onto the current value will
        be the updated average.
        */
        public override int[] GetRequiredIndexes(int currentIndex)
        {
            return new int[] { currentIndex, currentIndex - _lookbackPeriod };
        }

        // Updated Average = Current Average + ((new value - old value) / lookback period)
        public override void UpdateValue(IBar[] requiredData)
        {
            _currentValue = _currentValue + ((requiredData[0].Close - requiredData[1].Close) / _lookbackPeriod);
        }

        // To evaluate value of node, stored simple moving average is returned
        public override decimal EvaluateNum(decimal[] numInputs, bool[] boolInputs)
        {
            return _currentValue;
        }
    }
}
