using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

/// <summary>
/// This adds extra functionality to the SteamVR_TrackedController class
/// this allows AltoController to access the controllers haptics 
/// and press position on the controller pad.
/// There is still some work to be done to get this working with Oculus Rift controllers
/// since they do not have a touch pad.
/// </summary>

public class AltoJetPackController : SteamVR_TrackedController
{
    public Vector2 padPosition = Vector2.zero;
    private bool inCoroutine = false;

    private void SetPadPosition(ClickedEventArgs e)
    {
        padPosition.x = e.padX;
        padPosition.y = e.padY;
    }

    public override void OnPadClicked(ClickedEventArgs e)
    {
        base.OnPadClicked(e);
        SetPadPosition(e);
    }

    public override void OnPadUnclicked(ClickedEventArgs e)
    {
        base.OnPadUnclicked(e);
        SetPadPosition(e);
    }

    public override void OnPadTouched(ClickedEventArgs e)
    {
        base.OnPadTouched(e);
        SetPadPosition(e);
    }

    public override void OnPadUntouched(ClickedEventArgs e)
    {
        base.OnPadUntouched(e);
        SetPadPosition(e);
    }

    //Taken from SteamVR_Controller.cs - this was the least buggy way to get access to controller haptics NB
    public void TriggerHapticPulse(ushort durationMicroSec = 500, EVRButtonId buttonId = EVRButtonId.k_EButton_SteamVR_Touchpad)
    {
        var system = OpenVR.System;
        if (system != null)
        {
            var axisId = (uint)buttonId - (uint)EVRButtonId.k_EButton_Axis0;
            system.TriggerHapticPulse(controllerIndex, axisId, (char)durationMicroSec);
        }
    }

    public void HapticCurve(float timeAmount, int pulse, bool increase = true)
    {
        if (!inCoroutine) { StartCoroutine(HapticPulseCurve(timeAmount, pulse, increase)); }
    }

    private IEnumerator HapticPulseCurve(float maxTime, int pulse, bool increase)
    {
        inCoroutine = true;
        float timer = 0.0f;
        float step = 0.0f;
        int currentPulse = 0;
        int pulseAmount = pulse;
        while(timer <= maxTime)
        {
            timer += Time.deltaTime;
            if (increase) { currentPulse = (int)(pulseAmount * step); }
            else { currentPulse = (int)(pulseAmount * Mathf.Abs(step - 1.0f)); }
            TriggerHapticPulse((ushort)currentPulse);
            step = timer / maxTime;
            yield return false;
        }
        inCoroutine = false;
    }
}
