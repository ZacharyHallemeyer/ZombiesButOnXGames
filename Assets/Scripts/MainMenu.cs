using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Post Processing
    public Slider hueShiftSlider;
    public Slider bloomSlider;

    public Slider musicSlider;
    public Slider soundEffectSlider;
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public virtual void SetSliderUI()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        soundEffectSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume");
        hueShiftSlider.value = PlayerPrefs.GetFloat("HueShift");
        bloomSlider.value = PlayerPrefs.GetFloat("BloomIntensity");
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
        Application.Quit();
    }

    public virtual void SetMusicVolumePreference(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        if(audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();
        audioManager.SetMusicVolume();
    }   
    
    public virtual void SetSoundEffectsPreference(float volume)
    {
        PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
        if (audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();
        audioManager.SetSoundEffectVolume();
    }

    public virtual void SetNewHueShift(float value)
    {
        PlayerPrefs.SetFloat("HueShift", value);
    }

    public virtual void SetNewBloom(float value)
    {
        PlayerPrefs.SetFloat("BloomIntensity", value);
    }
}
