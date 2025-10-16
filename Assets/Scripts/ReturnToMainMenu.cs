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
        if (GameManager.instance != null && GameManager.instance.IsLoading)
        {
            GameManager.instance.CancelLoad();
        }
        else if (GameManager.instance != null)
        {
            GameManager.instance.LoadMainMenu();
        }
    }
}
