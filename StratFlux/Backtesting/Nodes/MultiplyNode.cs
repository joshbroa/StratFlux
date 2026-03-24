using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class MultiplyNode : Node
    {
        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public MultiplyNode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Operation, NodeName.Multiply) { }

        // Requests value of two inputs
        public override int[] GetRequiredValues()
        {
            return new int[] { _inputs[0].GetInputConnection(0).GetNodeId(), _inputs[1].GetInputConnection(0).GetNodeId() };
        }

        // Multiplies values of two inputs and returns result
        public override decimal EvaluateNum(decimal[] numInputs, bool[] boolInputs)
        {
            return numInputs[0] * numInputs[1];
        }
    }
}
