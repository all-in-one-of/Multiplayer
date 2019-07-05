using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

// Score script  stores a Temporary Blue and Red score, which will be reset to zero at the start of each wave.
// tiles claimed during the waves sweep will be counted toward this temporary score.

// the winner of the temporary round will score a "real" point 

// so at the start of the game both teams have zero Temporary score
// and Zero score in thier permanent score

public class NetworkScore : NetworkBehaviour {

    public TMP_Text RedScoreText;
    public TMP_Text BlueScoreText;

    public GameObject gameOvertext;

    public static int startRedPermScore = 0;
    public static int startBluePermScore = 0;

    public static int startRedTempScore = 0;
    public static int startBlueTempScore = 0;

    // initializes scoreboard values to show zero
    private void Start()
    {
        ResetTempPoints();
        OnChangeRedPermananetScore(0);
        OnChangeBluePermanentScore(0);
    }

    public void ShowGameOvertext()
    {
        gameOvertext.SetActive(true);
    }

    //----------------------------------------------------------------PERMANENT POINTS + SCOREBOARD-----------------------------------------------------------

    // nethmi permanent
    [SyncVar(hook = "OnChangeRedPermananetScore")]
    public int RedPermanentScore = startRedPermScore;

    // arkryas permanent
    [SyncVar(hook = "OnChangeBluePermanentScore")]
    public int BluePermanentScore = startRedPermScore;

    void OnChangeRedPermananetScore(int RedScore)
    {
        RedPermanentScore = RedScore;
        RedScoreText.text = "RED:" + RedScore.ToString();
    }

    void OnChangeBluePermanentScore(int BlueScore)
    {
        BluePermanentScore = BlueScore;
        BlueScoreText.text = "BLUE:" + BlueScore.ToString();
    }

   
    public void ScorePermanentPoints(string team)
    {
        switch (team)
        {
            case "Nethmi":
                if (!isServer)
                {
                    return;
                    // clients get out of this function here
                }
                RedPermanentScore++;
                break;
            case "Arkryas":
                if (!isServer)
                {
                    return;
                }
                BluePermanentScore++;
                break;
            default:
                break;
        }
    }

    //------------------------------------------------------------TEMPORARY POINTS-----------------------------------------------------


    // temporary points should be scored on the server and updated to all clients - no use performing these actions only on server client only 

    // the "ScoreTempPoints" function is called as a COMMAND (called by client, runs on server) from the TileController script.
    //The "ScoreTempPoints" function changes the syncVARS , which are then broadcast out to all instances of the game.

    [SyncVar]
    public int currentRedTempScore;

    [SyncVar]
    public int currentBlueTempScore;

   
    public void ScoreTempPoints(string team, int amount)
    {
        Debug.Log("Sever is scoring " + amount + " temp point to " + team + " team");
        if (team == "blue") 
            currentBlueTempScore += amount;
        if(team == "red")
            currentRedTempScore += amount;
    }

 
    public void ResetTempPoints()
    {
        currentRedTempScore = startRedTempScore;
        currentBlueTempScore = startBlueTempScore;
    }

    void OnChangeRedTempScore (int currentRedTempScore)
    {
        Debug.Log("Red NETHMI temp Team score is " + currentRedTempScore);
    }

    void OnChangeBlueTempScore(int currentBlueTempScore)
    {
         Debug.Log("Blue AKYRAS temp Team score is " + currentBlueTempScore);
    }

}
