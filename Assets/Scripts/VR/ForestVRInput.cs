using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ForestVR
{
    public static class ForestVRInput
    {
        public static Vector2 ReadMove()
        {
            Vector2 move = Vector2.zero;

#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                {
                    move.x -= 1f;
                }

                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                {
                    move.x += 1f;
                }

                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                {
                    move.y -= 1f;
                }

                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                {
                    move.y += 1f;
                }
            }

            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                Vector2 stick = gamepad.leftStick.ReadValue();
                Vector2 dpad = gamepad.dpad.ReadValue();
                Vector2 gamepadMove = stick.sqrMagnitude >= dpad.sqrMagnitude ? stick : dpad;
                if (gamepadMove.sqrMagnitude > move.sqrMagnitude)
                {
                    move = gamepadMove;
                }
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            Vector2 legacyMove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (legacyMove.sqrMagnitude > move.sqrMagnitude)
            {
                move = legacyMove;
            }
#endif

            return Vector2.ClampMagnitude(move, 1f);
        }

        public static bool ConfirmPressedThisFrame()
        {
            bool pressed = false;

#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            pressed |= keyboard != null
                && (keyboard.spaceKey.wasPressedThisFrame
                    || keyboard.enterKey.wasPressedThisFrame
                    || keyboard.eKey.wasPressedThisFrame);

            Gamepad gamepad = Gamepad.current;
            pressed |= gamepad != null
                && (gamepad.buttonSouth.wasPressedThisFrame
                    || gamepad.rightTrigger.wasPressedThisFrame);
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            pressed |= Input.GetKeyDown(KeyCode.Space)
                || Input.GetKeyDown(KeyCode.Return)
                || Input.GetKeyDown(KeyCode.E)
                || Input.GetKeyDown(KeyCode.JoystickButton0);
#endif

            return pressed;
        }

        public static bool ConfirmHeld()
        {
            bool held = false;

#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            held |= keyboard != null
                && (keyboard.spaceKey.isPressed || keyboard.enterKey.isPressed || keyboard.eKey.isPressed);

            Gamepad gamepad = Gamepad.current;
            held |= gamepad != null
                && (gamepad.buttonSouth.isPressed || gamepad.rightTrigger.isPressed);
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            held |= Input.GetKey(KeyCode.Space)
                || Input.GetKey(KeyCode.Return)
                || Input.GetKey(KeyCode.E)
                || Input.GetKey(KeyCode.JoystickButton0);
#endif

            return held;
        }

        public static bool RecenterPressedThisFrame()
        {
            bool pressed = false;

#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            pressed |= keyboard != null && keyboard.rKey.wasPressedThisFrame;

            Gamepad gamepad = Gamepad.current;
            pressed |= gamepad != null && gamepad.startButton.wasPressedThisFrame;
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            pressed |= Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton7);
#endif

            return pressed;
        }
    }
}
