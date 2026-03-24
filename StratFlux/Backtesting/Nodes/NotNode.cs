using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class NotNode : Node
    {
        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public NotNode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.ConditionalOperator, NodeName.Not) { }

        // Requests the required value
        public override int[] GetRequiredValues()
        {
            return new int[] { _inputs[0].GetInputConnection(0).GetNodeId() };
        }

        // Returns the opposite boolean value of the input
        public override bool EvaluateBool(decimal[] numInputs, bool[] boolInputs)
        {
            return !boolInputs[0];
        }
    }
}
