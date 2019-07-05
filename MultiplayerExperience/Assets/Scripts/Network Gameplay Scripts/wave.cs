using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// this script is responsible for moving the wave across the game grid using a coroutine
// the wave initial start is synchronised across the server and clients, and then the position is synced thereafter by the networktransform component

public class wave : NetworkBehaviour {

    // wavespeedinseconds determines the amount of time it takes the wave to get from it's start to it's end 

        // Values below set in inspector, dont bother changing here
    public Vector3 waveRestartPosition = new Vector3(-12.89178f, 3.1f, 39.6f);
    public Vector3 waveEndPosition = new Vector3(-12.89178f, 3.1f, -215);

    // this variable is synced with the music, don't change it unless you know what you're doing
    public float waveSpeedInSeconds;

    
    public void StartWave()
    {
        Debug.Log("Wave function called in wave.cs");
        if (!isServer) // wave movement just runs on the server, position is updated to clients via the network transform
        {
            return;
        }
        RpcSyncStartWave();
    }

    [ClientRpc]
    void RpcSyncStartWave()
    {
        
        StartCoroutine(MoveOverSeconds(gameObject, waveEndPosition, waveSpeedInSeconds));
    }


    // moves the wave and restarts the movement 
    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    { 
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
       
        objectToMove.transform.position = waveRestartPosition;
        StartCoroutine(MoveOverSeconds(gameObject, waveEndPosition, waveSpeedInSeconds));
    }





























    /*
        [SerializeField] private float objectSpeed = 5;
        [SerializeField] private float resetPosition = -211f;
        [SerializeField] private float startPosition = 6.5f;



        protected virtual void FixedUpdate()
        {
                transform.Translate(Vector3.back * (objectSpeed * Time.deltaTime), Space.World);

                if (transform.localPosition.z <= resetPosition)
                {
                    Vector3 newPos = new Vector3(transform.position.x , transform.position.y, transform.position.z + startPosition);
                    transform.position = newPos;
                }

        }

        */
}
