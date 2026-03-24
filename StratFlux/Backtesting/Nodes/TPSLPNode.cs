using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class TPSLPNode : Node
    {
        private int _shareAmount;

        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public TPSLPNode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Action, NodeName.TPSLP) { }

        // Share amount of node is stored
        public override void MapData()
        {
#nullable disable
            int shareAmount = (int)_jsonData["share_amount"];
#nullable enable

            _shareAmount = shareAmount;
        }

        // This function will return the values which are required.
        // Three values are needed: boolean which decides if trade happens, take profit percentage, and stop loss percentage
        public override int[] GetRequiredValues()
        {
            return new int[] { _inputs[0].GetInputConnection(0).GetNodeId(), _inputs[1].GetInputConnection(0).GetNodeId(), _inputs[2].GetInputConnection(0).GetNodeId() };
        }

        // This will return either null or an order depending on if the deciding boolean value is true
        public override Order? EvaluateAction(decimal[] numInputs, bool[] boolInputs)
        {
            if (boolInputs[0])
            {
                // If true, then order is returned
                return new Order(_shareAmount, numInputs[0], numInputs[1], true);
            }
            else
            {
                // Otherwise null is returned
                return null;
            }
        }
    }
}
