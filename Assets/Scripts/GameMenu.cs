using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class GameMenu : MainMenu
{
    public GameObject gameMenu;
    public PlayerMovement playerMovement;
    public PlayerShooting playerShooting;

    public Slider gameMenuMusicSlider;
    public Slider gameMenuSoundEffectSlider;

    // Post processing
    public Slider gameMenuHueShiftSlider;
    public Slider gameMenuBloomSlider;
    private PostProcessVolume postProcessingVolume;
    private Bloom bloomLayer = null;
    private ColorGrading colorGradingLayer = null;

    private void Start()
    {
        postProcessingVolume = FindObjectOfType<PostProcessVolume>();
        postProcessingVolume.profile.TryGetSettings(out bloomLayer);
        postProcessingVolume.profile.TryGetSettings(out colorGradingLayer);

        SetBloomBasedOnPlayerPref();
        SetHueShiftBasedOnPlayerPref();

        SetSliderUI();
    }

    public void PauseGame()
    {
        // Prevents automatic weapon sounds to continue while in game menu
        if(FindObjectOfType<PlayerShooting>() != null)
            FindObjectOfType<AudioManager>().Stop(FindObjectOfType<PlayerShooting>().currentGun.name);
        MoveToMainMenu();
        Time.timeScale = 0;
        gameMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Prevents bug where game menu has a wrong reference to these scripts
        if(FindObjectOfType<PlayerMovement>() != null)
            FindObjectOfType<PlayerMovement>().enabled = false;
        if(FindObjectOfType<PlayerShooting>() != null)
            FindObjectOfType<PlayerShooting>().enabled = false;

        if(FindObjectOfType<SurvivalPlayerMovement>() != null)
            FindObjectOfType<SurvivalPlayerMovement>().enabled = false;
        if(FindObjectOfType<SurvivalPlayerShoot>() != null)
            FindObjectOfType<SurvivalPlayerShoot>().enabled = false;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Prevents bug where game menu has a wrong reference to these scripts
        if (FindObjectOfType<PlayerMovement>() != null)
            FindObjectOfType<PlayerMovement>().enabled = true;
        if (FindObjectOfType<PlayerShooting>() != null)
            FindObjectOfType<PlayerShooting>().enabled = true;

        if (FindObjectOfType<SurvivalPlayerMovement>() != null)
            FindObjectOfType<SurvivalPlayerMovement>().enabled = true;
        if (FindObjectOfType<SurvivalPlayerShoot>() != null)
            FindObjectOfType<SurvivalPlayerShoot>().enabled = true;
    }

    public void ExitToMainMenu()
    {
        if (FindObjectOfType<PlayerStats>() != null)
            FindObjectOfType<PlayerStats>().SaveScore();
        if (FindObjectOfType<SurvivePlayerStats>() != null)
            FindObjectOfType<SurvivePlayerStats>().SaveScore();
        SceneManager.LoadScene("MainMenu");
        
    }

    public override void SetSliderUI()
    {
        gameMenuMusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", .75f);
        gameMenuSoundEffectSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume", .75f);
        gameMenuHueShiftSlider.value = PlayerPrefs.GetFloat("HueShift", 0);
        gameMenuBloomSlider.value = PlayerPrefs.GetFloat("BloomIntensity", 30);
    }


    public void SetHueShiftBasedOnPlayerPref()
    {
        if (colorGradingLayer == null) return;
        if (!colorGradingLayer.enabled.value)
            colorGradingLayer.enabled.value = true;
        colorGradingLayer.hueShift.value = PlayerPrefs.GetFloat("HueShift", 0);
    }

    public void SetBloomBasedOnPlayerPref()
    {
        if (bloomLayer == null) return;
        if (!bloomLayer.enabled.value)
            bloomLayer.enabled.value = true;
        bloomLayer.intensity.value = PlayerPrefs.GetFloat("BloomIntensity", 30);
    }

    public override void SetNewHueShift(float value)
    {
        if (colorGradingLayer == null) return;
        if (!colorGradingLayer.enabled.value)
            colorGradingLayer.enabled.value =  true;
        colorGradingLayer.hueShift.value = value;
        PlayerPrefs.SetFloat("HueShift", value);
    }

    public override void SetNewBloom(float value)
    {
        if (bloomLayer == null) return;
        if (!bloomLayer.enabled.value)
            bloomLayer.enabled.value = true;
        bloomLayer.intensity.value = value;
        PlayerPrefs.SetFloat("BloomIntensity", value);
    }

}
