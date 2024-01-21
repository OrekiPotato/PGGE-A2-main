using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Back_Btn : MonoBehaviour
{
    public AudioSource mAudioSource;
    public AudioClip backAudioClip;

    // Start is called before the first frame update
    void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
        backAudioClip = Resources.Load<AudioClip>("Sound/Scifi_Back");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickBackBtn() 
    {
        Debug.Log("Going Back...");
        StartCoroutine(PlayAudioThenLoadScene("Menu", backAudioClip));
    }

    IEnumerator PlayAudioThenLoadScene(string sceneName, AudioClip audioClip)
    {
        mAudioSource.PlayOneShot(audioClip);
        yield return new WaitForSeconds(audioClip.length); // Waits until end of clip to change scene
        SceneManager.LoadScene(sceneName);
    }
}
