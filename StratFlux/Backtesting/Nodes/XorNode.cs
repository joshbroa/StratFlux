using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class XorNode : Node
    {
        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public XorNode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.ConditionalOperator, NodeName.Xor) { }

        // Requests the required values
        public override int[] GetRequiredValues()
        {
            return new int[] { _inputs[0].GetInputConnection(0).GetNodeId(), _inputs[1].GetInputConnection(0).GetNodeId() };
        }

        // Returns true if only one of the boolean values is true otherwise false is returned
        public override bool EvaluateBool(decimal[] numInputs, bool[] boolInputs)
        {
            return boolInputs[0] ^ boolInputs[1];
        }
    }
}
