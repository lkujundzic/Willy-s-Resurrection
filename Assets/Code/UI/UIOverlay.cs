using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ManicMiner.Global;
using ManicMiner.Utility;

namespace ManicMiner.UI
{
    public class UIOverlay : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            // Is game in demo mode?
            if(GameManager.IsGameInDemoMode == true)
            {
                // Yes, destroy most of the ui.
                //Destroy(transform.Find("RedStripe").gameObject);
                //Destroy(transform.Find("GreenStripe").gameObject);
                //Destroy(transform.Find("AirStripe").gameObject);
                //Destroy(transform.Find("AirText").gameObject);
                Destroy(transform.Find("HighScoreText").gameObject);
                Destroy(transform.Find("ScoreText").gameObject);
                Destroy(transform.Find("Lives").gameObject);
            }
        }
    } // Class end.
}
