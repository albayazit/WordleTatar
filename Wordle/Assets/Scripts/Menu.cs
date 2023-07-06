using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public GameObject help;
    public void LoadLevel()
    {
        SceneManager.LoadScene("Level");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OpenTelegram()
    {
        Application.OpenURL("https://t.me/albayazit");
    }

    public void CloseHelp()
    {
        help.gameObject.SetActive(false);
    }

    public void ShowHelp()
    {
        help.gameObject.SetActive(true);
    }
}
