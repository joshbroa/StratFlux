using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class SPosNode : Node
    {
        private int _position;

        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public SPosNode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Action, NodeName.SPos) { }

        // The amount to decrease the position is stored
        public override void MapData()
        {
#nullable disable
            int position = (int)_jsonData["position"];
#nullable enable

            _position = position;
        }

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
                // If true, then order is returned
                return new Order(_position);
            }
            else
            {
                // Otherwise null is returned
                return null;
            }
        }
    }
}
