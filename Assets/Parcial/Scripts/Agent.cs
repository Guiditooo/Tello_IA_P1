namespace FlyEngine
{
    public abstract class Agent
    {
        private Weight weightsTable;
        //private FSM fsm;
        public Agent(Weight weights)
        {
            weightsTable = weights;
        }
    }
}