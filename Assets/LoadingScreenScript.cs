using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreenScript : MonoBehaviour
{
    // Loading Screen
    public Image loadingScreen;
    public TextMeshProUGUI loadingText;

    // Player scripts
    public PlayerStats playerStats;

    void Start()
    {
        InvokeRepeating("TurnOffLoadingScreen", 1f, .05f);
    }

    /// <summary>
    /// Gradually turns done the alpha of loading screen until it is less than .01 
    /// </summary>
    private void TurnOffLoadingScreen()
    {
        loadingScreen.color *= new Color(1, 1, 1, .9f);
        loadingText.color *= new Color(1, 1, 1, .5f);

        // Prevents player from opening game menu during loading screen
        // Totally unnecessary, but I like it this way
        if (loadingScreen.color.a < .5 && !playerStats.enabled)
            playerStats.enabled = true;

        if (loadingScreen.color.a < .01)
            CancelInvoke("TurnOffLoadingScreen");
    }
}
