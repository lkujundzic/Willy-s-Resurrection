using UnityEngine;

using ManicMiner.Global;
using ManicMiner.Utility;

namespace ManicMiner.Friends
{
    public class Elevator : MonoBehaviour
    {
        // For visual animation.
        private Color32 _ElevatorBackgroundColor;
        private Color32 _ElevatorForegroundColor;
        private SpriteRenderer[] _SpriteRs;

        // We use 50 fixed frames per second, as in project settings.
        private TickCounter _TickerCounter = new TickCounter(ResourceManager.AnimateOnTicksForElevator);

        // Flags if true, we should render.
        private bool _IsTimeToDraw = false;

        // Start is called before the first frame update.
        void Start()
        {
            _SpriteRs = gameObject.GetComponentsInChildren<SpriteRenderer>();

            // Get colors on elevator elements.
            _ElevatorBackgroundColor = _SpriteRs[0].color;
            _ElevatorForegroundColor = _SpriteRs[1].color;
        }

        // Update is called once per frame.
        void Update()
        {
            if (_IsTimeToDraw == true)
            {
                Draw();
            }
        }

        // Update is called on fixed time intervals. 
        // For animation caclulation.
        private void FixedUpdate()
        {
            if (GameManager.IsGamePaused == false)
            {
                // Is time to resetCountdown?
                if (_TickerCounter.IsItTimeToCalculate() == true)
                {
                    // Yes.

                    // Is elevator active?
                    if (GameManager.ElevatorIsActive == true)
                    {
                        // Yes.
                        _IsTimeToDraw = true;
                    }
                }
            }
        }

        private void Draw()
        {
            Color32 tempColor;

            // Switch colors.
            tempColor = _ElevatorBackgroundColor;

            _ElevatorBackgroundColor = _ElevatorForegroundColor;
            _ElevatorForegroundColor = tempColor;

            _SpriteRs[0].color = _ElevatorBackgroundColor;
            _SpriteRs[1].color = _ElevatorForegroundColor;

            _IsTimeToDraw = false;
        }
    } // class ends.
}
