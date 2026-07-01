using UnityEngine;

namespace ForestVR
{
    public class BluetoothButtonInput : MonoBehaviour
    {
        public bool ConfirmPressedThisFrame { get; private set; }
        public bool ConfirmHeld { get; private set; }
        public bool RecenterPressedThisFrame { get; private set; }

        private void Update()
        {
            ConfirmPressedThisFrame = ForestVRInput.ConfirmPressedThisFrame();
            ConfirmHeld = ForestVRInput.ConfirmHeld();
            RecenterPressedThisFrame = ForestVRInput.RecenterPressedThisFrame();
        }
    }
}
