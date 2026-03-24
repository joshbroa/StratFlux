using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class GreaterEqualNode : Node
    {
        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public GreaterEqualNode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Condition, NodeName.GreaterEqual) { }

        // Requests the required values
        public override int[] GetRequiredValues()
        {
            return new int[] { _inputs[0].GetInputConnection(0).GetNodeId(), _inputs[1].GetInputConnection(0).GetNodeId() };
        }

        // Returns if the first value is greater than or equal to the second value
        public override bool EvaluateBool(decimal[] numInputs, bool[] boolInputs)
        {
            return numInputs[0] >= numInputs[1];
        }
    }
}
