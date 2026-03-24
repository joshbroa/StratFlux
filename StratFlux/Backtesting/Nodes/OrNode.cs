using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class OrNode : Node
    {
        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public OrNode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.ConditionalOperator, NodeName.Or) { }

        // Requests the required values
        public override int[] GetRequiredValues()
        {
            return new int[] { _inputs[0].GetInputConnection(0).GetNodeId(), _inputs[1].GetInputConnection(0).GetNodeId() };
        }

        // Returns true if either one of the boolean values is true
        public override bool EvaluateBool(decimal[] numInputs, bool[] boolInputs)
        {
            return boolInputs[0] || boolInputs[1];
        }
    }
}
