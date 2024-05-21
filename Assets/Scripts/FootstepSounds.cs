using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class TileCollider : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string soundFX;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] groundClips;
    [SerializeField] private AudioClip[] pathClips;
    private PlayerController player;
    private float timerlength = 0.3f;
    private float timer = 0f;
    private string oldTag;
    private int groundLength;
    private int pathLength;
    private float soundFXVolume;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerController>();
        groundLength = groundClips.Length;
        pathLength = pathClips.Length;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (!player.isMoving() && audioSource.isPlaying)
        {
            audioSource.Stop();
            timer = 0f;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (oldTag != other.tag)
        {
            audioSource.Stop();
        }
        if (other.CompareTag("Ground"))
        {
            if(timer < 0 || oldTag == "Path")
            {
                audioMixer.GetFloat(soundFX, out soundFXVolume);
                float volume = Mathf.Pow(10f, soundFXVolume / 20f);

                audioSource.PlayOneShot(groundClips[Random.Range(0, groundLength)], volume);
                timer = timerlength;
                oldTag = "Ground";
            }
        } 
        else if (other.CompareTag("Path"))
        {
            if (timer < 0 || oldTag == "Ground")
            {
                audioMixer.GetFloat(soundFX, out soundFXVolume);
                float volume = Mathf.Pow(10f, soundFXVolume / 20f);

                audioSource.PlayOneShot(pathClips[Random.Range(0, pathLength)], volume);
                timer = timerlength;
                oldTag = "Path";
            }
        }
    }
}
