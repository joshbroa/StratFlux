using Alpaca.Markets;
using Humanizer.Localisation.TimeToClockNotation;
using MessagePack;
using Newtonsoft.Json.Linq;
using StratFlux.Backtesting.Nodes;
using StratFlux.Services;
using System.Text.Json.Nodes;

namespace StratFlux.Backtesting
{
    public class StrategyClass
    {
        private JObject _strategyJson;
        private Dictionary<int, Node> _nodes;
        private List<int> _valueNodeIds;
        private List<int> _actionNodeIds;
        private List<int> _chartNodeIds;
        private int _position;

        public StrategyClass(JObject strategyJson)
        {
            _strategyJson = strategyJson;
            _nodes = new Dictionary<int, Node>();
            _valueNodeIds = new List<int>();
            _actionNodeIds = new List<int>();
            _chartNodeIds = new List<int>();
            _position = 0;
        }

        public void MapNodes()
        {
            // This dictionary exists so that for each node that is read through JSON, the name can be used to create the exact
            // coresponding class
            Dictionary<string, Func<int, NodeInput[], NodeOutput[], JObject, Node>> nodeCreationMappings = new Dictionary<string, Func<int, NodeInput[], NodeOutput[], JObject, Node>>()
            {
                { "CONST", (id, inputs, outputs, data) => new ConstNode(id, inputs, outputs, data) },
                { "OPEN", (id, inputs, outputs, data) => new OpenNode(id, inputs, outputs, data) },
                { "LOW", (id, inputs, outputs, data) => new LowNode(id, inputs, outputs, data) },
                { "HIGH", (id, inputs, outputs, data) => new HighNode(id, inputs, outputs, data) },
                { "CLOSE", (id, inputs, outputs, data) => new CloseNode(id, inputs, outputs, data) },
                { "VOLUME", (id, inputs, outputs, data) => new VolumeNode(id, inputs, outputs, data) },
                { "SMA", (id, inputs, outputs, data) => new SMANode(id, inputs, outputs, data) },
                { "EMA", (id, inputs, outputs, data) => new EMANode(id, inputs, outputs, data) },
                { "BOLU", (id, inputs, outputs, data) => new BOLUNode(id, inputs, outputs, data) },
                { "BOLD", (id, inputs, outputs, data) => new BOLDNode(id, inputs, outputs, data) },
                { "RSI", (id, inputs, outputs, data) => new RSINode(id, inputs, outputs, data) },
                { "MFI", (id, inputs, outputs, data) => new MFINode(id, inputs, outputs, data) },
                { "ADD", (id, inputs, outputs, data) => new AddNode(id, inputs, outputs, data) },
                { "SUBTRACT", (id, inputs, outputs, data) => new SubtractNode(id, inputs, outputs, data) },
                { "MULTIPLY", (id, inputs, outputs, data) => new MultiplyNode(id, inputs, outputs, data) },
                { "DIVIDE", (id, inputs, outputs, data) => new DivideNode(id, inputs, outputs, data) },
                { "EQUAL", (id, inputs, outputs, data) => new EqualNode(id, inputs, outputs, data) },
                { "LESS", (id, inputs, outputs, data) => new LessNode(id, inputs, outputs, data) },
                { "LESS-EQUAL", (id, inputs, outputs, data) => new LessEqualNode(id, inputs, outputs, data) },
                { "GREATER", (id, inputs, outputs, data) => new GreaterNode(id, inputs, outputs, data) },
                { "GREATER-EQUAL", (id, inputs, outputs, data) => new GreaterEqualNode(id, inputs, outputs, data) },
                { "CROSSOVER", (id, inputs, outputs, data) => new CrossoverNode(id, inputs, outputs, data) },
                { "CROSSUNDER", (id, inputs, outputs, data) => new CrossunderNode(id, inputs, outputs, data) },
                { "AND", (id, inputs, outputs, data) => new AndNode(id, inputs, outputs, data) },
                { "OR", (id, inputs, outputs, data) => new OrNode(id, inputs, outputs, data) },
                { "XOR", (id, inputs, outputs, data) => new XorNode(id, inputs, outputs, data) },
                { "NOT", (id, inputs, outputs, data) => new NotNode(id, inputs, outputs, data) },
                { "TPSLP", (id, inputs, outputs, data) => new TPSLPNode(id, inputs, outputs, data) },
                { "TPSLA", (id, inputs, outputs, data) => new TPSLANode(id, inputs, outputs, data) },
                { "IPOS", (id, inputs, outputs, data) => new IPosNode(id, inputs, outputs, data) },
                { "DPOS", (id, inputs, outputs, data) => new DPosNode(id, inputs, outputs, data) },
                { "SPOS", (id, inputs, outputs, data) => new SPosNode(id, inputs, outputs, data) },
                { "RPOS", (id, inputs, outputs, data) => new RPosNode(id, inputs, outputs, data) },
                { "CLOSE-TRADES", (id, inputs, outputs, data) => new CloseTradesNode(id, inputs, outputs, data) },
                { "INDICATOR-STOCK", (id, inputs, outputs, data) => new IndicatorStockNode(id, inputs, outputs, data) },
                { "INDICATOR", (id, inputs, outputs, data) => new IndicatorNode(id, inputs, outputs, data) },
                { "AREA-INDICATOR-STOCK", (id, inputs, outputs, data) => new AreaIndicatorStockNode(id, inputs, outputs, data) },
                { "AREA-INDICATOR", (id, inputs, outputs, data) => new AreaIndicatorNode(id, inputs, outputs, data) },
                { "CHART", (id, inputs, outputs, data) => new ChartNode(id, inputs, outputs, data) }
            };

#nullable disable
            // This algorithm loops through each of nodes stores in the JSON and creates adds it as a node object to the dictionary of nodes
            JObject jsonNodes = (JObject)_strategyJson["drawflow"]["Home"]["data"];

            // The nodes are nested properties which contain objects so each property must be iterated through and have it's value converted to a JObject
            foreach (JProperty jsonNodeProperty in jsonNodes.Properties())
            {
                JObject jsonNode = (JObject)jsonNodeProperty.Value;

                // The node Id is retrieved
                int nodeId = (int)jsonNode["id"];

                // Each node can have multiple inputs, so each of these will be looped through in order to create an array of inputs
                JProperty[] jsonNodeInputs = ((JObject)jsonNode["inputs"]).Properties().ToArray();
                NodeInput[] nodeInputs = new NodeInput[jsonNodeInputs.Length];

                for (int i = 0; i < nodeInputs.Length; i++)
                {
                    // Each input will only have one connection unless it is a chart node
                    // For this reason, as well as general consistency, an array of connections is created
                    JArray jsonInputConnections = (JArray)((JObject)jsonNodeInputs[i].Value)["connections"];
                    InputConnection[] inputConnections = new InputConnection[jsonInputConnections.Count()];

                    for (int j = 0; j < inputConnections.Length; j++)
                    {
                        // The connection is created by retrieving the Id of the node the input is from and the output Id of that node
                        int outputNodeId = (int)jsonInputConnections[j]["node"];
                        int outputId = GetInputOutputIdFromName((string)jsonInputConnections[j]["input"]);

                        inputConnections[j] = new InputConnection(outputNodeId, outputId);
                    }

                    // Node input is created
                    nodeInputs[i] = new NodeInput(inputConnections);
                }

                // Each node will only have one output, but in order to be future proof, an array of outputs is created and each output is looped through
                JProperty[] jsonNodeOutputs = ((JObject)jsonNode["outputs"]).Properties().ToArray();
                NodeOutput[] nodeOutputs = new NodeOutput[jsonNodeOutputs.Length];

                for (int i = 0; i < nodeOutputs.Length; i++)
                {
                    // Each output can have multiple connections to different inputs so an array is created to store these
                    JArray jsonOutputConnections = (JArray)((JObject)jsonNodeOutputs[i].Value)["connections"];
                    OutputConnection[] outputConnections = new OutputConnection[jsonOutputConnections.Count()];

                    for (int j = 0; j < outputConnections.Length; j++)
                    {
                        // The output connection is created by retrieving the Id of the node the ouptput goes to and the input Id of that node
                        int inputNodeId = (int)jsonOutputConnections[j]["node"];
                        int inputId = GetInputOutputIdFromName((string)jsonOutputConnections[j]["output"]);

                        outputConnections[j] = new OutputConnection(inputNodeId, inputId);
                    }

                    // Node output is created
                    nodeOutputs[i] = new NodeOutput(outputConnections);
                }

                // Finally, the node type and node name are retrieved in order to create the node and add it to the dictionary
                JObject jsonNodeData = (JObject)jsonNode["data"];

                string nodeName = (string)jsonNodeData["node_name"];

                _nodes.Add(nodeId, nodeCreationMappings[nodeName](nodeId, nodeInputs, nodeOutputs, jsonNodeData));

                // This assigns values from the JSON data to private variables in the node object
                _nodes[nodeId].MapData();

                // If the node type is value, action or chart, its index is saved in the appropriate list so nodes can be iterated through easily according to their type
                if (_nodes[nodeId].GetNodeType() == NodeType.Value)
                {
                    _valueNodeIds.Add(nodeId);
                }
                else if (_nodes[nodeId].GetNodeType() == NodeType.Action)
                {
                    _actionNodeIds.Add(nodeId);
                }
                else if (_nodes[nodeId].GetNodeType() == NodeType.Chart)
                {
                    _chartNodeIds.Add(nodeId);
                }
            }
#nullable enable
        }
        
        // This function runs the entire strategy for a given tick in the backtest. Then, any orders from the strategy are returned
        // NOTE: The chart and indicator nodes are looped through and evaluated as there is not enough time to include them in the program - more on this in the evaluation section.
        public List<Order> RunTick(IBar[] data, int currentIndex)
        {
            List<Order> orderList = new List<Order>();
            
            // First step is to update node values
            foreach (int nodeId in _valueNodeIds)
            {
                // This is done by getting the required values from the stock data and then passing it in as input
                int[] requiredIndexes = _nodes[nodeId].GetRequiredIndexes(currentIndex);

                IBar[] requiredData = new IBar[requiredIndexes.Length];

                for (int i = 0; i < requiredData.Length; i++)
                {
                    requiredData[i] = data[requiredIndexes[i]];
                }

                // Value is updated by giving it the required data
                _nodes[nodeId].UpdateValue(requiredData);
            }

            // Then, each action node must be checked to see if the action should go through
            foreach (int nodeId in _actionNodeIds)
            {
                // A null result indicates it shouldn't go through while an order does. So if the order exists, it is added to the orderList
                Order? result = (Order?)EvaluateNode(nodeId, currentIndex);

                if (result != null)
                {
                    orderList.Add(result);

                    // If the order changes the position, the position value in the strategy class must be updated
                    TradeType tradeType = result.GetTradeType();

                    // If the order is positional, the new position is retrieved from the order the order in the strategy class is changed to be that posittion
                    if (tradeType == TradeType.LongPosition || tradeType == TradeType.ShortPosition || tradeType == TradeType.ResetPosition)
                    {
                        int? newPosition = result.GetPosition();

                        if (newPosition != null) { _position = (int)newPosition; }
                    }
                }
            }

            // The order list is then returned
            return orderList;
        }

        // This function will allow any value nodes to get an accurate value before the required time frame start
        public void StartValueNodes(IBar[] marketData)
        {
            /*
            In order to do this, this function will update the values of the value nodes as if it was the actual backtest but only for the first 50 values.
            This will be done by going through each tick of the market data and letting the value node know what the current tick is. Each node will then
            decide want data it will need based on the current tick. If the data it needs has a negative index, it means it does not exist yet and will
            just continue the simulated back test. Thus, the value node will not be able to have a value on that node. This will repeat for the first 50
            ticks because the maximum range of data needed is 50 ticks.
            */

            // Each of the first 50 ticks are looped through
            for (int index = 0; index < 50; index++)
            {
                // Each of the nodes in the strategy are looped through
                foreach (int nodeId in _nodes.Keys.ToArray())
                {
                    // If the node is a value node, it will try and make sure it has a value if it doesn't already
                    if (_nodes[nodeId].GetNodeType() == NodeType.Value)
                    {
                        if (_nodes[nodeId].HasValueStarted())
                        {
                            // If the node already contains a value, then it can just update it to the next value as normal.
                            // This is done by first getting the required indexes.
                            int[] requiredIndexes = _nodes[nodeId].GetRequiredIndexes(index);

                            // Each of the indexes are looped through and the required data will be returned based on those indexes
                            IBar[] requiredBars = new IBar[requiredIndexes.Length];

                            for (int i = 0; i < requiredBars.Length; i++)
                            {
                                requiredBars[i] = marketData[requiredIndexes[i]];
                            }

                            // The value is updated
                            _nodes[nodeId].UpdateValue(requiredBars);
                        }
                        else
                        {
                            // In the event a node's value has not yet started, the required indexes will each be checked to make
                            // sure that they exist. The value will only start if they exist.
                            int[] requiredIndexesForStart = _nodes[nodeId].GetRequiredIndexesForStart(index);

                            IBar[] requiredBarsForStart = new IBar[requiredIndexesForStart.Length];

                            bool validIndexes = true;

                            for (int i = 0; i < requiredIndexesForStart.Length; i++)
                            {
                                if (requiredIndexesForStart[i] < 0)
                                {
                                    // If one of the indexes does not exist (is less than 0), it will break out the loop
                                    validIndexes = false;
                                    break;
                                }
                                else
                                {
                                    // If it exists, the data will be prepared to be sent back to the value node to start the value
                                    requiredBarsForStart[i] = marketData[requiredIndexesForStart[i]];
                                }
                            }

                            // The value will only start if validIndexes remained true while looping through each of the indexes
                            if (validIndexes)
                            {
                                _nodes[nodeId].CreateFirstValue(requiredBarsForStart);
                            }
                        }
                    }
                }
            }
        }

        // This function will evaluate a node given its node id. As different nodes can return different types, it returns an object type. In whatever context it is needed,
        // the result can then be implicitly cast. As evaluating nodes requires getting the value of other nodes, it is a recursive function.
        private Object? EvaluateNode(int nodeId, int currentIndex)
        {
            // Firstly, if the node being evaluated is a value node, it needs no other data to return the value. Therefore, empty arrays are given as its
            // parameters and the result is returned
            NodeType nodeType = _nodes[nodeId].GetNodeType();
            
            if (nodeType == NodeType.Value) { return _nodes[nodeId].EvaluateNum(Array.Empty<decimal>(), Array.Empty<bool>()); }

            // If the node isn't a value node, it will require the value of other nodes which can either be boolean or decimal. Therefore, two lists are
            // created and the index of the required nodes is retrieved
            int[] requiredNodes = _nodes[nodeId].GetRequiredValues();
            List<decimal> numInputs = new List<decimal>();
            List<bool> boolInputs = new List<bool>();

            // Each required node's index is looped through to get the value
            foreach (int requiredNode in requiredNodes)
            {
                NodeType requiredNodeType = _nodes[requiredNode].GetNodeType();

                // If the required node is an operation or value node, it will have a decimal value
                if (requiredNodeType == NodeType.Operation || requiredNodeType == NodeType.Value)
                {
                    // Return type may be null so only if it is not null will the result be added to the input list
                    decimal? result = (decimal?)EvaluateNode(requiredNode, currentIndex);

                    if (result != null) { numInputs.Add((decimal)result); }
                }
                else
                {
                    // Otherwise, it will have a boolean value. It cannot be an action node, chart node or indicator node because they do not have any outputs.
                    // Return type may be null so only if it is not null will the result be added to the input list.
                    bool? result = (bool?)EvaluateNode(requiredNode, currentIndex);

                    if (result != null) { boolInputs.Add((bool)result); }
                }
            }

            // If the node type is an operation, then it will return a decimal so the EvaluateNum() function is used on the node
            if (nodeType == NodeType.Operation)
            {
                return _nodes[nodeId].EvaluateNum(numInputs.ToArray(), boolInputs.ToArray());
            }
            else if (nodeType == NodeType.Condition || nodeType == NodeType.ConditionalOperator)
            {
                // Otherwise, if it is a condition or conditional operator type, a boolean will be returned via the EvaluateBool() function
                return _nodes[nodeId].EvaluateBool(numInputs.ToArray(), boolInputs.ToArray());
            }
            else if (nodeType == NodeType.Action)
            {
                NodeName nodeName = _nodes[nodeId].GetNodeName();

                // If the node is an increase or decrease position node, then the current position needs to be passed in to the evaluate action function
                if (nodeName == NodeName.IPos || nodeName == NodeName.DPos)
                {
                    return _nodes[nodeId].EvaluateAction(new decimal[] { _position }, boolInputs.ToArray());
                }

                // If the current node is an action node, either null or an order will be returned
                return _nodes[nodeId].EvaluateAction(numInputs.ToArray(), boolInputs.ToArray());
            }
            else
            {
                // If the node is another type, then it has been incorrectly passed into this function. Therefore, an integer value of 0 is returned.
                // Null cannot be returned but 0 can be cast into both a decimal and boolean value.
                return 0;
            }
        }

        // This function takes the name of an input or output id and reduces it to its integer number
        private int GetInputOutputIdFromName(string name)
        {
            // This works by finding the index of the underscore and taking the substring after that
            // This works because the output or input id will be in the format output_{number} or input_{number}
            int underscorePos = name.IndexOf('_');
            int integerId = Int32.Parse(name.Substring(underscorePos + 1));

            return integerId;
        }
    }
}
