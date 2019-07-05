using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


// this script handles all the Tile movement and claiming behaviours

public class TileController : NetworkBehaviour {

    
    
    public Material OriginalMat;
    public Material RedClaimedMat;
    public Material BlueClaimedMat;


    public GameObject[] Beacons;
    public Renderer[] TileRends;
    public BoxCollider[] TileColliders;
     
    public float DegreesPerSecond = 900f;
    private Vector3 currentRot, targetRot;
    private bool rotating = false;
    bool ReadyToFlip = true;

    public NetworkScore networkScore;

    //  the syncVar will bee updated to all clients whenever it is changed 
    //when the beaconActive bool is changedd the OnBeaconActive functioon will be called 



    void Start ()
    {
        TileColliders = GetComponentsInChildren<BoxCollider>();
        currentRot = transform.eulerAngles;
        
    }

    // flips the tiles when the wave touches them, or performs claiming actions when a player touches them
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wave" && ReadyToFlip == true)
        {
            if (!isServer) 
            {
                return;
            }
            RpcPerfromRotation();
        }

        // WWISE Territory claim sound could be triggered here?
        if (other.gameObject.tag == "FirePlayer") 
        {
            if (!isServer)
            {
                return;
            } // clients don't perform the below actions, just the server

            RpcRedTeamClaimTile();
            CmdScoreTempPoints("red");
        }

        // WWISE Territory claim sound could be triggered here?
        if (other.gameObject.tag == "IcePlayer")
        {
            if (!isServer)
            {
                return;
            }// clients don't perform the below actions, just the server
            RpcBlueTeamClaimTile();
            CmdScoreTempPoints("blue");
        }
    }

    // this function will communicate the temp points to everyone in the game 
    [Command] // called by client - runs only on server
    void CmdScoreTempPoints(string team)
    {
            networkScore.ScoreTempPoints(team,1);
    }


    [ClientRpc] // This function will now be run on clients when it is called on the server. Any arguments will automatically be passed to the clients with the ClientRpc call..
    void RpcPerfromRotation()
    {
        DisableColliders();
        StartCoroutine(Rotate());
        EndRotation();
    }

    [ClientRpc] // This function will now be run on clients when it is called on the server. Any arguments will automatically be passed to the clients with the ClientRpc call..
    void RpcRedTeamClaimTile()
    {
        
        EndRotation();
        DisableColliders();
        StartCoroutine("TiletimeOut");

        foreach (GameObject beacon in Beacons)
        {
            beacon.SetActive(true);
        }
        foreach (Renderer rend in TileRends)
        {
            rend.material = RedClaimedMat;
        }
    }

    [ClientRpc] // This function will now be run on clients when it is called on the server. Any arguments will automatically be passed to the clients with the ClientRpc call..
    void RpcBlueTeamClaimTile()
    {
        EndRotation();
        DisableColliders();
        StartCoroutine("TiletimeOut");

        foreach (GameObject beacon in Beacons)
        {
            beacon.SetActive(true);
        }
        foreach (Renderer rend in TileRends)
        {
            rend.material = BlueClaimedMat;
        }
    }

    //--------------------------------------------------------------------Tile rotation---------------------------------------------------
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
            EnableColliders();

            yield return new WaitForSeconds(2);
            rotating = false;
            TileRestart();
        }
    }

    //-------------other triggered tile behaviours below

    void DisableColliders()
    {
        foreach (BoxCollider tile in TileColliders)
        {
            tile.enabled = false;
        }
    }

    void EnableColliders()
    {
        foreach (BoxCollider tile in TileColliders)
        {
            tile.enabled = true;
        }
    }

    public void EndRotation()
    {
        ReadyToFlip = false;
    }

    public void TileRestart()
    {
        ReadyToFlip = true;
    }



    IEnumerator TiletimeOut()
    {

        
        yield return new WaitForSeconds(30);
        TileRestart();
        EnableColliders();

        //below values need to be synced with a syncvar
        foreach (GameObject beacon in Beacons)
        {
            beacon.SetActive(false);
        }
        foreach (Renderer rend in TileRends)
        {
            rend.material = OriginalMat;
        }
        
    }



}

