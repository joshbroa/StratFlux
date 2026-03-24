using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class EqualNode : Node
    {
        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public EqualNode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Condition, NodeName.Equal) { }

        // Requests the required values
        public override int[] GetRequiredValues()
        {
            return new int[] { _inputs[0].GetInputConnection(0).GetNodeId(), _inputs[1].GetInputConnection(0).GetNodeId() };
        }

        // Returns if the two values are equal to each other or not
        public override bool EvaluateBool(decimal[] numInputs, bool[] boolInputs)
        {
            return numInputs[0] == numInputs[1];
        }
    }
}
