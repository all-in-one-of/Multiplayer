using UnityEngine;

public class SoundTestScript : MonoBehaviour
{
    public AK.Wwise.Event PlayGridCaptureEvent = null;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("GridCaptureSound, B pressed");
            PlayGridCaptureEvent.Post(gameObject);
        }

    }
}
