using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

using ManicMiner.Global;
using ManicMiner.Utility;

namespace ManicMiner.Lights
{
    public class ElevatorLight : MonoBehaviour
    {
        // For visual animation.
        private SpriteRenderer _SpriteR = null;
        private Light2D _Light2D = null;
        private float _LightIntensity;

        // We use 50 fixed frames per second, as in project settings.
        private TickCounter _TickerCounter = new TickCounter(ResourceManager.AnimateOnTicksForElevatorLight);

        // Flags if true, we should render.
        private bool _IsTimeToDraw = false;

        // Start is called before the first frame update
        void Start()
        {
            _SpriteR = gameObject.GetComponentInParent<SpriteRenderer>();
            _Light2D = gameObject.GetComponent<Light2D>();

            _LightIntensity = _Light2D.intensity;

            _Light2D.intensity = 0;


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

                    // Is elevator active?
                    if (GameManager.ElevatorIsActive == true)
                    {
                        // Yes.
                        _Light2D.intensity = _LightIntensity;
                        _IsTimeToDraw = true;
                    }
                }
            }
        }

        private void Draw()
        {
            // Change color of the light to match pickup color.
            _Light2D.color = _SpriteR.color;

            _IsTimeToDraw = false;
        }
    } // Class end.
}