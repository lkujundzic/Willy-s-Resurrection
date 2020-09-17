using UnityEngine;

using ManicMiner.Global;
using ManicMiner.Utility;

namespace ManicMiner.Platforms
{
    public class Threadmill : MonoBehaviour
    {
        // For direction of threadmill spinning
        [SerializeField] private bool _IsThreadmillRotatingRight = true;

        // For visual animation.
        private SpriteRenderer _SpriteR;
        private int _SpriteFirstIndex;
        private int _SpriteLastIndex;
        private int _SpriteIndex;

        // We use 50 fixed frames per second, as in project settings.
        private TickCounter _TickerCounter = new TickCounter(ResourceManager.AnimateOnTicksForThreadmill);

        // Flags if true, we should render.
        private bool _IsTimeToDraw = false;

        // Start is called before the first frame update
        void Start()
        {
            // Load array of sprites from texture.
            _SpriteR = gameObject.GetComponent<SpriteRenderer>();

            // Initial setup of the threadmill. This is different for every level.
            GameManager.ThreadmillIsRotatingRight = _IsThreadmillRotatingRight;
            _SpriteFirstIndex = ResourceManager.FindSpriteIndex(ResourceManager.ThreadmillSprites, _SpriteR.sprite);

            _SpriteLastIndex = _SpriteFirstIndex + 7;
            _SpriteIndex = _SpriteFirstIndex;

            // Draw initial player sprite.
            Draw();
        }

        // Update is called once per frame. Actual drawing goes here.
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
                    CalculateRotateSprite();
                }
            }
        }

        private void CalculateRotateSprite()
        {
            // Is threadmill rorating right?
            if (_IsThreadmillRotatingRight == true)
            {
                // Yes.
                _SpriteIndex--;

                if (_SpriteIndex == _SpriteFirstIndex - 1)
                {
                    _SpriteIndex = _SpriteLastIndex;
                }
            }
            else
            {
                // No.
                _SpriteIndex++;

                if (_SpriteIndex == _SpriteLastIndex + 1)
                {
                    _SpriteIndex = _SpriteFirstIndex;
                }
            }

            _IsTimeToDraw = true;
        }

        // Drawing sprite.
        private void Draw()
        {
            _SpriteR.sprite = ResourceManager.ThreadmillSprites[_SpriteIndex];
            _IsTimeToDraw = false;
        }
    } // Class end.
}
