using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    [SerializeField] private bool isMainMenu = false;

    public GameObject MenuUI;
    public GameObject SoundMenuUI;
    private bool inSoundMenu = false;

    [SerializeField] private string mainMenuScene;
    [SerializeField] private string firstLevel;


    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] string master;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonPressClip;
    [SerializeField] private AudioClip menuPressClip;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            float masterVolume;
            audioMixer.GetFloat(master, out masterVolume);
            float volume = Mathf.Pow(10f, masterVolume / 20f);
            audioSource.PlayOneShot(menuPressClip, volume);
            if (inSoundMenu)
                ExitSoundMenu();
            else if (isMainMenu)
                return;
            else if (gameIsPaused)
                Resume();
            else
                Pause();
        }
    }
    public void ButtonPressSound()
    {
        float masterVolume;
        audioMixer.GetFloat(master, out masterVolume);
        float volume = Mathf.Pow(10f, masterVolume / 20f);
        audioSource.PlayOneShot(buttonPressClip, volume);
    }
    public void Resume()
    {
        MenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }
    void Pause()
    {
        MenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        gameIsPaused = false;
        SceneManager.LoadScene(firstLevel);
    }

    public void LoadSoundMenu()
    {
        MenuUI.SetActive(false);
        SoundMenuUI.SetActive(true);
        inSoundMenu = true;
    }

    public void ExitSoundMenu()
    {
        SoundMenuUI.SetActive(false);
        MenuUI.SetActive(true);
        inSoundMenu = false;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Only for unityeditor closing since Application.Quit() doesn't work in editor
    #endif
    }
}
