using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Assertions;

public class PlayerScript : NetworkBehaviour {

    
    public GameObject playercamera;
    public GameObject playerEars;
    //[SerializeField] private GameObject playerStar;

    public static NetworkIdentity LocalPlayer;

    void Start()
    {

        if (isLocalPlayer == true)
        {
            playercamera.SetActive(true);
            playerEars.SetActive(true);
        }
        else
        {
            playercamera.SetActive(false);
            playerEars.SetActive(false);
        }
            

    }

    // Controller/keyboard movement for testing without VR
    //void Update ()
    //{
        
    //    if (!isLocalPlayer)
    //    {
    //        return;
    //    }
    //    var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
    //    var z = Input.GetAxis("Vertical") * Time.deltaTime * 10.0f;

    //    transform.Rotate(0, x, 0);
    //    transform.Translate(0, 0, z);

    //}

    public override void OnStartLocalPlayer()
    {
        LocalPlayer = GetComponent<NetworkIdentity>();
        Debug.Log("net I.D." + LocalPlayer.netId);
       // playerStar.SetActive(true);
    }

    // the network identity can also be used to identify each client so that they can take control of non-player objects in the scene 
   


}
 