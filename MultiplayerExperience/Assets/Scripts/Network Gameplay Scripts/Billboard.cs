using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Billboard : MonoBehaviour {

    // find the player camera for this player, only look at that camera

    public Camera thisPlayerCamera;

    // Update is called once per frame
    void Update () {
        transform.LookAt(thisPlayerCamera.transform);
		
	}
}
