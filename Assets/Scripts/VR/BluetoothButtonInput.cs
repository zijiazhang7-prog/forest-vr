using UnityEngine;
using UnityEngine.Events;

namespace ForestVR
{
    public class BluetoothButtonInput : MonoBehaviour
    {
        public static BluetoothButtonInput Instance { get; private set; }

        public UnityEvent OnInteractPressed;

        public bool ConfirmPressedThisFrame { get; private set; }
        public bool ConfirmHeld { get; private set; }
        public bool RecenterPressedThisFrame { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (OnInteractPressed == null)
                OnInteractPressed = new UnityEvent();
        }

        private void Update()
        {
            ConfirmPressedThisFrame = ForestVRInput.ConfirmPressedThisFrame();
            ConfirmHeld = ForestVRInput.ConfirmHeld();
            RecenterPressedThisFrame = ForestVRInput.RecenterPressedThisFrame();

            if (ConfirmPressedThisFrame)
                OnInteractPressed?.Invoke();
        }

        public bool IsPressedThisFrame() => ConfirmPressedThisFrame;
        public bool IsHeld() => ConfirmHeld;
    }
}
