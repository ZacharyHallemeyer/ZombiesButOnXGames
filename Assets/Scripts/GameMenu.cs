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
        Time.timeScale = 0;
        gameMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerMovement.enabled = false;
        playerShooting.enabled = false;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerMovement.enabled = true;
        playerShooting.enabled = true;
    }

    public void ExitToMainMenu()
    {
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
