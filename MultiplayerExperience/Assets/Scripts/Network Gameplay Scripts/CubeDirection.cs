using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeDirection : MonoBehaviour {

    public float DegreesPerSecond = 180f; // degrees per second
    private Vector3 currentRot, targetRot;
    private bool rotating = false;

    public CubeColour currentColour;

    public enum CubeColour {Red, Green, Blue, White};

    

    // Use this for initialization
    void Awake () {
        
        currentColour = CubeColour.Red;
	}

    void Start()
    {
        currentRot = transform.eulerAngles;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartRotation(); 
        }
    }

    public void StartRotation()
    {
        StartCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        if (!rotating)
        {
            rotating = true;  // set the flag
            targetRot.x = currentRot.x + 90.0f; // calculate the new angle

            while (currentRot.x < targetRot.x)
            {
                currentRot.x = Mathf.MoveTowardsAngle(currentRot.x, targetRot.x, DegreesPerSecond * Time.deltaTime);
                transform.eulerAngles = currentRot;
                yield return null;
            }
            rotating = false;
            if (currentColour != CubeColour.White)
            {
                currentColour++;
            }
            else {
                currentColour = CubeColour.Red;
            }
        }
    }





}
