using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This static gamemanager calss contains the mechanism which counts how many players are in our scene. When the Minimum players criteria has been met
// The class calls a function on the "wave" script which starts the wave movement after a delay

public class GameManager : MonoBehaviour

{

    [SerializeField]
    private wave wave;

   

    static public GameManager instance;

    
  
    void Awake()
    {
        instance = this;
    }

    

    // Make sure this is set to "1" if you are testing with a single player -----I have tried to make this acessible in the editor ...but I failed.
    public const int minimumPlayers = 2;

    // fetches the playerID from the playerscript
    public static PlayerScript GetPlayer(string _playerID)
    {
        return players[_playerID];
    }

    private const string PLAYER_ID_PREFIX = "player";

    // this stores the player ID in a dictionary, then checks the number of entries in the dictionary against the minimum required players

    private static Dictionary<string, PlayerScript> players = new Dictionary<string, PlayerScript>();

    public static void RegisterPlayer(string _netID, PlayerScript _player)
    {

        string _playerID = PLAYER_ID_PREFIX + _netID;
        players.Add(_playerID, _player);
        _player.transform.name = _playerID;
        if (players.Keys.Count == minimumPlayers)
        {
      
            BeginTheWave();
            
        }

    }


    public static void BeginTheWave()
    {
        
        Debug.Log("void beginthewave called to start coroutine in gamemanager");
        instance.StartCoroutine(instance.DelayWave(4));
    }

    // " this Coroutine isnt  started because the the game object 'GameManager' is inactive (seems to only be inactive for the server and host client)"
    IEnumerator DelayWave(int time)
    {
        Debug.Log("Wave delay coroutine called in GameManager");
        yield return new WaitForSeconds(time);
        wave.StartWave();

    }



}
