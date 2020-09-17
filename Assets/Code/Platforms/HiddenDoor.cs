using UnityEngine;

using ManicMiner.Global;

namespace ManicMiner.Platforms
{
    public class HiddenDoor : MonoBehaviour
    {
        void FixedUpdate()
        {
            // Is the first lever activated (there are two of them)?
            if (GameManager.CurrentLeversToBeActivated == 1)
            {
                // Yes,  open hidden door.
                Destroy(gameObject);
            }
        }
    } // Class end.
}
