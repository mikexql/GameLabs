using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// later on, teach interface
public class ReturnToMainMenu : MonoBehaviour, IInteractiveButton
{   
    // implements the interface
    public void ButtonClick()
    {
        Debug.Log("Onclick return to main menu button");
        GameManager.instance.LoadMainMenu();
    }
}
