using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public static class VRInputHelper
{
    private static bool prevRightTrigger = false;
    private static bool prevLeftTrigger = false;
    private static bool prevRightPrimary = false;
    private static bool prevLeftPrimary = false;
    private static bool prevRightSecondary = false;
    private static bool prevLeftSecondary = false;
    private static bool prevRightGrip = false;
    private static bool prevLeftGrip = false;

    private static int lastUpdateFrame = -1;

    private static bool isRightTriggerDown = false;
    private static bool isLeftTriggerDown = false;
    private static bool isRightPrimaryDown = false;
    private static bool isLeftPrimaryDown = false;
    private static bool isRightSecondaryDown = false;
    private static bool isLeftSecondaryDown = false;
    private static bool isRightGripDown = false;
    private static bool isLeftGripDown = false;

    private static void UpdateInputStates()
    {
        if (Time.frameCount == lastUpdateFrame) return;
        lastUpdateFrame = Time.frameCount;

        InputDevice rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        bool currentRightTrigger = false;
        bool currentRightPrimary = false;
        bool currentRightSecondary = false;
        bool currentRightGrip = false;

        if (rightDevice.isValid)
        {
            if (rightDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool tb)) currentRightTrigger = tb;
            else if (rightDevice.TryGetFeatureValue(CommonUsages.trigger, out float tv)) currentRightTrigger = tv > 0.45f;

            if (rightDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool pb)) currentRightPrimary = pb;
            if (rightDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool sb)) currentRightSecondary = sb;
            if (rightDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool gb)) currentRightGrip = gb;
            else if (rightDevice.TryGetFeatureValue(CommonUsages.grip, out float gv)) currentRightGrip = gv > 0.45f;
        }

        isRightTriggerDown = currentRightTrigger && !prevRightTrigger;
        isRightPrimaryDown = currentRightPrimary && !prevRightPrimary;
        isRightSecondaryDown = currentRightSecondary && !prevRightSecondary;
        isRightGripDown = currentRightGrip && !prevRightGrip;

        prevRightTrigger = currentRightTrigger;
        prevRightPrimary = currentRightPrimary;
        prevRightSecondary = currentRightSecondary;
        prevRightGrip = currentRightGrip;

        InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        bool currentLeftTrigger = false;
        bool currentLeftPrimary = false;
        bool currentLeftSecondary = false;
        bool currentLeftGrip = false;

        if (leftDevice.isValid)
        {
            if (leftDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool tb)) currentLeftTrigger = tb;
            else if (leftDevice.TryGetFeatureValue(CommonUsages.trigger, out float tv)) currentLeftTrigger = tv > 0.45f;

            if (leftDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool pb)) currentLeftPrimary = pb;
            if (leftDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool sb)) currentLeftSecondary = sb;
            if (leftDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool gb)) currentLeftGrip = gb;
            else if (leftDevice.TryGetFeatureValue(CommonUsages.grip, out float gv)) currentLeftGrip = gv > 0.45f;
        }

        isLeftTriggerDown = currentLeftTrigger && !prevLeftTrigger;
        isLeftPrimaryDown = currentLeftPrimary && !prevLeftPrimary;
        isLeftSecondaryDown = currentLeftSecondary && !prevLeftSecondary;
        isLeftGripDown = currentLeftGrip && !prevLeftGrip;

        prevLeftTrigger = currentLeftTrigger;
        prevLeftPrimary = currentLeftPrimary;
        prevLeftSecondary = currentLeftSecondary;
        prevLeftGrip = currentLeftGrip;
    }

    public static bool IsTriggerDown()
    {
        UpdateInputStates();
        return isRightTriggerDown || isLeftTriggerDown;
    }

    public static bool IsPrimaryButtonDown()
    {
        UpdateInputStates();
        return isRightPrimaryDown || isLeftPrimaryDown;
    }

    public static bool IsSecondaryButtonDown()
    {
        UpdateInputStates();
        return isRightSecondaryDown || isLeftSecondaryDown;
    }

    public static bool IsAnyVRActionTriggered()
    {
        UpdateInputStates();
        return isRightTriggerDown || isLeftTriggerDown ||
               isRightPrimaryDown || isLeftPrimaryDown ||
               isRightSecondaryDown || isLeftSecondaryDown ||
               isRightGripDown || isLeftGripDown;
    }

    public static void TriggerHaptic(XRNode node = XRNode.RightHand, float amplitude = 0.45f, float duration = 0.1f)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        if (device.isValid)
        {
            device.SendHapticImpulse(0, Mathf.Clamp01(amplitude), Mathf.Max(0.01f, duration));
        }
    }

    public static void TriggerHapticBoth(float amplitude = 0.35f, float duration = 0.08f)
    {
        TriggerHaptic(XRNode.RightHand, amplitude, duration);
        TriggerHaptic(XRNode.LeftHand, amplitude, duration);
    }

    public static bool IsVREnvironment()
    {
        if (Application.isMobilePlatform) return true;
        if (XRSettings.isDeviceActive) return true;

        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, devices);
        return devices.Count > 0;
    }
}
