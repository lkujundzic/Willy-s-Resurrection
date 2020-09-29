using UnityEngine;

using ManicMiner.Global;
using ManicMiner.Utility;

namespace ManicMiner.UI
{
    public class Scroller : MonoBehaviour
    {
        // Scrolling text parameters.
        private float _StartPositionOnX = 33f;
        private float _PositionOnX = 33f;
        private float _Speed = 13f;
        private float _SizeToMove = 310f;

        // Start is called before the first frame update.
        void Start()
        {
            _PositionOnX = _StartPositionOnX;
        }

        // Update is called once per frame.
        void Update()
        {
            // Is it past end of scrolling?
            if (_PositionOnX + _SizeToMove < 0)
            {
                // Yes, start demo mode.
                //_PositionOnX = _StartPositionOnX;
                GameManager.RunDemoMode();
            }

            _PositionOnX -= _Speed * Time.deltaTime;
            transform.position = new Vector3(_PositionOnX, transform.position.y);
        }
    } // Class end.
}
