using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// class to contain the Alto haptics
/// </summary>
namespace Alto
{
    public class AltoHaptics : MonoBehaviour
    {
        public enum HapticState
        {
            None,
            Ground,
            Water,
            Hover,
            Impact,
            Vehicle
        }
        private HapticState hapticState;
        public HapticState GetHapticState()
        {
            return hapticState;
        }

        //this is for haptics in the Alto
        private float hapticCounter = 0.0f;
        //used for impact haptic control
        private bool impacting = false;
        public bool CheckImpact
        {
            get { return impacting; }
        }

        void Start()
        {
            hapticState = HapticState.None;
        }

        IEnumerator ChangeHapticTimer(float time, bool impact = false, HapticState nextState = HapticState.None)
        {
            yield return new WaitForSeconds(time);
            hapticState = nextState;
            if (impact) { impacting = false; }
        }
        
        /// Public functions for altoController class
        public void ChangeHaptic(HapticState newHaptic)
        {
            if(hapticState != newHaptic && !impacting)
            {
                hapticState = newHaptic;
            }
        }

        public void ImpactHaptic(HapticState nextHaptic, float timer = 0.75f)
        {
            hapticState = HapticState.Impact;
            impacting = true;
            StartCoroutine(ChangeHapticTimer(timer, impacting, nextHaptic));
        }

        //called in AltoController Update
        public void RunHapticFeedback(int intensity, float magnitude)
        {
            magnitude = (magnitude * 0.02f) / 2.0f;
            //checks for standard haptic states, can put in custom haptic states here
            switch (hapticState)
            {
                case HapticState.None:
                    HapticFeedback("StopMotor1", 0);
                    HapticFeedback("StopMotor2", 0);
                    break;
                case HapticState.Ground:
                    HapticFeedback("StopMotor2", 0);
                    HapticFeedback("Ground", intensity);
                    break;
                case HapticState.Water:
                    //regular pulses with erratic pules on one motor and erratic pulses on the other motor
                    hapticCounter += Time.deltaTime;
                    if (hapticCounter > 2.0f) { hapticCounter = 0.0f; }
                    int wrand = (int)(Random.Range(0, 6));
                    HapticFeedback("Hover", intensity * wrand);
                    HapticFeedback("Water", intensity * (int)hapticCounter * wrand);
                    break;
                case HapticState.Hover:
                    HapticFeedback("StopMotor1", 0);
                    //erratic pulses, but within a weak range
                    int hrand = (int)(Random.Range(0, 5));
                    if (intensity <= 1)
                    {
                        hrand = 0;
                        if (magnitude >= 1) { intensity = (int)magnitude; }
                    }
                    else
                    {
                        intensity += (int)magnitude / 2;
                    }
                    HapticFeedback("Hover", intensity * hrand);
                    break;
                case HapticState.Impact:
                    HapticFeedback("StopMotor1", 0);
                    //TODO: rebounding pulse effect
                    if(intensity <= 1) { intensity += 2; }
                    HapticFeedback("Impact", intensity * 2);
                    break;
                case HapticState.Vehicle:
                    HapticFeedback("StopMotor2", 0);
                    //regular pulses with erratic pulses on both motors - combine to make a steam engine pulse effect
                    hapticCounter += Time.deltaTime;
                    if (hapticCounter > 2.0f) { hapticCounter = 0.0f; }
                    int vrand = (int)(Random.Range(0, 4));
                    HapticFeedback("Vehicle", intensity * vrand);
                    HapticFeedback("Vehicle", intensity * (int)hapticCounter * 10);
                    break;
                default:
                    break;
            }
            //TODO: add altoactive haptic,
            //sin wave like haptics

        }

        private void HapticFeedback(string name, int intensity)
        {
            AltoInput.SendHapticCommand(name, intensity);
        }

    }
}
