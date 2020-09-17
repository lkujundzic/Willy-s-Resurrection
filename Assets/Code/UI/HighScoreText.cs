using UnityEngine;
using TMPro;

using ManicMiner.Global;

namespace ManicMiner.UI
{
    public class HighScoreText : MonoBehaviour
    {
        private int _GameHighScore;
        private TMP_Text _TMPText;

        // Start is called before the first frame update.
        void Start()
        {
            _TMPText = gameObject.GetComponent<TMP_Text>();

            _GameHighScore = GameManager.GameHighScore;
            _TMPText.text = "High Score " + _GameHighScore.ToString("D6");
        }

        // Update is called once per frame.
        void Update()
        {
            if (_GameHighScore < GameManager.GameHighScore)
            {
                _GameHighScore = GameManager.GameHighScore;
                _TMPText.text = "High Score " + _GameHighScore.ToString("D6");
            }
        }
    } // Class end.
}
