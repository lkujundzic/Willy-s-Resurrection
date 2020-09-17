using UnityEngine;

using ManicMiner.Global;
using ManicMiner.Utility;

namespace ManicMiner.Platforms
{
    public class Mud : MonoBehaviour
    {
        // Is sliding of the mud active.
        private bool _IsMudSliding = false;

        // For visual animation.
        private SpriteRenderer _SpriteR;
        private int _SpriteFirstIndex;
        private int _SpriteLastIndex;
        private int _SpriteIndex;

        // We use 50 fixed frames per second, as in project settings.
        private TickCounter _TickerCounter = new TickCounter(ResourceManager.AnimateOnTicksForMud);

        // Flags if true, we should render.
        private bool _IsTimeToDraw = false;

        // Start is called before the first frame update.
        void Start()
        {
            // Load array of sprites from texture.
            _SpriteR = gameObject.GetComponent<SpriteRenderer>();

            // Initial setup of the mud. This is different for every level.
            _SpriteFirstIndex = ResourceManager.FindSpriteIndex(ResourceManager.MudSprites, _SpriteR.sprite);

            _SpriteLastIndex = _SpriteFirstIndex + 7;
            _SpriteIndex = _SpriteFirstIndex;

            // Draw initial player sprite.
            Draw();
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
                if (_TickerCounter.IsItTimeToCalculate() == true && _IsMudSliding == true)
                {
                    // Yes.
                    _SpriteIndex++;

                    // Is it over last frame?
                    if (_SpriteIndex == _SpriteLastIndex + 1)
                    {
                        // Yes.
                        Destroy(gameObject);
                    }
                    else
                    {
                        // No.
                        _IsTimeToDraw = true;
                    }
                }
            }
        }

        public void Slide(bool shouldSlide)
        {
            _IsMudSliding = shouldSlide;
        }

        // Drawing sprite methods.
        private void Draw()
        {
            _SpriteR.sprite = ResourceManager.MudSprites[_SpriteIndex];
            _IsTimeToDraw = false;
        }
    } // Class end.
}
