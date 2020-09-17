using UnityEngine;

using ManicMiner.Global;
using ManicMiner.Utility;

namespace ManicMiner.Scenes
{
    public class Level : MonoBehaviour
    {
        // Music.
        private AudioSource _MusicSource;

        // Resources for sounds.
        [SerializeField] private AudioClip _AirCounterSound = null;

        // Counting of the elevator, on the end of the level.
        private bool _ElevatorStartedCounting = false;

        // We use 50 fixed frames per second, as in project settings.
        private TickCounter _TickerCounter = new TickCounter(ResourceManager.AnimateOnTicksForDeath);

        private void Awake()
        {
            // Reset keys pressed, plus remove pause, if pressed during level.
            ResourceManager.ResetKeys();
            GameManager.IsGamePaused = false;

            // Initial setup for levels. Some values are set manually for every level.
            GameManager.CurrentAirBubbles = GameManager.GameMaxLevelAirBubbles;
            GameManager.CurrentPickupsToCollect = 0;
            GameManager.CurrentLeversToBeActivated = 0;

            GameManager.ElevatorIsActive = false;
            GameManager.ElevatorCounting = false;

            GameManager.LevelCompleted = false;
            GameManager.PlayerIsDead = false;
        }

        // Start is called before the first frame update.
        void Start()
        {
            // Lets add some the music.
            _MusicSource = gameObject.GetComponent<AudioSource>();
            SwitchMusicTune();
        }

        // Update is called once per frame.
        void Update()
        {
            // Is player not dead? 
            if (GameManager.PlayerIsDead == false)
            {
                // No.

                // Did level not got completed?
                if (GameManager.LevelCompleted != true)
                {
                    // Did elevator not started counting?
                    if (GameManager.ElevatorCounting != true)
                    {
                        // Yes.
                        ResourceManager.CatchFreshGameControlButtonsPressed();

                        // Pause key.
                        if (ResourceManager.KeyForPause.Clicked == true)
                        {
                            GameManager.IsGamePaused = !GameManager.IsGamePaused;
                        }

                        // Music key.
                        if (ResourceManager.KeyForMusic.Clicked == true)
                        {
                            GameManager.IsMusicOff = !GameManager.IsMusicOff;
                            SwitchMusicTune();
                        }

                        // Quit key.
                        if (ResourceManager.KeyForQuit.Clicked == true)
                        {
                            // Yes, load menu screen
                            GameManager.RunMenu();
                        }
                    }
                    else
                    {
                        // No.
                        if (_ElevatorStartedCounting == false)
                        {
                            // Started counting, play air stripe counter, insted of game music.
                            _ElevatorStartedCounting = true;
                            _MusicSource.Stop();
                            _MusicSource.PlayOneShot(_AirCounterSound);
                        }
                    }
                }
                else
                {
                    // No.
                    // Run next level.
                    GameManager.RunNextLevel();
                }
            }
            else
            {
                // Is time to resetCountdown?
                if (_TickerCounter.IsItTimeToCalculate() == true)
                {
                    // Yes.
                    GameManager.CurrentPlayerLives--;

                    // Do player still has lives?
                    if (GameManager.CurrentPlayerLives > 0)
                    {
                        // Yes.
                        // Start level from beggining.
                        GameManager.ReloadLevel();
                    }
                    else
                    {
                        // No.
                        // Load menu screen.
                        GameManager.RunMenu();
                    }
                }
            }
        }

        //For Music control.
        private void SwitchMusicTune()
        {
            // Is the music off?
            if (GameManager.IsMusicOff == true)
            {
                // Yes.
                _MusicSource.Pause();
            }
            else
            {
                // No.
                _MusicSource.Play();
            }
        }
    } // Class ends.
}
