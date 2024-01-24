using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public Text roomName;
    public Text playerCountText;
    public Button roomButton;

    public void SetRoomName(string _roomName)
    {
        roomName.text = _roomName;
    }

    public void SetButtonColor(Color color)
    {
        Image buttonImage = roomButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = color;
        }
    }

    public void UpdatePlayerCount(int playerCount, int maxPlayers)
    {
        playerCountText.text = playerCount + "/" + maxPlayers;

        // Uses the amount of players to interpolate the color values (from white to red)
        Color buttonColor = Color.Lerp(Color.white, Color.red, (float)playerCount / maxPlayers);
        SetButtonColor(buttonColor);
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
