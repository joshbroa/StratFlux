namespace StratFlux.Backtesting
{
    public class NodeOutput
    {
        private OutputConnection[] _outputConnections;

        public NodeOutput(OutputConnection[] outputConnections)
        {
            _outputConnections = outputConnections;
        }
    }
}
