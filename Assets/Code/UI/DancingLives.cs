using UnityEngine;

using ManicMiner.Global;
using ManicMiner.Utility;

namespace ManicMiner.UI
{
    public class DancingLives : MonoBehaviour
    {
        private int _CurrentLives = -1;

        // For visual animation.
        private SpriteRenderer _SpriteR;
        private int _SpriteIndex;

        // We use 50 fixed frames per second, as in project settings.
        private TickCounter _TickerCounter = new TickCounter(ResourceManager.AnimateOnTicksForLives);

        // Flags if true, we should render.
        private bool _IsTimeToDraw = false;

        // Start is called before the first frame update.
        void Start()
        {
            // Load array of sprites from texture.
            _SpriteR = gameObject.GetComponent<SpriteRenderer>();

            _SpriteIndex = 5;

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
        private void FixedUpdate()
        {
            if (GameManager.IsGamePaused == false)
            {
                // Is time to resetCountdown?
                if (_TickerCounter.IsItTimeToCalculate() == true)
                {
                    // Yes.
                    if (_SpriteIndex == 7)
                    {
                        _SpriteIndex = 4;
                    }
                    else
                    {
                        _SpriteIndex++;
                    }

                    _IsTimeToDraw = true;
                }
            }
        }

        // Drawing sprite method.
        private void Draw()
        {
            _SpriteR.sprite = ResourceManager.PlayerSprites[_SpriteIndex];

            if (_CurrentLives != GameManager.CurrentPlayerLives)
            {
                _CurrentLives = GameManager.CurrentPlayerLives;
                _SpriteR.size = new Vector2(2 * (_CurrentLives - 1), _SpriteR.size.y);
            }
            _IsTimeToDraw = false;
        }
    } // Class end.
}
