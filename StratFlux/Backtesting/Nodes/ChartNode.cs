using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class ChartNode : Node
    {
        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public ChartNode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Chart, NodeName.Chart) { }


    }
}
