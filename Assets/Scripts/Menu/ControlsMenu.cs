using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles Rebinding controls and display said controls
/// </summary>
public class ControlsMenu : MonoBehaviour
{
    /// ============================================================================= ///
    /// Keyboard index: 0
    /// Gamepad index: 1
    /// ============================================================================= ///

    private InputMaster inputMaster;
    public TextMeshProUGUI crouchKeyboardText, grappleKeyboardText, switchKeyboardText, reloadKeyboardText, 
                           shootKeyboardText, interactKeyboardText, moveKeyboardText, grenadeKeyboardText, 
                           specialKeyboardText, adsKeyboardText;

    public TextMeshProUGUI crouchGamepadText, grappleGamepadText, switchGamepadText, reloadGamepadText, shootGamepadText,
                       interactGamepadText, moveGamepadText, grenadeGamepadText, specialGamepadText, adsGamepadText;

    public Slider keyboardSens, keyboardADSSens, gamepadSens, gamepadADSSens;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private bool rebindInPogress = false;


    /// <summary>
    /// Sets the values of control keyboard menu with the current keyboard controls 
    /// Dependencies: SetControlSliders
    /// </summary>
    public void SetControlMenuKeyboard()
    {
        SetControls setControls = gameObject.AddComponent<SetControls>();
        inputMaster = new InputMaster();
        inputMaster = setControls.SetPlayerControls(inputMaster);

        shootKeyboardText.text = "SHOOT: " + inputMaster.Player.Shoot.bindings[0].ToDisplayString();
        crouchKeyboardText.text = "CROUCH: " + inputMaster.Player.Crouch.bindings[0].ToDisplayString();
        grappleKeyboardText.text = "GRAPPLE: " + inputMaster.Player.Grapple.bindings[0].ToDisplayString();
        switchKeyboardText.text = "SWITCH: " +
                            inputMaster.Player.SwitchWeaponButton.bindings[0].ToDisplayString()
                            + " OR " +
                            inputMaster.Player.SwitchWeaponMouseWheel.bindings[0].ToDisplayString();
        reloadKeyboardText.text = "RELOAD: " + inputMaster.Player.Reload.bindings[0].ToDisplayString();
        interactKeyboardText.text = "INTERACT: " + inputMaster.Player.Interact.bindings[0].ToDisplayString();
        moveKeyboardText.text = "MOVE: " +
                            inputMaster.Player.Movement.bindings[1].ToDisplayString() +
                            inputMaster.Player.Movement.bindings[2].ToDisplayString() +
                            inputMaster.Player.Movement.bindings[3].ToDisplayString() +
                            inputMaster.Player.Movement.bindings[4].ToDisplayString();
        grenadeKeyboardText.text = "GRENADE: " + inputMaster.Player.Grenade.bindings[0].ToDisplayString();
        specialKeyboardText.text = "SPECIAL: " + inputMaster.Player.Special.bindings[0].ToDisplayString();
        adsKeyboardText.text = "ADS: " + inputMaster.Player.ADS.bindings[0].ToDisplayString();

        SetControlSliders();
    }

    /// <summary>
    /// Sets the values of control gamepad menu with the current gamepad controls 
    /// Dependencies: SetControlSliders
    /// </summary>
    public void SetControlMenuGamepad()
    {
        SetControls setControls = gameObject.AddComponent<SetControls>();
        inputMaster = new InputMaster();
        inputMaster = setControls.SetPlayerControls(inputMaster);

        shootGamepadText.text = "SHOOT: " + inputMaster.Player.Shoot.bindings[1].ToDisplayString();
        crouchGamepadText.text = "CROUCH: " + inputMaster.Player.Crouch.bindings[1].ToDisplayString();
        grappleGamepadText.text = "GRAPPLE: " + inputMaster.Player.Grapple.bindings[1].ToDisplayString();
        switchGamepadText.text = "SWITCH WEAPON: " + inputMaster.Player.SwitchWeaponButton.bindings[1].ToDisplayString();
        reloadGamepadText.text = "RELOAD: " + inputMaster.Player.Reload.bindings[1].ToDisplayString();
        interactGamepadText.text = "INTERACT: " + inputMaster.Player.Interact.bindings[1].ToDisplayString();
        moveGamepadText.text = "MOVE: LEFT STICK";
        grenadeGamepadText.text = "GRENADE: " + inputMaster.Player.Grenade.bindings[1].ToDisplayString();
        specialGamepadText.text = "SPECIAL: " + inputMaster.Player.Special.bindings[1].ToDisplayString();
        adsGamepadText.text = "ADS: " + inputMaster.Player.ADS.bindings[1].ToDisplayString();

        SetControlSliders();
    }

    // KEYBOARD AND MOUSE ======================================================================

    /// <summary>
    /// Sets new binding
    /// </summary>
    public void SetNewShootKeyboard()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Shoot.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindShootKeyboard())
                            .Start();
    }

    /// <summary>
    /// Finishes new binding
    /// </summary>
    public void OnRebindShootKeyboard()
    {
        shootKeyboardText.text = "SHOOT: " + inputMaster.Player.Shoot.bindings[0].ToDisplayString();
        PlayerPrefs.SetString("ShootKeyboard", GetInputString(inputMaster.Player.Shoot.bindings[0].ToString()));
        OnRebindComplete();
    }

    /// <summary>
    /// Sets new binding
    /// </summary>
    public void SetNewCrouchKeyboard()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Crouch.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindCrouchKeyboard())
                            .Start();
    }

    /// <summary>
    /// Finishes new binding
    /// </summary>
    public void OnRebindCrouchKeyboard()
    {
        crouchKeyboardText.text = "CROUCH: " +
            inputMaster.Player.Crouch.bindings[0].ToDisplayString();
        PlayerPrefs.SetString("CrouchKeyboard", GetInputString(inputMaster.Player.Crouch.bindings[0].ToString()));
        OnRebindComplete();
    }

    /// <summary>
    /// Sets new binding
    /// </summary>
    public void SetNewGrappleKeyboard()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Grapple.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindGrappleKeyboard())
                            .Start();
    }

    /// <summary>
    /// Finishes new binding
    /// </summary>
    public void OnRebindGrappleKeyboard()
    {
        grappleKeyboardText.text = "GRAPPLE: " +
            inputMaster.Player.Grapple.bindings[0].ToDisplayString();
        PlayerPrefs.SetString("GrappleKeyboard", GetInputString(inputMaster.Player.Grapple.bindings[0].ToString()));
        OnRebindComplete();
    }

    /// <summary>
    /// Sets new binding
    /// </summary>
    public void SetNewSwitchWeaponKeyboard()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.SwitchWeaponButton.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindWeaponKeyboard())
                            .Start();
    }

    /// <summary>
    /// Finishes new binding
    /// </summary>
    public void OnRebindWeaponKeyboard()
    {
        switchKeyboardText.text = "SWITCH: " +
                inputMaster.Player.SwitchWeaponButton.bindings[0].ToDisplayString()
                + " OR " +
                inputMaster.Player.SwitchWeaponMouseWheel.bindings[0].ToDisplayString();
        PlayerPrefs.SetString("SwitchWeaponKeyboard", GetInputString(inputMaster.Player.SwitchWeaponButton.bindings[0].ToString()));
        OnRebindComplete();
    }

    /// <summary>
    /// Sets new binding
    /// </summary>
    public void SetNewReloadKeyboard()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Reload.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindReloadKeyboard())
                            .Start();
    }

    /// <summary>
    /// Finishes new binding
    /// </summary>
    public void OnRebindReloadKeyboard()
    {
        reloadKeyboardText.text = "RELOAD: " +
               inputMaster.Player.Reload.bindings[0].ToDisplayString();
        PlayerPrefs.SetString("ReloadKeyboard", GetInputString(inputMaster.Player.Reload.bindings[0].ToString()));
        OnRebindComplete();
    }

    /// <summary>
    /// Sets new binding
    /// </summary>
    public void SetNewInteractKeyboard()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Interact.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindInteractKeyboard())
                            .Start();
    }

    /// <summary>
    /// Finishes new binding
    /// </summary>
    public void OnRebindInteractKeyboard()
    {
        interactKeyboardText.text = "INTERACT: " +
               inputMaster.Player.Interact.bindings[0].ToDisplayString();
        PlayerPrefs.SetString("InteractKeyboard", GetInputString(inputMaster.Player.Interact.bindings[0].ToString()));
        OnRebindComplete();
    }

    /// <summary>
    /// Sets new binding
    /// </summary>
    public void SetNewGrenadeKeyboard()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation =inputMaster.Player.Grenade.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindGrappleKeyboard())
                            .Start();
    }

    /// <summary>
    /// Finishes new binding
    /// </summary>
    public void OnRebindGrenadeKeyboard()
    {
        grenadeKeyboardText.text = "GRENADE: " +
               inputMaster.Player.Grenade.bindings[0].ToDisplayString();
        PlayerPrefs.SetString("GrenadeKeyboard", GetInputString(inputMaster.Player.Grenade.bindings[0].ToString()));
        OnRebindComplete();
    }

    /// <summary>
    /// Sets new binding
    /// </summary>
    public void SetNewSpecialKeyboard()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Special.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindSpecialKeyboard())
                            .Start();
    }

    /// <summary>
    /// Finishes new binding
    /// </summary>
    public void OnRebindSpecialKeyboard()
    {
        specialKeyboardText.text = "SPECIAL: " +
               inputMaster.Player.Special.bindings[0].ToDisplayString();
        PlayerPrefs.SetString("SpecialKeyboard", GetInputString(inputMaster.Player.Special.bindings[0].ToString()));
        OnRebindComplete();
    }

    /// <summary>
    /// Sets new binding
    /// </summary>
    public void SetNewADSKeyboard()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Special.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindADSKeyboard())
                            .Start();
    }

    /// <summary>
    /// Finishes new binding
    /// </summary>
    public void OnRebindADSKeyboard()
    {
        adsKeyboardText.text = "ADS: " +
               inputMaster.Player.ADS.bindings[0].ToDisplayString();
        PlayerPrefs.SetString("ADSKeyboard", GetInputString(inputMaster.Player.ADS.bindings[0].ToString()));
        OnRebindComplete();
    }

    // END OF KEYBOARD AND MOUSE =================================================================

    // GAMEPAD ===================================================================================

    /// <summary>
    /// Sets new binding
    /// </summary>
    public void SetNewShootGamepad()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Shoot.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindShootGamepad())
                            .Start();
    }

    /// <summary>
    /// Finishes new binding
    /// </summary>
    public void OnRebindShootGamepad()
    {
        shootGamepadText.text = "SHOOT: " + inputMaster.Player.Shoot.bindings[1].ToDisplayString();
        PlayerPrefs.SetString("ShootGamepad", GetInputString(inputMaster.Player.Shoot.bindings[1].ToString()));
        OnRebindComplete();
    }

    /// <summary>
    /// Sets new binding
    /// </summary>
    public void SetNewCrouchGamepad()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Crouch.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindCrouchGamepad())
                            .Start();
    }

    /// <summary>
    /// Finishes new binding
    /// </summary>
    public void OnRebindCrouchGamepad()
    {
        crouchGamepadText.text = "CROUCH: " +
            inputMaster.Player.Crouch.bindings[1].ToDisplayString();
        PlayerPrefs.SetString("CrouchGamepad", GetInputString(inputMaster.Player.Crouch.bindings[1].ToString()));
        OnRebindComplete();
    }

    /// <summary>
    /// Sets new binding
    /// </summary>
    public void SetNewGrappleGamepad()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Grapple.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindGrappleGamepad())
                            .Start();
    }

    /// <summary>
    /// Finishes new binding
    /// </summary>
    public void OnRebindGrappleGamepad()
    {
        grappleGamepadText.text = "GRAPPLE: " +
            inputMaster.Player.Grapple.bindings[1].ToDisplayString();
        PlayerPrefs.SetString("GrappleGamepad", GetInputString(inputMaster.Player.Grapple.bindings[1].ToString()));
        OnRebindComplete();
    }

    /// <summary>
    /// Sets new binding
    /// </summary>
    public void SetNewSwitchWeaponGamepad()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.SwitchWeaponButton.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindWeaponGamepad())
                            .Start();
    }

    /// <summary>
    /// Finishes new binding
    /// </summary>
    public void OnRebindWeaponGamepad()
    {
        switchGamepadText.text = "SWITCH: " +
               inputMaster.Player.SwitchWeaponButton.bindings[1].ToDisplayString();
        PlayerPrefs.SetString("SwitchWeaponGamepad", GetInputString(inputMaster.Player.SwitchWeaponButton.bindings[1].ToString()));
        OnRebindComplete();
    }

    public void SetNewReloadGamepad()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Reload.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindReloadGamepad())
                            .Start();
    }

    /// <summary>
    /// Finishes new binding
    /// </summary>
    public void OnRebindReloadGamepad()
    {
        reloadGamepadText.text = "RELOAD: " +
               inputMaster.Player.Reload.bindings[1].ToDisplayString();
        PlayerPrefs.SetString("ReloadGamepad", GetInputString(inputMaster.Player.Reload.bindings[1].ToString()));
        OnRebindComplete();
    }

    /// <summary>
    /// Sets new binding
    /// </summary>
    public void SetNewInteractGamepad()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Interact.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindInteractGamepad())
                            .Start();
    }

    /// <summary>
    /// Finishes new binding
    /// </summary>
    public void OnRebindInteractGamepad()
    {
        interactGamepadText.text = "INTERACT: " +
               inputMaster.Player.Interact.bindings[1].ToDisplayString();
        PlayerPrefs.SetString("InteractGamepad", GetInputString(inputMaster.Player.Interact.bindings[1].ToString()));
        OnRebindComplete();
    }

    /// <summary>
    /// Sets new binding
    /// </summary>
    public void SetNewGrenadeGamepad()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Grenade.PerformInteractiveRebinding()
                            .WithControlsExcluding("Mouse")
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindGrenadeGamepad())
                            .Start();
    }

    /// <summary>
    /// Finishes new binding
    /// </summary>
    public void OnRebindGrenadeGamepad()
    {
        grenadeGamepadText.text = "GRENADE: " +
               inputMaster.Player.Grenade.bindings[1].ToDisplayString();
        PlayerPrefs.SetString("GrenadeGamepad", GetInputString(inputMaster.Player.Grenade.bindings[1].ToString()));
        OnRebindComplete();
    }

    /// <summary>
    /// Sets new binding
    /// </summary>
    public void SetNewSpecialGamepad()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Special.PerformInteractiveRebinding()
                            .WithControlsExcluding("Mouse")
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindSpecialGamepad())
                            .Start();
    }

    /// <summary>
    /// Finishes new binding
    /// </summary>
    public void OnRebindSpecialGamepad()
    {
        specialGamepadText.text = "SPECIAL: " +
               inputMaster.Player.Special.bindings[1].ToDisplayString();
        PlayerPrefs.SetString("SpecialGamepad", GetInputString(inputMaster.Player.Special.bindings[1].ToString()));
        OnRebindComplete();
    }

    /// <summary>
    /// Sets new binding
    /// </summary>
    public void SetNewADSGamepad()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Special.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindADSGamepad())
                            .Start();
    }

    /// <summary>
    /// Finishes new binding
    /// </summary>
    public void OnRebindADSGamepad()
    {
        adsGamepadText.text = "ADS: " +
               inputMaster.Player.ADS.bindings[1].ToDisplayString();
        PlayerPrefs.SetString("ADSGamepad", GetInputString(inputMaster.Player.ADS.bindings[1].ToString()));
        OnRebindComplete();
    }

    // END OF GAMEPAD ============================================================================

    /// <summary>
    /// returns a string that unity input system can read as a path. 
    /// Only works on strings that results from this kind of value:
    /// inputMaster.Player.ADS.bindings[1].ToString()
    /// </summary>
    private string GetInputString(string str)
    {
        return str.Substring(str.IndexOf('<'), str.IndexOf('[') - str.IndexOf('<'));
    }

    /// <summary>
    /// Rebinds controls and disposes of rebinding operation
    /// </summary>
    public void OnRebindComplete()
    {
        rebindingOperation.Dispose();
        if(FindObjectOfType<PlayerShooting>() != null)
            FindObjectOfType<PlayerShooting>().RebindContols();
        if(FindObjectOfType<PlayerMovement>() != null)
            FindObjectOfType<PlayerMovement>().RebindContols();
        if(FindObjectOfType<SurvivalPlayerMovement>() != null)
            FindObjectOfType<SurvivalPlayerMovement>().RebindContols();
        if(FindObjectOfType<SurvivalPlayerShoot>() != null)
            FindObjectOfType<SurvivalPlayerShoot>().RebindContols();
        rebindInPogress = false;
    }

    // SLIDERS ====================================================================================

    public void SetNewADSSens(float value)
    {
        keyboardADSSens.value = value;
        gamepadADSSens.value = value;
        PlayerPrefs.SetFloat("ADSSens", value);
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
            playerMovement.adsSensMultiplier = value;
    }

    public void SetNewSens(float value)
    {
        keyboardSens.value = value;
        gamepadSens.value = value;
        PlayerPrefs.SetFloat("Sens", value);
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
            playerMovement.sensMultiplier = value;
    }

    public void SetControlSliders()
    {
        keyboardADSSens.value = PlayerPrefs.GetFloat("ADSSens", 1f);
        gamepadADSSens.value = PlayerPrefs.GetFloat("ADSSens", 1f);
        keyboardSens.value = PlayerPrefs.GetFloat("Sens", 1f);
        gamepadSens.value = PlayerPrefs.GetFloat("Sens", 1f);
    }

    // END OF SLIDERS =============================================================================
}
