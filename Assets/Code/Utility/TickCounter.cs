namespace ManicMiner.Utility
{
    public class TickCounter
    {
        private int _TicksToCount;
        private int _CurrentTick;

        public TickCounter(int ticksToCount = 4)
        {
            _TicksToCount = ticksToCount;
            _CurrentTick = _TicksToCount;
        }

        public bool IsItTimeToCalculate()
        {
            bool isTime = false;
            _CurrentTick--;

            // Is it time to calculate frame?
            if (_CurrentTick == 0)
            {
                // Yes.
                _CurrentTick = _TicksToCount;
                isTime = true;
            }

            return isTime;
        }

        public void ReSetTickCounter(int ticksToCount = 4)
        {
            _TicksToCount = ticksToCount;

            if (_TicksToCount < _CurrentTick)
            {
                _CurrentTick = _TicksToCount;
            }
        }
    } // Class end.
}
