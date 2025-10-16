using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// later on, teach interface
public class RestartButtonController : MonoBehaviour, IInteractiveButton
{
    // implements the interface
    public void ButtonClick()
    {
        Debug.Log("Onclick restart button");
        GameManager.instance.GameRestart();
    }
}
