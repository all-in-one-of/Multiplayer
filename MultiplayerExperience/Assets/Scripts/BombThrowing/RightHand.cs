using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Networking;

// This is a work in progress script, currently non functioning, which will enable the player to throw a networked projectile.
// In order to avoid a complex transfer of authority from the player to the server, the script aims to have a "Fake" non-networked bullet and a "real" networked bullet visible to all players.
// The player picks up the fake bullet and throws it, at the moment they release , the fake bullet dissapears and the real bullet inherits the velocity and angular velocity from the fake one.

public class RightHand : NetworkBehaviour {

    public SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    public float throwForce = 1.5f;

    public GameObject bulletPrefab;
    private Transform FakeBulletTransform;

    //public GameObject RightToolTipsOculus;
    //public GameObject RightToolTipsVive;
    //public GameObject toolTips;


    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        /*
        if (XRDevice.model.Contains("ive")){
            toolTips = RightToolTipsVive; 
        }
        else if (XRDevice.model.Contains("culus")){
            toolTips = RightToolTipsOculus;
        }
        */
    } 
	

	void Update () {
        device = SteamVR_Controller.Input((int)trackedObj.index);
        /*
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)){
           toolTips.SetActive(true);
       }
       else if (device.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu)){
           toolTips.SetActive(false);
       }
       */
    }

    //---------------------------------------------------------------------------------------------RIGHT-GRABBING--------------------------------------------------------
    void OnTriggerStay(Collider col)
    {
        Debug.Log("OnTriggerStay detected");
        if (col.gameObject.CompareTag("Throwable")){
            Debug.Log("controller detects fake bullet ");
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)){
                GrabObject(col);
            }
            else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) {
                DropObject(col);
            }
        }
    }

    void GrabObject(Collider coli)
    {
        Debug.Log("controller trigger down, should be grabbing");
        coli.transform.SetParent(gameObject.transform);
        coli.GetComponent<Rigidbody>().isKinematic = true;
    }

    //---------------------------------------------------------------------------------------THROWING------------------------------------------

    void DropObject(Collider coli)
    {
        FakeBulletTransform = coli.gameObject.transform;
        Destroy(coli.gameObject);
        if (!isLocalPlayer)
        {
            return;
        }
        CmdSpawnProjectile();
    }


    // This network projectile spawner code needs to be moved to the PlayerScript - so that it has a root gameobject network identity
    //  this will require the details of the device velocity and angular velocity fake bullet position and rotation to be passed to the PlayerScript
    [Command]
    void CmdSpawnProjectile()
    {
        
        //create the bullet from the Bullet prefab
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            FakeBulletTransform.position,
            FakeBulletTransform.rotation);
        Rigidbody rigidbody = bullet.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.velocity = device.velocity * throwForce;
        rigidbody.angularVelocity = device.angularVelocity;

        NetworkServer.Spawn(bullet);
    }

}

