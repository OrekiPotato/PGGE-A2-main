using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public InputField roomInputField;
    public GameObject lobbyPanel;

    public RoomItem roomItemPrefab;
    List<RoomItem> roomItemsList = new List<RoomItem>();
    public Transform contentObject;

    public float updateDelay = 1.5f;
    float nextUpdateTime;

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings(); // Connect to Photon server
            PhotonNetwork.AutomaticallySyncScene = true; // Automatically sync the scene when players join

            // Set a callback to be executed once connected to the master server
            PhotonNetwork.AddCallbackTarget(this);
        }
        else
        {
            // If already connected, directly join the lobby
            OnConnectedToMaster();
        }
    }

    private void CreateDummyRooms()
    {
        // Create dummy rooms and instantiate RoomItems
        for (int i = 1; i <= 5; i++)
        {
            string dummyRoomName = "DummyRoom" + i;
            RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 3 };
            PhotonNetwork.CreateRoom(dummyRoomName, roomOptions);

            // Instantiate RoomItem for each dummy room
            RoomItem newRoomItem = Instantiate(roomItemPrefab, contentObject);
            newRoomItem.SetRoomName(dummyRoomName);
            roomItemsList.Add(newRoomItem);
        }
    }

    public void OnClickCreate()
    {
        if (roomInputField.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions(){ MaxPlayers = 3});
        }
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        PhotonNetwork.LoadLevel("MultiplayerMap00");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateRoomList(roomList);
    }

    void UpdateRoomList(List<RoomInfo> list)
    {


        // Clears the list of any items
        foreach (RoomItem item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();


        // Populates the list of rooms in the Lobby
        foreach (RoomInfo room in list)
        {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItemsList.Add(newRoom);
        }
    }

    public override void OnJoinedLobby()
    {
        // Called when successfully joined the lobby
        CreateDummyRooms();
    }


    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnConnectedToMaster()
    {
        // Called when successfully connected to Photon master server
        PhotonNetwork.JoinLobby();

        // Remove the callback target as it's no longer needed
        PhotonNetwork.RemoveCallbackTarget(this);
    }

}
