using UnityEngine;

using ManicMiner.Global;
using ManicMiner.Utility;

namespace ManicMiner.Friends
{
    public class Sunbeams : MonoBehaviour
    {
        private int _ActiveSunbeam = 1;
        private int _NextSunbeam = 1;

        private GameObject[] _Sunbeams = new GameObject[7];

        // We use 50 fixed frames per second, as in project settings.
        private TickCounter _TickerCounter = new TickCounter(17);

        // Flags if true, we should render.
        private bool _IsTimeToDraw = false;

        // Start is called before the first frame update.
        void Start()
        {
            // Register all sunbeams objects.
            for (int c = 0; c < _Sunbeams.Length; c++)
            {
                _Sunbeams[c] = transform.Find("Sunbeam" + (c + 1).ToString()).gameObject;

                // Is it not active sunbean?
                if ((c + 1) != _ActiveSunbeam)
                {
                    // Yes, deactivate it.
                    _Sunbeams[c].SetActive(false);
                }
            }
        }

        // Update is called once per frame. Actual drawing goes here.
        void Update()
        {
            if (_IsTimeToDraw == true)
            {
                Draw();
            }
        }

        // Fixed update is called on fixed time intervals. 
        private void FixedUpdate()
        {
            if (GameManager.IsGamePaused == false)
            {
                // Is time to resetCountdown?
                if (_TickerCounter.IsItTimeToCalculate() == true)
                {
                    // Yes.
                    // Get random sunbeam: 1 to 7.
                    _NextSunbeam = Mathf.FloorToInt(Random.Range(1.0f, 7.99f));

                    // Get random tick count: from 4 to 45.
                    _TickerCounter.ReSetTickCounter(Mathf.FloorToInt(Random.Range(4.0f, 45.99f)));

                    _IsTimeToDraw = true;
                }
            }
        }

        private void Draw()
        {
            // Deactivate active sunbeam first.
            _Sunbeams[_ActiveSunbeam - 1].SetActive(false);

            // Activate new one.
            _Sunbeams[_NextSunbeam - 1].SetActive(true);
            _ActiveSunbeam = _NextSunbeam;

            _IsTimeToDraw = false;
        }
    } // Class end.
}
