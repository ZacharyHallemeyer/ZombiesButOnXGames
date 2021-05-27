using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoUIScript : MonoBehaviour
{
    public TextMeshProUGUI text;
    
    public void ChangeGunUIText(int currentAmmo, int maxAmmo)
    {
        text.text = currentAmmo + " / " + maxAmmo;
    }
}
