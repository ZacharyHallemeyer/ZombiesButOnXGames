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
    public void TurnOffLoadingScreen()
    {
        loadingScreen.color *= new Color(1, 1, 1, .9f);
        if( !(loadingText.color.a < .01) )
            loadingText.color *= new Color(1, 1, 1, .5f);

        // Prevents player from opening game menu during loading screen
        // Totally unnecessary, but I like it this way
        if (loadingScreen.color.a < .5 && !playerStats.enabled)
            playerStats.enabled = true;

        if (loadingScreen.color.a < .01f && loadingText.color.a < .01f)
            CancelInvoke("TurnOffLoadingScreen");
    }

    public void TurnOnLoadingScreen()
    {
        /*loadingScreen.color *= new Color(1, 1, 1, 1.1f);
        loadingText.color *= new Color(1, 1, 1, 1.5f);

        if (loadingScreen.color.a > .9)
        {
            InvokeRepeating("TurnOffLoadingScreen", 1f, .01f);
            CancelInvoke("TurnOnLoadingScreen");
        }*/
        loadingScreen.color = new Color(loadingScreen.color.r, loadingScreen.color.g, loadingScreen.color.b, 1f);
        loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, 1f);
    }
}
