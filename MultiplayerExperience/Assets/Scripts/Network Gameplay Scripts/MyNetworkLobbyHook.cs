using System.Collections;
using System.Collections.Generic;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;

// this script communicates between the LobbyManager (Lobby) and the Networkmanager (game)

public class MyNetworkLobbyHook : LobbyHook {


    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        SetupLocalPlayer localPlayer = gamePlayer.GetComponent<SetupLocalPlayer>();

        // variables below set from Player Info script in the lobby 

        
        localPlayer.playerColor = lobby.playerColor;

    }


}
