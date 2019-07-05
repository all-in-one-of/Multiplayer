using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// this matches the position of the puck, is set at start
/// the arrow quad is attached to the gameobject this script is on, place it at the correct distance from the puck edge
/// if the arrow is not aligned with the pressure on Alto the board itself needs to be calibrated - use the calibration variables on the controller Gameobject
/// detects if player has collided with an object they were travelling to and sends command for controller to play alto impact haptics
/// UPDATE: moved arrow GUI to a HUD display, this still detects impact and is used, have disabled the arrow GUI section
/// </summary>
namespace Alto
{
public class ArrowController : MonoBehaviour
{

    public AltoController altoControl;
    //for arrow height
    public GameObject puck;
    //public GameObject HMD;
    //public float arrowHeight = 0.47f;
    //for arrow direction
    private Vector3 altoDirection = Vector3.zero;
    //for arrow scale
    //public GameObject arrow;
    //public GameObject circle;
    //private Vector3 maxScale;
    //private Vector3 minScale;
    //private float minDistance; //for Z coord
    //private float maxDistance;
    public float minMagnitude = 7.0f;
    private float altoMagnitude = 0.0f;
    //private float maxMagnitude = 25.0f;

    void Start()
    {
        transform.position = puck.transform.position;
        //maxScale = new Vector3(0.48f, 0.46f, 1.0f);
        //minScale = arrow.transform.localScale; //0.2, 0.1, 1.0
        //maxDistance = 0.87f;
        //minDistance = arrow.transform.localPosition.z; //0.72f
    }

    private void GetAltoData()
    {
        altoDirection = altoControl.GetAltoDirection;
        altoMagnitude = altoControl.GetAltoMagnitude;
    }

    /*private void ScaleArrow()
    {
        //find goal vector, feed to coroutine
        float scaleValue = 0.0f;
        //magnitude is approx. between 0 and 25, will keep values below minMagnitude set in inspector from affecting the scale
        if (altoMagnitude >= minMagnitude)
        {
            arrow.GetComponent<MeshRenderer>().enabled = true;
            if (maxMagnitude >= altoMagnitude)
            {
                scaleValue = altoMagnitude / maxMagnitude;
            }
            else
            {
                maxMagnitude = altoMagnitude;
                scaleValue = 1.0f;
            }
            //apply to scaleValue to get the goalScale vector
            //((maxscale - minscale) * scaleValue) + minscale = scale to lerp to
            float differenceGoalX = (maxScale.x - minScale.x) * scaleValue + minScale.x;
            float differenceGoalY = (maxScale.y - minScale.y) * scaleValue + minScale.y;
            float differenceGoalZPos = (maxDistance - minDistance) * scaleValue + minDistance;
            Vector3 goalScale = new Vector3(differenceGoalX, differenceGoalY, 1.0f);
            arrow.transform.localScale = goalScale;
            Vector3 goalPosition = new Vector3(transform.localPosition.x, GetArrowHeight(), differenceGoalZPos);
            arrow.transform.localPosition = goalPosition;
        }
        else
        {
            arrow.GetComponent<MeshRenderer>().enabled = false;
        }
    }*/

    private void RotateArrow()
    {
        //rotate arrow to face direction given from Alto
        if (altoDirection != Vector3.zero && altoMagnitude >= minMagnitude)
        {
            Quaternion arrowRotation = Quaternion.LookRotation(altoDirection);
            transform.rotation = arrowRotation;
        }
    }

    /*private float GetArrowHeight()
    {
        float yValue = arrowHeight;   
        Vector3 circleVec = circle.transform.localPosition;
        circleVec.y = yValue;   
        if (HMD.transform.position.y >= 0.1f) //not on the ground
        {
            //TODO: this setting was giving the arrow the position of the HMD on top of it's localposition, needs to be fixed
            //yValue -= HMD.transform.position.y - arrowHeight;
            //set the circle
            circleVec.y = yValue;
            circle.transform.localPosition = circleVec;
        }
        return yValue;
    }*/

    /*private void OnTriggerEnter(Collider obj)
    {
        if (obj.tag == "Ammo")
        {
            altoControl.CheckCollision(obj.tag);
        }
        if(obj.tag.Contains("Object"))
        {
            altoControl.CheckCollision("Impact");
        }
    }*/

    /*private void OnTriggerExit(Collider obj)
    {
        if (obj.tag.Contains("Object") && altoControl.altoHaptics.CheckImpact)
        {
            //altoControl.impacting = false;
        }
    }*/

    void Update()
    {
        GetAltoData();
        //find direction to face arrow in, rotate to that angle
        RotateArrow();
        //use magnitude to change scale of arrow
        //ScaleArrow();
    }
}
}
