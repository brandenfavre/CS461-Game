using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider soundFXVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private string master;
    [SerializeField] private string soundFX;
    [SerializeField] private string music;

    private void Start()
    {
        // Get correct UI slider positions by getting the volume from audioMixer
        float masterVolume;
        audioMixer.GetFloat(master, out masterVolume);
        masterVolumeSlider.value = Mathf.Pow(10f, masterVolume / 20f);

        float soundFXVolume;
        audioMixer.GetFloat(soundFX, out soundFXVolume);
        soundFXVolumeSlider.value = Mathf.Pow(10f, soundFXVolume / 20f);

        float musicVolume;
        audioMixer.GetFloat(music, out musicVolume);
        musicVolumeSlider.value = Mathf.Pow(10f, musicVolume / 20f);
    }
    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat(master, Mathf.Log10(level) * 20f);
    }

    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat(soundFX, Mathf.Log10(level) * 20f);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat(music, Mathf.Log10(level) * 20f);
    }

}
