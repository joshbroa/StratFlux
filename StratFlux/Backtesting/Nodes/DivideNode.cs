using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class DivideNode : Node
    {
        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public DivideNode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Operation, NodeName.Divide) { }

        // Requests value of two inputs
        public override int[] GetRequiredValues()
        {
            return new int[] { _inputs[0].GetInputConnection(0).GetNodeId(), _inputs[1].GetInputConnection(0).GetNodeId() };
        }

        // Divides first value by second value
        public override decimal EvaluateNum(decimal[] numInputs, bool[] boolInputs)
        {
            // If second value is 0, division cannot take place so 0 will be returned instead
            if (numInputs[1] == 0)
            {
                return 0;
            }
            else
            {
                return numInputs[0] / numInputs[1];
            }
        }
    }
}
