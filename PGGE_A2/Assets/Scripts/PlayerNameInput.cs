using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class PlayerNameInput : MonoBehaviour
{
    private InputField mInputField;
    const string playerNamePrefKey = "PlayerName";

    // Start is called before the first frame update
    void Start()
    {
        // Refactoring 3 (simplification)
        mInputField = GetComponent<InputField>();

        string defaultName = string.Empty;

        // Refactoring 1
        // Moved "PlayerPrefs.HasKey(playerNamePrefKey)" into main conditional statement for easy readability.
        if (mInputField != null && PlayerPrefs.HasKey(playerNamePrefKey)) 
        {
            defaultName = PlayerPrefs.GetString(playerNamePrefKey);
            mInputField.text = defaultName;
        }
        PhotonNetwork.NickName = defaultName;
    }

    public void SetPlayerName()
    {
        string value = mInputField.text;
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player Name is null or empty");
            return;
        }
        PhotonNetwork.NickName = value;
        PlayerPrefs.SetString(playerNamePrefKey, value);

        Debug.Log("Nickname entered: " + value);
    }

}
