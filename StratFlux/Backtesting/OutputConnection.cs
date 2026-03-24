namespace StratFlux.Backtesting
{
    public class OutputConnection
    {
        private int _nodeId;
        private int _inputId;

        public OutputConnection(int nodeId, int inputId)
        {
            _nodeId = nodeId;
            _inputId = inputId;
        }
    }
}
