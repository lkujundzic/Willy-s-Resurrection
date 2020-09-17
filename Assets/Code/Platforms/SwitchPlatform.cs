using UnityEngine;

using ManicMiner.Global;

namespace ManicMiner.Platforms
{
    public class SwitchPlatform : MonoBehaviour
    {
        void FixedUpdate()
        {
            // Is the second lever activated (there are two of them)?
            if (GameManager.CurrentLeversToBeActivated == 0)
            {
                // Yes,  switch off platform.
                Destroy(gameObject);
            }
        }
    } // Class end.
}
