using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public AudioSource mAudioSource;
    public AudioClip joinAudioClip;
    public AudioClip multiPlayerAudioClip;

    // Start is called before the first frame update
    void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
        joinAudioClip = Resources.Load<AudioClip>("Sound/Scifi_Join");
        multiPlayerAudioClip = Resources.Load<AudioClip>("Sound/Scifi_Multi");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickSinglePlayer()
    {
        //Debug.Log("Loading singleplayer game");
        StartCoroutine(PlayAudioThenLoadScene("SinglePlayer", joinAudioClip));
    }

    public void OnClickMultiPlayer()
    {
        //Debug.Log("Loading multiplayer game");
        StartCoroutine(PlayAudioThenLoadScene("Multiplayer_Launcher", multiPlayerAudioClip));
    }

    IEnumerator PlayAudioThenLoadScene(string sceneName, AudioClip audioClip)
    {
        mAudioSource.PlayOneShot(audioClip);
        yield return new WaitForSeconds(audioClip.length); // Waits until end of clip to change scene
        SceneManager.LoadScene(sceneName);
    }

}
