using UnityEngine;

using ManicMiner.Global;
using ManicMiner.Utility;

namespace ManicMiner.UI
{
    public class AirStripe : MonoBehaviour
    {
        private bool _ElevatorStartedCounting = false;

        // For visual animation.
        private SpriteRenderer _SpriteR;

        // Sound.
        private AudioSource _SoundSource;

        // Resources for sounds.
        [SerializeField] private AudioClip _DeathSound = null;

        // We use 50 fixed frames per second, as in project settings.
        private TickCounter _TickerCounter = new TickCounter(ResourceManager.AnimateOnTicksForAir);

        // Flags if true, we should render.
        private bool _IsTimeToDraw = false;

        // Start is called before the first frame update.
        void Start()
        {
            // Get the refrence to sprite renderer.
            _SpriteR = gameObject.GetComponent<SpriteRenderer>();

            _SoundSource = gameObject.GetComponent<AudioSource>();

            // Draw initial air stripe.
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
            // Should elevator start counting and not started counting yet?
            if (GameManager.ElevatorCounting == true && _ElevatorStartedCounting == false)
            {
                // Yes.
                _ElevatorStartedCounting = true;
                _TickerCounter.ReSetTickCounter(1);
            }

            // Is time to resetCountdown?
            if (_TickerCounter.IsItTimeToCalculate() == true)
            {
                // Decrease the oxigen, if above zero.
                if (GameManager.CurrentAirBubbles > 0)
                {
                    // Is elevator counting?
                    if (GameManager.ElevatorCounting == true)
                    {
                        // Yes.
                        GameManager.CurrentAirBubbles -= 2;
                        GameManager.AddAirScore();
                    }
                    else
                    {
                        GameManager.CurrentAirBubbles--;
                    }

                    _IsTimeToDraw = true;
                }
                else
                {
                    // Is elevator counting?
                    if (GameManager.ElevatorCounting == true)
                    {
                        // Yes.
                        // Chage state to level compete.
                        GameManager.LevelCompleted = true;
                    }
                    else
                    {
                        // No.
                        // In this case, player died.
                        _SoundSource.PlayOneShot(_DeathSound);
                        GameManager.PlayerIsDead = true;
                    }
                }
            }
        }

        // Drawing sprite method.
        private void Draw()
        {
            _SpriteR.size = new Vector2(0.125f * GameManager.CurrentAirBubbles, _SpriteR.size.y);
            _IsTimeToDraw = false;
        }
    } // Class end.
}
