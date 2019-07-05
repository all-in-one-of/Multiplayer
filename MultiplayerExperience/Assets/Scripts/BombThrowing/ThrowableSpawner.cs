using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ThrowableSpawner : NetworkBehaviour {

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float CoolDownTime;

    

    public bool ProjectileGrabbed;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (!isLocalPlayer)
        {
            return;
        }

        // need to have the projectile register when it's a child of the controller or is grabbed
        // then trigger the new bullet to instantiate
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdSpawnProjectile();
        }


    }

    // Commands
    // functions called from clients which only get executed on the server
    [Command]
    void CmdSpawnProjectile()
    {
        //create the bullet from the Bullet prefab
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        NetworkServer.Spawn(bullet);
    }

    IEnumerator WaitSpawnBullet()
    {
        yield return new WaitForSeconds(CoolDownTime);
        CmdSpawnProjectile();

    }
}
