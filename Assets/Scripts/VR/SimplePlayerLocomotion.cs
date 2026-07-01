using UnityEngine;

namespace ForestVR
{
    [RequireComponent(typeof(CharacterController))]
    public class SimplePlayerLocomotion : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform viewTransform;

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 1.8f;
        [SerializeField] private float sprintSpeed = 2.6f;
        [SerializeField] private float gravity = -18f;
        [SerializeField] private float groundedStickForce = -2f;

        private CharacterController controller;
        private float verticalVelocity;

        public Transform ViewTransform
        {
            get => viewTransform;
            set => viewTransform = value;
        }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (viewTransform == null && Camera.main != null)
            {
                viewTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            Vector2 input = ForestVRInput.ReadMove();
            Transform view = viewTransform != null ? viewTransform : transform;

            Vector3 forward = Vector3.ProjectOnPlane(view.forward, Vector3.up);
            if (forward.sqrMagnitude < 0.0001f)
            {
                forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            }

            forward.Normalize();
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
            Vector3 planarMove = forward * input.y + right * input.x;

            float speed = IsSprintHeld() ? sprintSpeed : walkSpeed;
            if (planarMove.sqrMagnitude > 1f)
            {
                planarMove.Normalize();
            }

            if (controller.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = groundedStickForce;
            }

            verticalVelocity += gravity * Time.deltaTime;

            Vector3 velocity = planarMove * speed;
            velocity.y = verticalVelocity;
            controller.Move(velocity * Time.deltaTime);
        }

        private static bool IsSprintHeld()
        {
#if ENABLE_INPUT_SYSTEM
            UnityEngine.InputSystem.Keyboard keyboard = UnityEngine.InputSystem.Keyboard.current;
            if (keyboard != null && (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed))
            {
                return true;
            }

            UnityEngine.InputSystem.Gamepad gamepad = UnityEngine.InputSystem.Gamepad.current;
            if (gamepad != null && gamepad.leftStickButton.isPressed)
            {
                return true;
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKey(KeyCode.LeftShift)
                || Input.GetKey(KeyCode.RightShift)
                || Input.GetKey(KeyCode.JoystickButton8))
            {
                return true;
            }
#endif

            return false;
        }
    }
}
