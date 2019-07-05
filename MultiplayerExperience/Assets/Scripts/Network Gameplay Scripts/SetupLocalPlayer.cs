using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// this script is responsible for setting the colour of the player, as chosen in the Lobby. 
// It also obtains the player's unique NET ID which is then used by the Gamemanager to count players 

[RequireComponent(typeof(PlayerScript))]
public class SetupLocalPlayer : NetworkBehaviour {


    // synchronises the current player color (red is default) across the server and all clients
    [SyncVar]
    public Color playerColor = Color.red;


    void Start()
    {
        // PlayerColor chosen in Lobby
        if (playerColor == Color.red)
        {
            // find the red player avatar (cockpit) and activate it

            Transform[] trans = GetComponentsInChildren<Transform>();
            foreach (Transform t in trans)
            {
                t.gameObject.tag = "FirePlayer";

            }
            this.gameObject.tag = "FirePlayer";
            Renderer[] rends = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in rends)
            r.material.color = Color.red;
        }
        else
        {
            // find the blue player avatar cockpit and activate that instead

            Transform[] trans = GetComponentsInChildren<Transform>();
            foreach (Transform t in trans)
            {
                t.gameObject.tag = "IcePlayer";
            }
            this.gameObject.tag = "IcePlayer";
            Renderer[] rends = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in rends)
            r.material.color = Color.blue;
        }
    }


    //---------------------NET ID Stuff for player counting

         public override void OnStartClient()
        {
            base.OnStartClient();
        
            string _netID = GetComponent<NetworkIdentity>().netId.ToString();
            PlayerScript _player = GetComponent<PlayerScript>();
            GameManager.RegisterPlayer(_netID, _player);
        }

}




