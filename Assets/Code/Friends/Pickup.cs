using UnityEngine;

using ManicMiner.Global;
using ManicMiner.Utility;

namespace ManicMiner.Friends
{
    public class Pickup : MonoBehaviour
    {
        // For visual animation.
        private SpriteRenderer _SpriteR = null;
        private int _ColorIndex = 0;

        // We use 50 fixed frames per second, as in project settings.
        private TickCounter _TickerCounter = new TickCounter(ResourceManager.AnimateOnTicksForPickup);

        // Flags if true, we should render.
        private bool _IsTimeToDraw = false;

        // Start is called before the first frame update.
        void Start()
        {
            _SpriteR = gameObject.GetComponent<SpriteRenderer>();

            // Initialize Color index.
            Color32 color = _SpriteR.color;

            // Go through all pickup colors.
            for (int count = 0; count < ResourceManager.PickupColors.Length; count++)
            {
                // Is it the same as initial color?
                if (color.r == ResourceManager.PickupColors[count].r && color.g == ResourceManager.PickupColors[count].g && color.b == ResourceManager.PickupColors[count].b)
                {
                    // Yes.
                    _ColorIndex = count;
                    break;
                }
            }

            // Register pickup for collection.
            GameManager.RegisterPickup();
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
                    _ColorIndex++;

                    // Is it over last color?
                    if (_ColorIndex == ResourceManager.PickupColors.Length)
                    {
                        // Yes.
                        _ColorIndex = 0;
                    }

                    _IsTimeToDraw = true;
                }
            }
        }

        private void Draw()
        {
            // Change color.
            _SpriteR.color = ResourceManager.PickupColors[_ColorIndex];
            _IsTimeToDraw = false;
        }
    } // Class end.
}
