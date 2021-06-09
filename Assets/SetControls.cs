using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SetControls : MonoBehaviour
{
    public InputMaster SetPlayerControls(InputMaster inputMaster)
    {
        // Set keyboard and mouse contols
        inputMaster.Player.Crouch.ChangeBindingWithPath(
                           GetInputString(inputMaster.Player.Crouch.bindings[0].ToString())).
                           WithPath(PlayerPrefs.GetString("CrouchKeyboard"));
        inputMaster.Player.Shoot.ChangeBindingWithPath(
                           GetInputString(inputMaster.Player.Shoot.bindings[0].ToString())).
                           WithPath(PlayerPrefs.GetString("ShootKeyboard"));
        inputMaster.Player.Grapple.ChangeBindingWithPath(
                           GetInputString(inputMaster.Player.Grapple.bindings[0].ToString())).
                           WithPath(PlayerPrefs.GetString("GrappleKeyboard"));
        inputMaster.Player.SwitchWeaponButton.ChangeBindingWithPath(
                           GetInputString(inputMaster.Player.SwitchWeaponButton.bindings[0].ToString())).
                           WithPath(PlayerPrefs.GetString("SwitchWeaponKeyboard"));
        inputMaster.Player.Reload.ChangeBindingWithPath(
                           GetInputString(inputMaster.Player.Reload.bindings[0].ToString())).
                           WithPath(PlayerPrefs.GetString("ReloadKeyboard"));
        inputMaster.Player.Interact.ChangeBindingWithPath(
                           GetInputString(inputMaster.Player.Interact.bindings[0].ToString())).
                           WithPath(PlayerPrefs.GetString("InteractKeyboard"));
        inputMaster.Player.Grenade.ChangeBindingWithPath(
                           GetInputString(inputMaster.Player.Grenade.bindings[0].ToString())).
                           WithPath(PlayerPrefs.GetString("GrenadeKeyboard"));
        inputMaster.Player.Special.ChangeBindingWithPath(
                           GetInputString(inputMaster.Player.Special.bindings[0].ToString())).
                           WithPath(PlayerPrefs.GetString("SpecialKeyboard"));
        inputMaster.Player.Escape.ChangeBindingWithPath(
                           GetInputString(inputMaster.Player.Escape.bindings[0].ToString())).
                           WithPath(PlayerPrefs.GetString("EscapeKeyboard"));

        // Set gamepad controls
        inputMaster.Player.Crouch.ChangeBindingWithPath(
                           GetInputString(inputMaster.Player.Crouch.bindings[1].ToString())).
                           WithPath(PlayerPrefs.GetString("CrouchGamepad"));
        inputMaster.Player.Shoot.ChangeBindingWithPath(
                           GetInputString(inputMaster.Player.Shoot.bindings[1].ToString())).
                           WithPath(PlayerPrefs.GetString("ShootGamepad"));
        inputMaster.Player.Grapple.ChangeBindingWithPath(
                           GetInputString(inputMaster.Player.Grapple.bindings[1].ToString())).
                           WithPath(PlayerPrefs.GetString("GrappleGamepad"));
        inputMaster.Player.SwitchWeaponButton.ChangeBindingWithPath(
                           GetInputString(inputMaster.Player.SwitchWeaponButton.bindings[1].ToString())).
                           WithPath(PlayerPrefs.GetString("SwitchWeaponGamepad"));
        inputMaster.Player.Reload.ChangeBindingWithPath(
                           GetInputString(inputMaster.Player.Reload.bindings[1].ToString())).
                           WithPath(PlayerPrefs.GetString("ReloadGamepad"));
        inputMaster.Player.Interact.ChangeBindingWithPath(
                           GetInputString(inputMaster.Player.Interact.bindings[1].ToString())).
                           WithPath(PlayerPrefs.GetString("InteractGamepad"));
        inputMaster.Player.Grenade.ChangeBindingWithPath(
                           GetInputString(inputMaster.Player.Grenade.bindings[1].ToString())).
                           WithPath(PlayerPrefs.GetString("GrenadeGamepad"));
        inputMaster.Player.Special.ChangeBindingWithPath(
                           GetInputString(inputMaster.Player.Special.bindings[1].ToString())).
                           WithPath(PlayerPrefs.GetString("SpecialGamepad"));
        inputMaster.Player.Escape.ChangeBindingWithPath(
                           GetInputString(inputMaster.Player.Escape.bindings[1].ToString())).
                           WithPath(PlayerPrefs.GetString("EscapeGamepad"));


        return inputMaster;
    }

    private string GetInputString(string str)
    {
        return str.Substring(str.IndexOf('<'), str.IndexOf('[') - str.IndexOf('<'));
    }
}
