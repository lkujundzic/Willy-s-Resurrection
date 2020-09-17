using UnityEngine;

using ManicMiner.Global;

namespace ManicMiner.Scenes
{
    public class Menu : MonoBehaviour
    {
        // Music.
        private AudioSource _MusicSource;

        private void Awake()
        {
            GameManager.CurrentLevel = 0;
#if UNITY_STANDALONE
            Cursor.visible = false;
#endif
        }

        // Start is called before the first frame update.
        void Start()
        {
            // Lets add some the music.
            _MusicSource = gameObject.GetComponent<AudioSource>();
            SwitchMusicTune();
        }

        // Update is called once per frame
        void Update()
        {
            ResourceManager.CatchFreshGameControlButtonsPressed();

            // Game start key.
            if (ResourceManager.KeyForGameStart.Clicked == true)
            {
                GameManager.RunGame();
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
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif

                Application.Quit();
            }
        }

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
    } // Class end.
}
