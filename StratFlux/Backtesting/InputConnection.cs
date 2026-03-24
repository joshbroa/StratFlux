namespace StratFlux.Backtesting
{
    public class InputConnection
    {
        private int _nodeId;
        private int _outputId;

        public InputConnection(int nodeId, int outputId)
        {
            _nodeId = nodeId;
            _outputId = outputId;
        }

        public int GetNodeId()
        {
            return _nodeId;
        }

        public int GetNodeOutputId()
        {
            return _outputId;
        }
    }
}
