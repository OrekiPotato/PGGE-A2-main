using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioClip[] footstepsClips;
    public LayerMask groundLayer;

    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayFootstepClipEvent() // Used in animation events to sync footstep in animation with clips.
    {
        CheckGround();
    }

    private void CheckGround()
    {
        // Create a downwards ray to detect layer the player object is standing on.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f, groundLayer)) // 
        {
            string groundTag = hit.collider.tag;

            PlayFootstepSound(groundTag);
        }
    }

    private void PlayFootstepSound(string tag)
    {
        // Randomise the pitch and volume.
        audioSource.pitch = Random.Range(1f, 2f);
        audioSource.volume = Random.Range(0.5f, 5f);

        if (footstepsClips.Length == 0)
        {
            Debug.LogError("No footsteps clips assigned");
            return;
        }

        // Creates a list for clips with the matching tag names (ie, Snow/Wood/Concrete etc.)
        List<AudioClip> tagClips = new List<AudioClip>();
        foreach (var sound in footstepsClips)
        {
            if (sound.name.ToLower().Contains(tag.ToLower()))
            {
                tagClips.Add(sound);
            }
        }

        // If no specific tag is found, use random clip
        if (tagClips.Count == 0)
        {
            Debug.LogWarning("No footstep sound found for tag: " + tag);
            return;
        }

        // Chooses a random clip within the list and plays it once.
        AudioClip footstepClip = tagClips[Random.Range(0,tagClips.Count)];
        audioSource.PlayOneShot(footstepClip);
    }

}
