using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ForestVR
{
    public class EditorMouseLook : MonoBehaviour
    {
        [SerializeField] private bool requireRightMouse = true;
        [SerializeField] private float sensitivity = 2.2f;
        [SerializeField] private float minPitch = -75f;
        [SerializeField] private float maxPitch = 75f;

        private float yaw;
        private float pitch;

        private void Start()
        {
            Vector3 euler = transform.localEulerAngles;
            yaw = euler.y;
            pitch = NormalizeAngle(euler.x);
        }

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            Vector2 delta = Vector2.zero;
            bool canLook = !requireRightMouse;

#if ENABLE_INPUT_SYSTEM
            Mouse mouse = Mouse.current;
            if (mouse != null)
            {
                canLook |= mouse.rightButton.isPressed;
                delta = mouse.delta.ReadValue();
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            canLook |= Input.GetMouseButton(1);
            delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 12f;
#endif

            if (!canLook)
            {
                return;
            }

            yaw += delta.x * sensitivity * 0.08f;
            pitch -= delta.y * sensitivity * 0.08f;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
#endif
        }

        private static float NormalizeAngle(float angle)
        {
            return angle > 180f ? angle - 360f : angle;
        }
    }
}
