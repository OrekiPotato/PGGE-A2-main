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

    bool connected = false;

    private void Start()
    {
        PhotonNetwork.JoinLobby();
    }

    //public override void OnJoinedLobby()
    //{
    //    if (connected)
    //    {
    //        StartCoroutine(CreateDumbRoomsAfterDelay());
    //    }
    //}

    //IEnumerator CreateDumbRoomsAfterDelay()
    //{
    //    yield return new WaitForSeconds(1.5f);
    //    StartCoroutine(CreateDumbRooms());
    //}

    //IEnumerator CreateDumbRooms()
    //{
    //    yield return new WaitForSeconds(1.0f);

    //    // Creates the 5 dummy rooms
    //    for (int i = 1; i <= 5; i++)
    //    {
    //        string dumbRoomName = "Dumb Room" + i;
    //        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 3 };
    //        PhotonNetwork.CreateRoom(dumbRoomName, roomOptions, TypedLobby.Default);

    //        // Instantiates the new dummy rooms into room list.
    //        RoomItem newDumbRoom = Instantiate(roomItemPrefab, contentObject);
    //        newDumbRoom.SetRoomName(dumbRoomName);
    //        roomItemsList.Add(newDumbRoom);
    //    }
    //}

    public override void OnJoinedLobby()
    {
        StartCoroutine(InitiallyCreatedRooms());
        connected = true;
    }


    IEnumerator InitiallyCreatedRooms()
    {
        yield return new WaitForSeconds(1.0f);
        if (connected)
        {
            // Wait for a delay before creating rooms
            yield return new WaitForSeconds(0.001f);

            for (int i = 1; i <= 5; i++)
            {

                string dumbRoomName = "Room " + i;
                RoomOptions dumbOptions = new RoomOptions() { MaxPlayers = 3 };
                //RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 3 };
                //PhotonNetwork.CreateRoom(dumbRoomName, roomOptions, TypedLobby.Default);

                // Instantiates the new dummy rooms into room list.
                RoomItem newDumbRoom = Instantiate(roomItemPrefab, contentObject);
                newDumbRoom.SetRoomName(dumbRoomName);
                newDumbRoom.UpdatePlayerCount(0, dumbOptions.MaxPlayers);
                //newDumbRoom.SetButtonColor(Color.red);
                roomItemsList.Add(newDumbRoom);
            }
        }
        
    }



    public void OnClickCreate()
    {
        if (roomInputField.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions() { MaxPlayers = 3 });
        }
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        PhotonNetwork.LoadLevel("MultiplayerMap00");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room list updated. Count: " + roomList.Count);
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
            newRoom.UpdatePlayerCount(room.PlayerCount, room.MaxPlayers); // Updates the player counter.
            roomItemsList.Add(newRoom);
        }

    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

}
