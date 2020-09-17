using UnityEngine;
using TMPro;

using ManicMiner.Global;

namespace ManicMiner.UI
{
    public class ScoreText : MonoBehaviour
    {
        private int _CurrentScore;
        private TMP_Text _TMPText;

        // Start is called before the first frame update.
        void Start()
        {
            _TMPText = gameObject.GetComponent<TMP_Text>();

            _CurrentScore = GameManager.CurrentScore;
            _TMPText.text = "Score " + _CurrentScore.ToString("D6");
        }

        // Update is called once per frame.
        void Update()
        {
            if (_CurrentScore < GameManager.CurrentScore)
            {
                _CurrentScore = GameManager.CurrentScore;
                _TMPText.text = "Score " + _CurrentScore.ToString("D6");
            }
        }
    } // Class end.
}
