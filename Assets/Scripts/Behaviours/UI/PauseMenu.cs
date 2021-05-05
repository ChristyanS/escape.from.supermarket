using System.Collections;
using System.Collections.Generic;
using Behaviours.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject endGameUI;



    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }



        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(1);
            VirtualInputManager.Instance.EnableAllControls(true);
        }
        if (endGameUI.gameObject.active)
        {

            EndMenu();
        }


    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

    }

    void EndMenu()
    {
        endGameUI.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

    }
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");

    }

    public void QuitGame()
    {

        Application.Quit();

    }
}
