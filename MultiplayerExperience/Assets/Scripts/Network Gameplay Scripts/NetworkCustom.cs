using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworkCustom : NetworkManager
{
    /*
    
    public int chosenCharacter = 0; // it seems like this has to be set on each client before the network manager does anything 

    public GameObject[] characters;

    //subclass for sending network messages
    public class NetworkMessage : MessageBase
    {
        public int chosenClass; // this message is sent by the cleint to the server 
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        NetworkMessage message = extraMessageReader.ReadMessage<NetworkMessage>();
        int selectedClass = message.chosenClass; // the server reads the message from the client an uses the int to spawn the right player object
        Debug.Log("server add with message " + selectedClass);

        GameObject player;
        Transform startPos = GetStartPosition();

        if (startPos != null)
        {
            player = Instantiate(characters[selectedClass], startPos.position, startPos.rotation) as GameObject;
            chosenCharacter = 1;
        }
        else
        {
            player = Instantiate(characters[selectedClass], Vector3.zero, Quaternion.identity) as GameObject;
            chosenCharacter = 1;
        }

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

    }

    public override void OnClientConnect(NetworkConnection conn) // this happens first 'client side' and sends a message from client to the server
    {
        NetworkMessage test = new NetworkMessage();
        
        test.chosenClass = chosenCharacter; // this converts the chosencharacter 'int' into a message for the server

        ClientScene.AddPlayer(conn, 0, test);
        chosenCharacter = 1; // incrementing chosen character here isnt working 
    }


    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        //base.OnClientSceneChanged(conn);
    }

    public void btn1()
    {
        chosenCharacter = 0;
    }

    public void btn2()
    {
        chosenCharacter = 1;
    }
    */
}
