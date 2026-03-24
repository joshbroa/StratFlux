using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class IPosNode : Node
    {
        private int _increaseBy;

        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public IPosNode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Action, NodeName.IPos) { }

        // The amount to increase the position is stored
        public override void MapData()
        {
#nullable disable
            int increaseBy = (int)_jsonData["increase_by"];
#nullable enable

            // This will make sure increaseBy is a positive integer
            _increaseBy = NormaliseValue(increaseBy, 1, increaseBy);
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
                // If true, then order is returned. This is the current position (passed in via numInputs) + what to increase the position by.
                return new Order((int)numInputs[0] + _increaseBy);
            }
            else
            {
                // Otherwise null is returned
                return null;
            }
        }
    }
}
