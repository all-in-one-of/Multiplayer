using UnityEngine;

public class MusicTestScript : MonoBehaviour
{
    public AK.Wwise.Event PlayMusicEvent = null;
    public AK.Wwise.Event StopMusicEvent = null;

    public void Awake()
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


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Playmusic event, spacebar pressed");
            PlayMusicEvent.Post(gameObject);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Stopmusic event, Enter Key pressed");
            StopMusicEvent.Post(gameObject);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("A Bass1");
            AkSoundEngine.SetState("Arkryas_Bass_Track", "A_Bass1");
            AkSoundEngine.SetState("Nethmi_Bass_Track", "N_Bass_OFF");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("A Bass2");
            AkSoundEngine.SetState("Arkryas_Bass_Track", "A_Bass2");
            AkSoundEngine.SetState("Nethmi_Bass_Track", "N_Bass_OFF");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("A Bass3");
            AkSoundEngine.SetState("Arkryas_Bass_Track", "A_Bass3");
            AkSoundEngine.SetState("Nethmi_Bass_Track", "N_Bass_OFF");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("N Bass1");
            AkSoundEngine.SetState("Arkryas_Bass_Track", "A_Bass_OFF");
            AkSoundEngine.SetState("Nethmi_Bass_Track", "N_Bass1");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("N Bass2");
            AkSoundEngine.SetState("Arkryas_Bass_Track", "A_Bass_OFF");
            AkSoundEngine.SetState("Nethmi_Bass_Track", "N_Bass2");
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("N Bass3");
            AkSoundEngine.SetState("Arkryas_Bass_Track", "A_Bass_OFF");
            AkSoundEngine.SetState("Nethmi_Bass_Track", "N_Bass3");
        }

    }
}
