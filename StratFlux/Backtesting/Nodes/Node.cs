using Alpaca.Markets;
using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class Node
    {
        protected int _id;
        protected NodeInput[] _inputs;
        protected NodeOutput[] _outputs;
        protected JObject _jsonData;
        protected NodeType _nodeType;
        protected NodeName _nodeName;

        public Node(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData, NodeType nodeType, NodeName nodeName)
        {
            _id = id;
            _inputs = inputs;
            _outputs = outputs;
            _jsonData = jsonData;
            _nodeType = nodeType;
            _nodeName = nodeName;
        }

        public NodeType GetNodeType()
        {
            return _nodeType;
        }

        public NodeName GetNodeName()
        {
            return _nodeName;
        }

        // This function will map node data stored in JSON to private variables in each class' implementation
        public virtual void MapData() { }

        // This function will allow value nodes to tell the strategy if they have a value yet
        public virtual bool HasValueStarted() { return false; }

        // This allows the value nodes to communicate which values they need to start updating their value
        public virtual int[] GetRequiredIndexesForStart(int currentIndex) { return new int[] { currentIndex }; }

        // This allows the value nodes to communicate which values they will need for each tick
        public virtual int[] GetRequiredIndexes(int currentIndex) { return new int[] { currentIndex }; }

        // This allows value nodes to create their first value based on the required market data
        public virtual void CreateFirstValue(IBar[] requiredData) { }

        // This updates value nodes with the new market data they requested
        public virtual void UpdateValue(IBar[] requiredData) { }

        // This is a method which can be used to allow nodes to list the Id's of nodes that they need data from
        public virtual int[] GetRequiredValues() { return Array.Empty<int>(); }

        // This method will allow value or operation nodes to return their decimal value
        public virtual decimal EvaluateNum(decimal[] numInputs, bool[] boolInputs) { return 0; }

        // This method will allow condition or conditional operator nodes to return their boolean value
        public virtual bool EvaluateBool(decimal[] numInputs, bool[] boolInputs) { return false; }

        // This method will allow action nodes to return their orders
        public virtual Order? EvaluateAction(decimal[] numInputs, bool[] boolInputs) { return null; }

        // This is a protected function which can be used to normalise integers to be between one number and another
        protected int NormaliseValue(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }
    }
}
