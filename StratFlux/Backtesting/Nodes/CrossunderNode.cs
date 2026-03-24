using Newtonsoft.Json.Linq;

namespace StratFlux.Backtesting.Nodes
{
    public class CrossunderNode : Node
    {
        private decimal _primaryValue;
        private decimal _secondaryValue;

        // All constructor parameters are for the base class. Node type and node name are set for this individual node.
        public CrossunderNode(int id, NodeInput[] inputs, NodeOutput[] outputs, JObject jsonData)
            : base(id, inputs, outputs, jsonData, NodeType.Condition, NodeName.Crossunder)
        {
            // By settings primary value to be below secondary value, the crossunder will not happen just because the primary value started lower than the secondary value
            _primaryValue = 0;
            _secondaryValue = 1;
        }

        // Requests the required values
        public override int[] GetRequiredValues()
        {
            return new int[] { _inputs[0].GetInputConnection(0).GetNodeId(), _inputs[1].GetInputConnection(0).GetNodeId() };
        }

        // This checks if the primary value has crossed under the secondary value since the last tick
        public override bool EvaluateBool(decimal[] numInputs, bool[] boolInputs)
        {
            // Stores if the primary line was previously over the secondary line
            bool previouslyUnder = _primaryValue <= _secondaryValue;

            _primaryValue = numInputs[0];
            _secondaryValue = numInputs[1];

            // Stores if the primary line is currently over the secondary line
            bool currentlyUnder = _primaryValue <= _secondaryValue;

            // If primary line was previously under secondary, it is not possible for a crossover. However, if it wasn't, and if it over now, a crossover happened so true is returned
            if (previouslyUnder)
            {
                return false;
            }
            else
            {
                if (currentlyUnder)
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
