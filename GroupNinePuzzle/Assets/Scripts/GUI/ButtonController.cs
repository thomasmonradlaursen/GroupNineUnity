using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public void QuitProgram()
    {
        Application.Quit();
    }
    public void ReloadGameOnExit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
