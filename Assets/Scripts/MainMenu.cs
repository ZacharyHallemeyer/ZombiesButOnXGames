using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Slider musicSlider;
    public Slider soundEffectSlider;
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public virtual void SetVolumeUI()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        soundEffectSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume");
    }

    public void SetEasyMode()
    {
        PlayerPrefs.SetString("Difficulty", "Easy");
        PlayGame();
    }

    public void SetHardMode()
    {
        PlayerPrefs.SetString("Difficulty", "Hard");
        PlayGame();
    }    
    
    public void SetInsaneMode()
    {
        PlayerPrefs.SetString("Difficulty", "Insane");
        PlayGame();
    }


    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    public virtual void SetMusicVolumePreference(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        audioManager.SetMusicVolume();
    }   
    
    public virtual void SetSoundEffectsPreference(float volume)
    {
        PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
        audioManager.SetSoundEffectVolume();
    }


}
