
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Wave Controller is actually respoinsible for the Management of music tracks depending on the score

public class WaveController : NetworkBehaviour
{

    public AK.Wwise.Event PlayMusicEvent = null;
    public AK.Wwise.Event StopMusicEvent = null;

    public GameObject Wave;

    public NetworkScore networkScore;

    public int Round = 0;


    [SyncVar]
    public int RandomNumber;

    [Command]
    public void CmdServerMakeARandomNumber()
    {
        LocalRandomNumbergenerator();
    }

    public void LocalRandomNumbergenerator()
    {
        RandomNumber = Random.Range(0, 2);
    }

    public void Awake()
    {
        IdleMusicState();
    }

    public void IdleMusicState()
    {
        AkSoundEngine.SetState("Arkryas_Bass_Track", "A_Bass_OFF");
        AkSoundEngine.SetState("Arkryas_Chords_Track", "A_Chords_OFF");
        AkSoundEngine.SetState("Arkryas_Lead_Track", "A_Lead_OFF");
        AkSoundEngine.SetState("Arkryas_Drums_Track", "A_Drums_OFF");
        AkSoundEngine.SetState("Nethmi_Bass_Track", "N_Bass_OFF");
        AkSoundEngine.SetState("Nethmi_Chords_Track", "N_Chords_OFF");
        AkSoundEngine.SetState("Nethmi_Lead_Track", "N_Lead_OFF");
        AkSoundEngine.SetState("Nethmi_Drums_Track", "N_Drums_OFF");
        AkSoundEngine.SetState("NeutralBeat", "HasNotReachedLevel3");
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Wave") // currently all this code is called on all copies of this scipt everywhere 
        {
            if (Round == 0 && Wave.layer == LayerMask.NameToLayer("InitDrumWave"))
            {
                PlayMusicEvent.Post(gameObject);
                Round = 1;
                return;
            }
            if (Round == 3)// trigger final rounds drums
            {
                AkSoundEngine.SetState("NeutralBeat", "HasReachedLevel3");
                Debug.Log("drums neutralbeat at level 3");
            }
            if (Round > 4) // last round, game over
            {
                other.gameObject.tag = "Untagged";
                Wave.layer = LayerMask.NameToLayer("Default");
                IdleMusicState();
                Debug.Log("Game over music state");
                networkScore.ShowGameOvertext();
                Wave.SetActive(false);
                return;
            }


            // this handles the initial case of the first wave and sets the game over music state
            if (Wave.layer == LayerMask.NameToLayer("InitDrumWave") && Round >= 1 || Wave.layer == LayerMask.NameToLayer("Wave4Drums"))
            {
                Wave.layer = LayerMask.NameToLayer("Wave1Bass");
                if (!isServer)
                {
                    return;
                }
                RpcSetTrack(Round, "Bass");
                return;
            }

            // the below functions would be better as an enum - they cycle through the wave instrument types

            else if (Wave.layer == LayerMask.NameToLayer("Wave1Bass"))
            {
                Wave.layer = LayerMask.NameToLayer("Wave2Chords");
                if (!isServer)
                {
                    return;
                }
                RpcSetTrack(Round, "Chords");

                return;
            }
            else if (Wave.layer == LayerMask.NameToLayer("Wave2Chords"))
            {
                Wave.layer = LayerMask.NameToLayer("Wave3Lead");
                if (!isServer)
                {
                    return;
                }
                RpcSetTrack(Round, "Lead");
                return;
            }
            else if (Wave.layer == LayerMask.NameToLayer("Wave3Lead"))
            {

                int thisRound = Round; // caches the value of current round to use in the Rpc call. 
                Wave.layer = LayerMask.NameToLayer("Wave4Drums");
                Round++; // increases the round on clients and server ready for the next collision (bass) to read it
                if (!isServer)
                {
                    return;
                }
                RpcSetTrack(thisRound, "Drums");
                return;
            }

        }
    }

    // try to write a general fucntion music picker for blue red and draw, which is passed values for Round and Team

    [ClientRpc]// set track called on all clients FROM the server
    public void RpcSetTrack(int round, string instrument)
    {
        Debug.Log("Checking the score");
        if (networkScore.currentBlueTempScore > networkScore.currentRedTempScore)
        {
            // Blue Temp win
            networkScore.ScorePermanentPoints("Arkryas");
            if (!isServer)
            {
                return;
            }
            RpcSetInstrumentBlue(round, instrument);
        }
        if (networkScore.currentBlueTempScore < networkScore.currentRedTempScore)
        {
            // Red Temp win
            networkScore.ScorePermanentPoints("Nethmi");
            if (!isServer)
            {
                return;
            }
            RpcSetInstrumentRed(round, instrument);
        }
        if (networkScore.currentRedTempScore == networkScore.currentBlueTempScore)
        {
            Debug.Log("Draw state");
            if (!isServer)
            {
                return;
            }
            CmdServerMakeARandomNumber();
            if (RandomNumber == 0)
            {
                RpcSetInstrumentRed(round, instrument);
            }
            else
            {
                RpcSetInstrumentBlue(round, instrument);
            }
        }
        if (!isServer)
        {
            return;
        }
        CmdResetTempPoints();
    }

    [ClientRpc]
    private void RpcSetInstrumentBlue(int round, string instrument)
    {
        Debug.Log("Blue Temp win actions");
        AkSoundEngine.SetState("Nethmi_" + instrument + "_Track", "N_" + instrument + "_OFF");
        AkSoundEngine.SetState("Arkryas_" + instrument + "_Track", "A_" + instrument + round);
        Debug.Log("round " + round + "Blue or Arkryas" + instrument);
    }

    [ClientRpc]
    private void RpcSetInstrumentRed(int round, string instrument)
    {
        Debug.Log("Red Temp win actions");
        AkSoundEngine.SetState("Arkryas_" + instrument + "_Track", "A_" + instrument + "_OFF");
        AkSoundEngine.SetState("Nethmi_" + instrument + "_Track", "N_" + instrument + round);
        Debug.Log("round " + round + "Red or Nethmi" + instrument);
    }

    [Command]
    public void CmdResetTempPoints()
    {
        networkScore.ResetTempPoints();
    }
}
