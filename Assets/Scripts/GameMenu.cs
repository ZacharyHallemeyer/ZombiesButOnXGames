using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MainMenu
{
    public GameObject gameMenu;
    public PlayerMovement playerMovement;
    public PlayerShooting playerShooting;

    public Slider gameMenuMusicSlider;
    public Slider gameMenuSoundEffectSlider;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
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

    public void SetVolumeGameUI()
    {
        gameMenuMusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        gameMenuSoundEffectSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume");
    }

}
