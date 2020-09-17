using UnityEngine;

using ManicMiner.Global;
using ManicMiner.Utility;

namespace ManicMiner.UI
{
    public class Scroller : MonoBehaviour
    {
        // Scrolling text parameters.
        private float _StartPositionOnX = 33f;
        private float _PositionOnX = 33f;
        private float _SizeToMove = 310f;

        // We use 50 fixed frames per second, as in project settings.
        private TickCounter _TickerCounter = new TickCounter(ResourceManager.AnimateOnTicksForScroller);

        // Flags if true, we should render.
        private bool _IsTimeToDraw = false;

        // Start is called before the first frame update.
        void Start()
        {
            _PositionOnX = _StartPositionOnX;
        }

        // Update is called once per frame.
        void Update()
        {
            if (_IsTimeToDraw == true)
            {
                transform.position = new Vector3(_PositionOnX, transform.position.y);
                _IsTimeToDraw = false;
            }
        }

        // Update is called on fixed time intervals. 
        private void FixedUpdate()
        {
            // Is time to resetCountdown?
            if (_TickerCounter.IsItTimeToCalculate() == true)
            {
                // Is it past end of scrolling?
                if (_PositionOnX + _SizeToMove < 0)
                {
                    // Yes.
                    _PositionOnX = _StartPositionOnX;
                }

                _PositionOnX -= 0.250f;
                _IsTimeToDraw = true;
            }
        }
    } // Class end.
}
