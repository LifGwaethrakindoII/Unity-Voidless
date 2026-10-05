using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Voidless.OldInputSystem
{
    public static class InputControllerExtensions
    {
        /// <summary>Converts XBoxInputKey enumerator value to KeyCode value.</summary>
        /// <param name="_XBoxInputKey">XBoxInputKey value.</param>
        /// <returns>XBoxInputKey value to KeyCode, mapped relative to the platform.</returns>
        public static KeyCode ToKeyCode(this XBoxInputKey _XBoxInputKey)
        {
            switch(_XBoxInputKey)
            {
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_XBOXONE || UNITY_WSA_10_0 || UNITY_WSA)
                case XBoxInputKey.A:                return KeyCode.JoystickButton0;
                case XBoxInputKey.B:                return KeyCode.JoystickButton1;
                case XBoxInputKey.X:                return KeyCode.JoystickButton2;
                case XBoxInputKey.Y:                return KeyCode.JoystickButton3;
                case XBoxInputKey.LB:               return KeyCode.JoystickButton4;
                case XBoxInputKey.RB:               return KeyCode.JoystickButton5;
                case XBoxInputKey.Back:             return KeyCode.JoystickButton6;
                case XBoxInputKey.Start:            return KeyCode.JoystickButton7;
                case XBoxInputKey.LeftStickClick:   return KeyCode.JoystickButton8;
                case XBoxInputKey.RightStickClick:  return KeyCode.JoystickButton9;
#elif (UNITY_STANDALONE_LINUX || UNITY_XBOXONE || UNITY_WSA_10_0 || UNITY_WSA)
                case XBoxInputKey.A:                return KeyCode.JoystickButton0;
                case XBoxInputKey.B:                return KeyCode.JoystickButton1;
                case XBoxInputKey.X:                return KeyCode.JoystickButton2;
                case XBoxInputKey.Y:                return KeyCode.JoystickButton3;
                case XBoxInputKey.LB:               return KeyCode.JoystickButton4;
                case XBoxInputKey.RB:               return KeyCode.JoystickButton5;
                case XBoxInputKey.Back:             return KeyCode.JoystickButton6;
                case XBoxInputKey.Start:            return KeyCode.JoystickButton7;
                case XBoxInputKey.LeftStickClick:   return KeyCode.JoystickButton9;
                case XBoxInputKey.RightStickClick:  return KeyCode.JoystickButton10;
                case XBoxInputKey.DPadUp:           return KeyCode.JoystickButton13;
                case XBoxInputKey.DPadDown:         return KeyCode.JoystickButton14;
                case XBoxInputKey.DPadLeft:         return KeyCode.JoystickButton11;
                case XBoxInputKey.DPadRight:        return KeyCode.JoystickButton12;
#elif (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_XBOXONE || UNITY_WSA_10_0 || UNITY_WSA)
                case XBoxInputKey.A:                return KeyCode.JoystickButton16;
                case XBoxInputKey.B:                return KeyCode.JoystickButton17;
                case XBoxInputKey.X:                return KeyCode.JoystickButton18;
                case XBoxInputKey.Y:                return KeyCode.JoystickButton19;
                case XBoxInputKey.LB:               return KeyCode.JoystickButton13;
                case XBoxInputKey.RB:               return KeyCode.JoystickButton14;
                case XBoxInputKey.Back:             return KeyCode.JoystickButton10;
                case XBoxInputKey.Start:            return KeyCode.JoystickButton9;
                case XBoxInputKey.LeftStickClick:   return KeyCode.JoystickButton11;
                case XBoxInputKey.RightStickClick:  return KeyCode.JoystickButton12;
                case XBoxInputKey.DPadUp:           return KeyCode.JoystickButton5;
                case XBoxInputKey.DPadDown:         return KeyCode.JoystickButton6;
                case XBoxInputKey.DPadLeft:         return KeyCode.JoystickButton7;
                case XBoxInputKey.DPadRight:        return KeyCode.JoystickButton8;
                case XBoxInputKey.XBoxButton:       return KeyCode.JoystickButton15;
#endif
                }

                return KeyCode.None;
        }

        /// <summary>Converts NintendoSwitchButton enumerator value to KeyCode value.</summary>
        /// <param name="_NintendoSwitchButton">NintendoSwitchButton value.</param>
        /// <returns>NintendoSwitchButton value to KeyCode, mapped relative to the platform.</returns>
        public static KeyCode ToKeyCode(NintendoSwitchButton _NintendoSwitchButton)
        {
            /*switch(_NintendoSwitchButton)
            {
                case NintendoSwitchButton.B:                return KeyCode.JoystickButton0;
                case NintendoSwitchButton.A:                return KeyCode.JoystickButton1;
                case NintendoSwitchButton.Y:                return KeyCode.JoystickButton2;
                case NintendoSwitchButton.X:                return KeyCode.JoystickButton3;
                case NintendoSwitchButton.L:                return KeyCode.JoystickButton4;
                case NintendoSwitchButton.R:                return KeyCode.JoystickButton5;
                case NintendoSwitchButton.Minus:            return KeyCode.JoystickButton6;
                case NintendoSwitchButton.Plus:             return KeyCode.JoystickButton7;
                case NintendoSwitchButton.LeftStick:        return KeyCode.JoystickButton8;
                case NintendoSwitchButton.RightStick:   return KeyCode.JoystickButton9;
                case NintendoSwitchButton.ZL:           return KeyCode.JoystickButton10;
                case NintendoSwitchButton.ZR:           return KeyCode.JoystickButton11;
                case NintendoSwitchButton.Down:             return KeyCode.JoystickButton12;
                case NintendoSwitchButton.Right:            return KeyCode.JoystickButton13;
                case NintendoSwitchButton.Left:             return KeyCode.JoystickButton14;
                case NintendoSwitchButton.Up:           return KeyCode.JoystickButton15;
            }*/

            return KeyCode.None;
        }

        /// <summary>Begins an Input Mashing Sequence [With InputController's API].</summary>
        /// <param name="inputID">KeyCode to press during the sequence.</param>
        /// <param name="acceleration">Acceleration rate when the Input's ID is pressed in a frame (dividedd by the frame rate).</param>
        /// <param name="decceleration">Decceleration rate when the Input's ID in not pressed in a frame (divided by the frame rate).</param>
        /// <param name="minLimit">Minimum tolerance value.</param>
        /// <param name="maxLimit">Max limit where the sequence is considered a success.</param>
        /// <param name="onFailed">Optional Callback invoked when the sequence has failed [when the value passes the minimum limit].</param>
        /// <param name="onSucceeded">Optional Callback invoked when the sequence has been successfully done.</param>
        public static IEnumerator<float> InputMashingSequence(int inputID, float acceleration, float decceleration, float minLimit, float maxLimit, Action onFailed = null, Action onSucceeded = null)
        {
            if(InputController.Instance == null) yield break;

            float current = 0.0f;
            float progress = 0.0f;
            bool inputEntered = false;
            bool inputEnteredLastFrame = false;

            while(current > minLimit && current < maxLimit)
            {
                inputEntered = InputController.InputBegin(inputID);

                if(!inputEnteredLastFrame)
                {
                    current += (inputEntered ? acceleration : -decceleration) * Time.deltaTime;
                    progress = Mathf.Clamp(VMath.RemapValueToNormalizedRange(current, minLimit, maxLimit), 0.0f, 1.0f);
                    inputEnteredLastFrame = inputEntered;
                }
                else inputEnteredLastFrame = false;

                yield return progress;
            }

            if(current <= minLimit && onFailed != null)
            {
                progress = 0.0f;
                onFailed();

            } else if(current >= maxLimit && onSucceeded != null)
            {
                progress = 1.0f;
                onSucceeded();
            }
        }

        /// <summary>Utility function to subscribe object implementing IInputControllerHandler to InputController's events.</summary>
        /// <param name="_controllerHandler">IInputControllerHandler object to subscribe to events.</param>
        public static void SubscribeToInputControllerEvents(this IInputControllerHandler _controllerHandler)
        {
            InputController.onInputReceived += _controllerHandler.OnInputReceived;
            InputController.onRightAxesChange += _controllerHandler.OnRightAxesChange;
            InputController.onLeftAxesChange += _controllerHandler.OnLeftAxesChange;
            InputController.onRightTriggerAxisChange += _controllerHandler.OnRightTriggerAxisChange;
            InputController.onLeftTriggerAxisChange += _controllerHandler.OnLeftTriggerAxisChange;
            InputController.onDPadAxesChanges += _controllerHandler.OnDPadAxesChanges;
        }

        /// <summary>Utility function to unsubscribe object implementing IInputControllerHandler to InputController's events.</summary>
        /// <param name="_controllerHandler">IInputControllerHandler object to unsubscribe to events.</param>
        public static void UnsubscribeToInputControllerEvents(this IInputControllerHandler _controllerHandler)
        {
            InputController.onInputReceived -= _controllerHandler.OnInputReceived;
            InputController.onRightAxesChange -= _controllerHandler.OnRightAxesChange;
            InputController.onLeftAxesChange -= _controllerHandler.OnLeftAxesChange;
            InputController.onRightTriggerAxisChange -= _controllerHandler.OnRightTriggerAxisChange;
            InputController.onLeftTriggerAxisChange -= _controllerHandler.OnLeftTriggerAxisChange;
            InputController.onDPadAxesChanges -= _controllerHandler.OnDPadAxesChanges;
        }
    }
}