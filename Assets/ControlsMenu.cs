using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

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


    private void Awake()
    {
    }

    public void SetControlMenuKeyboard()
    {
        SetControls setControls = gameObject.AddComponent<SetControls>();
        inputMaster = new InputMaster();
        inputMaster = setControls.SetPlayerControls(inputMaster);

        shootKeyboardText.text = "SHOOT: " + 
                            inputMaster.Player.Shoot.bindings[0].ToDisplayString();
        crouchKeyboardText.text = "CROUCH: " + 
                            inputMaster.Player.Crouch.bindings[0].ToDisplayString();
        grappleKeyboardText.text = "GRAPPLE: " +
                            inputMaster.Player.Grapple.bindings[0].ToDisplayString();
        switchKeyboardText.text = "SWITCH: " +
                            inputMaster.Player.SwitchWeaponButton.bindings[0].ToDisplayString()
                            + " OR " +
                            inputMaster.Player.SwitchWeaponMouseWheel.bindings[0].ToDisplayString();
        reloadKeyboardText.text = "RELOAD: " +
                            inputMaster.Player.Reload.bindings[0].ToDisplayString();
        interactKeyboardText.text = "INTERACT: " +
                            inputMaster.Player.Interact.bindings[0].ToDisplayString();
        moveKeyboardText.text = "MOVE: " +
                            inputMaster.Player.Movement.bindings[1].ToDisplayString() +
                            inputMaster.Player.Movement.bindings[2].ToDisplayString() +
                            inputMaster.Player.Movement.bindings[3].ToDisplayString() +
                            inputMaster.Player.Movement.bindings[4].ToDisplayString();
        grenadeKeyboardText.text = "GRENADE: " +
                            inputMaster.Player.Grenade.bindings[0].ToDisplayString();
        specialKeyboardText.text = "SPECIAL: " +
                            inputMaster.Player.Special.bindings[0].ToDisplayString();
        adsKeyboardText.text = "ADS: " +
                            inputMaster.Player.ADS.bindings[0].ToDisplayString();
    }

    public void SetControlMenuGamepad()
    {
        SetControls setControls = gameObject.AddComponent<SetControls>();
        inputMaster = new InputMaster();
        inputMaster = setControls.SetPlayerControls(inputMaster);

        shootGamepadText.text = "SHOOT: " + inputMaster.Player.Shoot.bindings[1].ToDisplayString();
        crouchGamepadText.text = "CROUCH: " +
                            inputMaster.Player.Crouch.bindings[1].ToDisplayString();
        grappleGamepadText.text = "GRAPPLE: " +
                            inputMaster.Player.Grapple.bindings[1].ToDisplayString();
        switchGamepadText.text = "SWITCH WEAPON: " +
                            inputMaster.Player.SwitchWeaponButton.bindings[1].ToDisplayString();
        reloadGamepadText.text = "RELOAD: " +
                            inputMaster.Player.Reload.bindings[1].ToDisplayString();
        interactGamepadText.text = "INTERACT: " +
                            inputMaster.Player.Interact.bindings[1].ToDisplayString();
        moveGamepadText.text = "MOVE: LEFT STICK";
        grenadeGamepadText.text = "GRENADE: " +
                            inputMaster.Player.Grenade.bindings[1].ToDisplayString();
        specialGamepadText.text = "SPECIAL: " +
                            inputMaster.Player.Special.bindings[1].ToDisplayString();
        adsGamepadText.text = "ADS: " +
                    inputMaster.Player.ADS.bindings[1].ToDisplayString();
    }

    // KEYBOARD AND MOUSE ======================================================================
    public void SetNewShootKeyboard()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Shoot.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindShootKeyboard())
                            .Start();
    }

    public void OnRebindShootKeyboard()
    {
        shootKeyboardText.text = "SHOOT: " + inputMaster.Player.Shoot.bindings[0].ToDisplayString();
        PlayerPrefs.SetString("ShootKeyboard", GetInputString(inputMaster.Player.Shoot.bindings[0].ToString()));
        OnRebindComplete();
    }

    public void SetNewCrouchKeyboard()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Crouch.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindCrouchKeyboard())
                            .Start();
    }

    public void OnRebindCrouchKeyboard()
    {
        crouchKeyboardText.text = "CROUCH: " +
            inputMaster.Player.Crouch.bindings[0].ToDisplayString();
        PlayerPrefs.SetString("CrouchKeyboard", GetInputString(inputMaster.Player.Crouch.bindings[0].ToString()));
        OnRebindComplete();
    }

    public void SetNewGrappleKeyboard()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Grapple.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindGrappleKeyboard())
                            .Start();
    }

    public void OnRebindGrappleKeyboard()
    {
        grappleKeyboardText.text = "GRAPPLE: " +
            inputMaster.Player.Grapple.bindings[0].ToDisplayString();
        PlayerPrefs.SetString("GrappleKeyboard", GetInputString(inputMaster.Player.Grapple.bindings[0].ToString()));
        OnRebindComplete();
    }

    public void SetNewSwitchWeaponKeyboard()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.SwitchWeaponButton.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindWeaponKeyboard())
                            .Start();
    }

    public void OnRebindWeaponKeyboard()
    {
        switchKeyboardText.text = "SWITCH: " +
                inputMaster.Player.SwitchWeaponButton.bindings[0].ToDisplayString()
                + " OR " +
                inputMaster.Player.SwitchWeaponMouseWheel.bindings[0].ToDisplayString();
        PlayerPrefs.SetString("SwitchWeaponKeyboard", GetInputString(inputMaster.Player.SwitchWeaponButton.bindings[0].ToString()));
        OnRebindComplete();
    }

    public void SetNewReloadKeyboard()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Reload.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindReloadKeyboard())
                            .Start();
    }

    public void OnRebindReloadKeyboard()
    {
        reloadKeyboardText.text = "RELOAD: " +
               inputMaster.Player.Reload.bindings[0].ToDisplayString();
        PlayerPrefs.SetString("ReloadKeyboard", GetInputString(inputMaster.Player.Reload.bindings[0].ToString()));
        OnRebindComplete();
    }

    public void SetNewInteractKeyboard()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Interact.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindInteractKeyboard())
                            .Start();
    }

    public void OnRebindInteractKeyboard()
    {
        interactKeyboardText.text = "INTERACT: " +
               inputMaster.Player.Interact.bindings[0].ToDisplayString();
        PlayerPrefs.SetString("InteractKeyboard", GetInputString(inputMaster.Player.Interact.bindings[0].ToString()));
        OnRebindComplete();
    }

    public void SetNewGrenadeKeyboard()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation =inputMaster.Player.Grenade.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindGrappleKeyboard())
                            .Start();
    }

    public void OnRebindGrenadeKeyboard()
    {
        grenadeKeyboardText.text = "GRENADE: " +
               inputMaster.Player.Grenade.bindings[0].ToDisplayString();
        PlayerPrefs.SetString("GrenadeKeyboard", GetInputString(inputMaster.Player.Grenade.bindings[0].ToString()));
        OnRebindComplete();
    }

    public void SetNewSpecialKeyboard()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Special.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindSpecialKeyboard())
                            .Start();
    }

    public void OnRebindSpecialKeyboard()
    {
        specialKeyboardText.text = "SPECIAL: " +
               inputMaster.Player.Special.bindings[0].ToDisplayString();
        PlayerPrefs.SetString("SpecialKeyboard", GetInputString(inputMaster.Player.Special.bindings[0].ToString()));
        OnRebindComplete();
    }

    public void SetNewADSKeyboard()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Special.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindADSKeyboard())
                            .Start();
    }

    public void OnRebindADSKeyboard()
    {
        adsKeyboardText.text = "ADS: " +
               inputMaster.Player.ADS.bindings[0].ToDisplayString();
        PlayerPrefs.SetString("ADSKeyboard", GetInputString(inputMaster.Player.ADS.bindings[0].ToString()));
        OnRebindComplete();
    }

    // END OF KEYBOARD AND MOUSE =================================================================

    // GAMEPAD ===================================================================================

    public void SetNewShootGamepad()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Shoot.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindShootGamepad())
                            .Start();
    }

    public void OnRebindShootGamepad()
    {
        shootGamepadText.text = "SHOOT: " + inputMaster.Player.Shoot.bindings[1].ToDisplayString();
        PlayerPrefs.SetString("ShootGamepad", GetInputString(inputMaster.Player.Shoot.bindings[1].ToString()));
        OnRebindComplete();
    }

    public void SetNewCrouchGamepad()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Crouch.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindCrouchGamepad())
                            .Start();
    }

    public void OnRebindCrouchGamepad()
    {
        crouchGamepadText.text = "CROUCH: " +
            inputMaster.Player.Crouch.bindings[1].ToDisplayString();
        PlayerPrefs.SetString("CrouchGamepad", GetInputString(inputMaster.Player.Crouch.bindings[1].ToString()));
        OnRebindComplete();
    }

    public void SetNewGrappleGamepad()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Grapple.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindGrappleGamepad())
                            .Start();
    }

    public void OnRebindGrappleGamepad()
    {
        grappleGamepadText.text = "GRAPPLE: " +
            inputMaster.Player.Grapple.bindings[1].ToDisplayString();
        PlayerPrefs.SetString("GrappleGamepad", GetInputString(inputMaster.Player.Grapple.bindings[1].ToString()));
        OnRebindComplete();
    }

    public void SetNewSwitchWeaponGamepad()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.SwitchWeaponButton.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindWeaponGamepad())
                            .Start();
    }

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

    public void OnRebindReloadGamepad()
    {
        reloadGamepadText.text = "RELOAD: " +
               inputMaster.Player.Reload.bindings[1].ToDisplayString();
        PlayerPrefs.SetString("ReloadGamepad", GetInputString(inputMaster.Player.Reload.bindings[1].ToString()));
        OnRebindComplete();
    }

    public void SetNewInteractGamepad()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Interact.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindInteractGamepad())
                            .Start();
    }

    public void OnRebindInteractGamepad()
    {
        interactGamepadText.text = "INTERACT: " +
               inputMaster.Player.Interact.bindings[1].ToDisplayString();
        PlayerPrefs.SetString("InteractGamepad", GetInputString(inputMaster.Player.Interact.bindings[1].ToString()));
        OnRebindComplete();
    }

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

    public void OnRebindGrenadeGamepad()
    {
        grenadeGamepadText.text = "GRENADE: " +
               inputMaster.Player.Grenade.bindings[1].ToDisplayString();
        PlayerPrefs.SetString("GrenadeGamepad", GetInputString(inputMaster.Player.Grenade.bindings[1].ToString()));
        OnRebindComplete();
    }

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

    public void OnRebindSpecialGamepad()
    {
        specialGamepadText.text = "SPECIAL: " +
               inputMaster.Player.Special.bindings[1].ToDisplayString();
        PlayerPrefs.SetString("SpecialGamepad", GetInputString(inputMaster.Player.Special.bindings[1].ToString()));
        OnRebindComplete();
    }

    public void SetNewADSGamepad()
    {
        if (rebindInPogress) return;
        rebindInPogress = true;
        rebindingOperation = inputMaster.Player.Special.PerformInteractiveRebinding()
                            .OnMatchWaitForAnother(.1f)
                            .OnComplete(operation => OnRebindADSGamepad())
                            .Start();
    }

    public void OnRebindADSGamepad()
    {
        adsGamepadText.text = "ADS: " +
               inputMaster.Player.ADS.bindings[1].ToDisplayString();
        PlayerPrefs.SetString("ADSGamepad", GetInputString(inputMaster.Player.ADS.bindings[1].ToString()));
        OnRebindComplete();
    }

    // END OF GAMEPAD ============================================================================

    private string GetInputString(string str)
    {
        return str.Substring(str.IndexOf('<'), str.IndexOf('[') - str.IndexOf('<'));
    }

    public void OnRebindComplete()
    {
        rebindingOperation.Dispose();
        //FindObjectOfType<PlayerStats>().RebindContols();
        FindObjectOfType<PlayerShooting>().RebindContols();
        FindObjectOfType<PlayerMovement>().RebindContols();
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
