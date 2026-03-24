using Alpaca.Markets;
using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class OpenNode : Node
    {
        private int _tickOffset;
        private bool _hasStarted;
        private decimal _currentValue;

        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public OpenNode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Value, NodeName.Open)
        {
            _hasStarted = false;
        }

        // Tick offset is stored in JSON data of node
        public override void MapData()
        {
#nullable disable
            int rawTickOffset = (int)_jsonData["tick_offset"];
#nullable enable

            _tickOffset = NormaliseValue(rawTickOffset, 0, 49);
        }

        // Returns if the value has started yet
        public override bool HasValueStarted()
        {
            return _hasStarted;
        }

        // Returns the required tick which is just the current tick - tick offset
        public override int[] GetRequiredIndexesForStart(int currentIndex)
        {
            return new int[] { currentIndex - _tickOffset };
        }

        // Current value is set and has started bool is set to true
        public override void CreateFirstValue(IBar[] requiredData)
        {
            _currentValue = requiredData[0].Open;
            _hasStarted = true;
        }

        // Returns required tick which is just the current tick - tick offset
        public override int[] GetRequiredIndexes(int currentIndex)
        {
            return new int[] { currentIndex - _tickOffset };
        }

        // Open value is updated to open value of new tick
        public override void UpdateValue(IBar[] requiredData)
        {
            _currentValue = requiredData[0].Open;
        }
        
        // To evalutate value of node, stored open value is returned
        public override decimal EvaluateNum(decimal[] numInputs, bool[] boolInputs)
        {
            return _currentValue;
        }
    }
}
