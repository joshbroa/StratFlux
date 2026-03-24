using Alpaca.Markets;
using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class ConstNode : Node
    {
        private decimal _const;

        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public ConstNode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Value, NodeName.Const) { }

        // The constant value stored in the JSON is mapped to _const
        public override void MapData()
        {
#nullable disable
            _const = (decimal)_jsonData["value"];
#nullable enable
        }

        public override bool HasValueStarted()
        {
            return true;
        }

        public override int[] GetRequiredIndexes(int currentIndex)
        {
            return Array.Empty<int>();
        }

        public override decimal EvaluateNum(decimal[] numInputs, bool[] boolInputs)
        {
            return _const;
        }
    }
}
