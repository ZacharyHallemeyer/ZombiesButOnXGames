using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    // Post Processing
    public Slider hueShiftSlider;
    public Slider bloomSlider;

    public Slider musicSlider;
    public Slider soundEffectSlider;
    private AudioManager audioManager;

    public GameObject mainMenuFirstButton, difficultyFirstButton, modeFirstButton, optionsFirstButton;
    public GameObject aboutFirstButton, statsDifficultyFirstButton, statsFirstButton;
    public GameObject controlsGamepadFirstButton, controlsKeyboardFirstButton, controllerSelectionFirstButton;


    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        MoveToMainMenu();
        // Set player prefs controls to default if player pref controls are null
        if (PlayerPrefs.GetString("CrouchKeyboard") == "")
            SetPlayerPrefControls();
    }

    /// <summary>
    /// Sets option sliders to player prefs values
    /// </summary>
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
        MoveToMode();
    }

    public void SetHardMode()
    {
        PlayerPrefs.SetString("Difficulty", "Hard");
        MoveToMode();
    }

    public void SetInsaneMode()
    {
        PlayerPrefs.SetString("Difficulty", "Insane");
        MoveToMode();
    }


    public void PlayWaveMode()
    {
        SceneManager.LoadScene("WaveMode");
    }

    public void PlaySurvivalMode()
    {
        SceneManager.LoadScene("SurvivalMode");
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    // NAVAGATION ================================================
    public void MoveToMainMenu()
    {
        SetCurrentEvent(null);
        SetCurrentEvent(mainMenuFirstButton);
    }

    public void MoveToDifficulty()
    {
        SetCurrentEvent(null);
        SetCurrentEvent(difficultyFirstButton);
    }

    public void MoveToMode()
    {
        SetCurrentEvent(null);
        SetCurrentEvent(modeFirstButton);
    }

    public void MoveToOptions()
    {
        SetCurrentEvent(null);
        SetCurrentEvent(optionsFirstButton);
    }

    public void MoveToStatsDifficulty()
    {
        SetCurrentEvent(null);
        SetCurrentEvent(statsDifficultyFirstButton);
    }

    public void MoveToStats()
    {
        SetCurrentEvent(null);
        SetCurrentEvent(statsFirstButton);
    }

    public void MoveToControlsGamepad()
    {
        SetCurrentEvent(null);
        SetCurrentEvent(controlsGamepadFirstButton);
    }
    public void MoveToControlsKeyboard()
    {
        SetCurrentEvent(null);
        SetCurrentEvent(controlsKeyboardFirstButton);
    }

    public void MoveToControlsSelection()
    {
        SetCurrentEvent(null);
        SetCurrentEvent(controllerSelectionFirstButton);
    }

    public void MoveToAbout()
    {
        SetCurrentEvent(null);
        SetCurrentEvent(aboutFirstButton);
    }

    public void SetCurrentEvent(GameObject currentObject)
    {
        EventSystem.current.SetSelectedGameObject(currentObject);
    }

    // End of Navagation ==========================================

    // OPTIONS ====================================================

    /// <summary>
    /// Sets player prefs and audio volume in regard to general volume
    /// </summary>
    public virtual void SetMusicVolumePreference(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        if(audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();
        audioManager.SetMusicVolume();
    }   
    
    /// <summary>
    /// Sets player prefs and audio volume in regard to general volume
    /// </summary>
    public virtual void SetSoundEffectsPreference(float volume)
    {
        PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
        if (audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();
        audioManager.SetSoundEffectVolume();
    }

    /// <summary>
    /// Sets player prefs in regard to hue only
    /// </summary>
    public virtual void SetNewHueShift(float value)
    {
        PlayerPrefs.SetFloat("HueShift", value);
    }

    /// <summary>
    /// Sets player prefs in regard to bloom only
    /// </summary>
    public virtual void SetNewBloom(float value)
    {
        PlayerPrefs.SetFloat("BloomIntensity", value);
    }

    /// <summary>
    /// Set Player controls to default. This function can be used as a reset for controls
    /// </summary>
    public void SetPlayerPrefControls()
    {
        InputMaster inputMaster = new InputMaster();

        PlayerPrefs.SetString("CrouchKeyboard", GetInputString(inputMaster.Player.Crouch.bindings[0].ToString()));
        PlayerPrefs.SetString("ShootKeyboard", GetInputString(inputMaster.Player.Shoot.bindings[0].ToString()));
        PlayerPrefs.SetString("GrappleKeyboard", GetInputString(inputMaster.Player.Grapple.bindings[0].ToString()));
        PlayerPrefs.SetString("SwitchWeaponKeyboard", GetInputString(inputMaster.Player.SwitchWeaponButton.bindings[0].ToString()));
        PlayerPrefs.SetString("ReloadKeyboard", GetInputString(inputMaster.Player.Reload.bindings[0].ToString()));
        PlayerPrefs.SetString("InteractKeyboard", GetInputString(inputMaster.Player.Interact.bindings[0].ToString()));
        PlayerPrefs.SetString("GrenadeKeyboard", GetInputString(inputMaster.Player.Grenade.bindings[0].ToString()));
        PlayerPrefs.SetString("SpecialKeyboard", GetInputString(inputMaster.Player.Special.bindings[0].ToString()));
        PlayerPrefs.SetString("EscapeKeyboard", GetInputString(inputMaster.Player.Escape.bindings[0].ToString()));
        PlayerPrefs.SetString("ADSKeyboard", GetInputString(inputMaster.Player.ADS.bindings[0].ToString()));

        PlayerPrefs.SetString("CrouchGamepad", GetInputString(inputMaster.Player.Crouch.bindings[1].ToString()));
        PlayerPrefs.SetString("ShootGamepad", GetInputString(inputMaster.Player.Shoot.bindings[1].ToString()));
        PlayerPrefs.SetString("GrappleGamepad", GetInputString(inputMaster.Player.Grapple.bindings[1].ToString()));
        PlayerPrefs.SetString("SwitchWeaponGamepad", GetInputString(inputMaster.Player.SwitchWeaponButton.bindings[1].ToString()));
        PlayerPrefs.SetString("ReloadGamepad", GetInputString(inputMaster.Player.Reload.bindings[1].ToString()));
        PlayerPrefs.SetString("InteractGamepad", GetInputString(inputMaster.Player.Interact.bindings[1].ToString()));
        PlayerPrefs.SetString("GrenadeGamepad", GetInputString(inputMaster.Player.Grenade.bindings[1].ToString()));
        PlayerPrefs.SetString("SpecialGamepad", GetInputString(inputMaster.Player.Special.bindings[1].ToString()));
        PlayerPrefs.SetString("EscapeGamepad", GetInputString(inputMaster.Player.Escape.bindings[1].ToString()));
        PlayerPrefs.SetString("ADSGamepad", GetInputString(inputMaster.Player.ADS.bindings[1].ToString()));
        
        PlayerPrefs.SetFloat("Sens", 1f);
        PlayerPrefs.SetFloat("ADSSens", 1f);

        if(FindObjectOfType<PlayerShooting>() != null)
            FindObjectOfType<PlayerShooting>().RebindContols();
        if(FindObjectOfType<PlayerMovement>() != null)
            FindObjectOfType<PlayerMovement>().RebindContols();
    }

    /// <summary>
    /// returns a string that unity input system can read as a path. 
    /// Only works on strings that results from this kind of value:
    /// inputMaster.Player.ADS.bindings[1].ToString()
    /// </summary>
    private string GetInputString(string str)
    {
        return str.Substring(str.IndexOf('<'), str.IndexOf('[') - str.IndexOf('<'));
    }
}
