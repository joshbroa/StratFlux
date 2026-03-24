using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class CloseTradesNode : Node
    {
        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public CloseTradesNode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Action, NodeName.CloseTrades) { }

        // No data needs to be mapped

        // This function will return the values which are required. Only the boolean value which decides if the order goes through or not is required
        public override int[] GetRequiredValues()
        {
            return new int[] { _inputs[0].GetInputConnection(0).GetNodeId() };
        }

        // This will return either null or an order depending on if the deciding boolean value is true
        public override Order? EvaluateAction(decimal[] numInputs, bool[] boolInputs)
        {
            if (boolInputs[0])
            {
                // If true, then order is returned. An order with null as the order size is used to indicate a close all trades order.
                return new Order(null);
            }
            else
            {
                // Otherwise null is returned
                return null;
            }
        }
    }
}
