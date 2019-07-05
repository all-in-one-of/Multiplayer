using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 1, sendInterval = 0.1f)]
public class ColorController : NetworkBehaviour
{
    readonly Color[] colors = {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
    };

    NetworkIdentity n;

    [SyncVar(hook = "OnColorIdxChanged")]
    int colorIdx;

    Renderer[] renderers;

    public override void OnStartClient()
    {
        GetComponent<NetworkIdentity>();
        

        base.OnStartClient();
        foreach(Renderer R in renderers)
        R.material.color = colors[colorIdx];



        
    }

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.C))  
            CmdChangeColor(new System.Random().Next(0, colors.Length));
    }

    [Command]
    void CmdChangeColor(int idx)
    {
        colorIdx = idx;
    }


    void OnColorIdxChanged(int newColorIdx)
    {
        foreach (Renderer R in renderers)
            R.material.color = colors[newColorIdx];
        
    }
    }
