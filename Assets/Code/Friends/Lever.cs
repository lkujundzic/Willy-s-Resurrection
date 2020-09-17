using UnityEngine;

using ManicMiner.Global;

namespace ManicMiner.Friends
{
    public class Lever : MonoBehaviour
    {
        private bool _IsLeverActivated = false;

        // For visual animation.
        private SpriteRenderer _SpriteR = null;

        // Flags if true, we should render.
        private bool _IsTimeToDraw = false;

        // Start is called before the first frame update.
        void Start()
        {
            _SpriteR = gameObject.GetComponent<SpriteRenderer>();

            // Register pickup for collection.
            GameManager.RegisterLever();
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
        // For animation caclulation.
        public void ActivateLever()
        {
            if (_IsLeverActivated == false)
            {
                _IsLeverActivated = true;

                GameManager.LeverActivated();

                _IsTimeToDraw = true;
            }
        }

        private void Draw()
        {
            // Change color.
            _SpriteR.sprite = ResourceManager.LeverSprites[1];
            _IsTimeToDraw = false;
        }
    } // Class end.
}
