using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class CrossoverNode : Node
    {
        private decimal _primaryValue;
        private decimal _secondaryValue;

        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public CrossoverNode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Condition, NodeName.Crossover)
        {
            // By settings primary value to be above secondary value, the crossover will not happen just because the primary value started higher than the secondary value
            _primaryValue = 1;
            _secondaryValue = 0;
        }

        // Requests the required values
        public override int[] GetRequiredValues()
        {
            return new int[] { _inputs[0].GetInputConnection(0).GetNodeId(), _inputs[1].GetInputConnection(0).GetNodeId() };
        }

        // This checks if the primary value has crossed over the secondary value since the last tick
        public override bool EvaluateBool(decimal[] numInputs, bool[] boolInputs)
        {
            // Stores if the primary line was previously over the secondary line
            bool previouslyOver = _primaryValue >= _secondaryValue;

            _primaryValue = numInputs[0];
            _secondaryValue = numInputs[1];

            // Stores if the primary line is currently over the secondary line
            bool currentlyOver = _primaryValue >= _secondaryValue;

            // If primary line was previously over secondary, it is not possible for a crossover. However, if it wasn't, and if it over now, a crossover happened so true is returned
            if (previouslyOver)
            {
                return false;
            }
            else
            {
                if (currentlyOver)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
