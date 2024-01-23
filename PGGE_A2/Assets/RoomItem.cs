using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public Text roomName;

    public void SetRoomName(string _roomName)
    {
        roomName.text = _roomName;
    }

    public void OnClickItem()
    {
        // Get the LobbyManager from the scene
        LobbyManager manager = FindObjectOfType<LobbyManager>();

        // Call the JoinRoom method in the LobbyManager with the room name
        if (manager != null)
        {
            manager.JoinRoom(roomName.text);
        }
    }
}
