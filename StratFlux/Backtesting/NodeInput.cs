namespace StratFlux.Backtesting
{
    public class NodeInput
    {
        private InputConnection[] _inputConnections;

        public NodeInput(InputConnection[] inputConnections)
        {
            _inputConnections = inputConnections;
        }

        public InputConnection GetInputConnection(int index)
        {
            return _inputConnections[index];
        }
    }
}
