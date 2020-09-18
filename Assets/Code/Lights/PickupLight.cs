using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

using ManicMiner.Global;
using ManicMiner.Utility;

namespace ManicMiner.Lights
{
    public class PickupLight : MonoBehaviour
    {
        // For visual animation.
        private SpriteRenderer _SpriteR = null;
        private Light2D _Light2D = null;
        private int _Degrees = 0;

        // We use 50 fixed frames per second, as in project settings.
        private TickCounter _TickerCounter = new TickCounter(ResourceManager.AnimateOnTicksForPickupLight);

        // Flags if true, we should render.
        private bool _IsTimeToDraw = false;

        // Start is called before the first frame update
        void Start()
        {
            _SpriteR = gameObject.GetComponentInParent<SpriteRenderer>();
            _Light2D = gameObject.GetComponent<Light2D>();

            Draw();
        }

        // Update is called once per frame
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
                    _IsTimeToDraw = true;
                }
            }
        }

        private void Draw()
        {
            // Change color of the light to match pickup color.
            _Light2D.color = _SpriteR.color;

            // Rotate light
            transform.eulerAngles = Vector3.forward * _Degrees;

            _Degrees += 2;

            if(_Degrees >= 90)
            {
                // Reset the multiplier
                _Degrees = 0;
            }

            _IsTimeToDraw = false;
        }
    } // Class end.
}