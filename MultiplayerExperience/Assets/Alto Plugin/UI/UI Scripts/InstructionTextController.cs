using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// controls which text is showing
/// child of HUD, but not a traditional UI element 
/// originally rotated to always face the player, now incorporated into HUD
/// </summary>

public class InstructionTextController : MonoBehaviour {

    //[Tooltip("The main camera/eye camera object")]
    //public GameObject HMD;
    //[Tooltip("An empty object as a child of the eye camera that is a distance in front of the camera")]
    //public Transform Target;
    //[Tooltip("Speed the text will lerp to new position - try 3.0f")]
    //public float speed;
    private TextMesh textField;
    //public bool adjusting = false;
    //private Vector3 HMDPreviousPosition;

    void Start ()
    {
        textField = gameObject.GetComponent<TextMesh>();
        //HMDPreviousPosition = HMD.transform.position; //local z and y always stay the same
	}

    /*private void AdjustPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, Target.position, speed * 0.02f);
        //check not too far up or down
        UpdateYPos();
        //move to target position
        Vector3.MoveTowards(transform.position, Target.position, speed * Time.deltaTime);
    }

    private void UpdateYPos()
    {
        float yPos = transform.localPosition.y;
        if (transform.localPosition.y > 2.0f)
        {
            yPos = 2.0f;
        }
        if (transform.localPosition.y < 0.5f)
        {
            yPos = 0.5f;
        }
        transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);
    }*/

    //called in game controller class
    public void UpdateText(string message)
    {
        //TODO: finish this to auto format message to be multiple lines if too long
        /*int splitValue = 1;
        if (message.Length >= 30)
        {
            splitValue = message.Length / 15;
        }
        for(int i = 0; i < splitValue; i++)
        {

        }*/
        textField.text = message;
    }

    /*void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, Target.position);
        if (distanceToTarget > 4.0f)
        {
            adjusting = true;          
        }
        if(distanceToTarget < 0.7f)
        {
            adjusting = false;
        }

        if (adjusting)
        {
            AdjustPosition();
        }

        transform.rotation = Quaternion.LookRotation(transform.position - HMD.transform.position);
    }*/
}
