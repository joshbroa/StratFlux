using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class DPosNode : Node
    {
        private int _decreaseBy;

        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public DPosNode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Action, NodeName.DPos) { }

        // The amount to decrease the position is stored
        public override void MapData()
        {
#nullable disable
            int decreaseBy = (int)_jsonData["decrease_by"];
#nullable enable

            // This will make sure decreaseBy is a positive integer
            _decreaseBy = NormaliseValue(decreaseBy, 1, decreaseBy);
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
                // If true, then order is returned. This is just the current position (passed in via numInputs) - the amount to decrease the position by
                return new Order((int)numInputs[0] - _decreaseBy);
            }
            else
            {
                // Otherwise null is returned
                return null;
            }
        }
    }
}
