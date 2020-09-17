using UnityEngine;

namespace ManicMiner.Utility
{
    // This class is made for use in Update method of Monobehaviour.
    public class KeyController
    {
        public KeyCode Key;

        public bool Clicked = false;
        public bool Pressed = false;
        public KeyController(KeyCode key)
        {
            Key = key;
        }

        public void Refresh()
        {
            if (Input.GetKeyDown(Key))
            {
                Clicked = true;
                Pressed = true;
            }
            else
            {
                Clicked = false;

                if (Input.GetKeyUp(Key))
                {
                    Pressed = false;
                }
            }
        }

        public void Reset()
        {
            Clicked = false;
            Pressed = false;
        }
    } // Class ends.
}
