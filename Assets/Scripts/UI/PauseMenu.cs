using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void GoToMainMenu()
    {
        // Reset the timescale before loading the title scene
        Time.timeScale = 1f;

        // Destroy the GameStateManager object
        Destroy(GameStateManager.Instance.gameObject);

        // Load the title scene
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        // Reset the timescale before loading the title scene
        Time.timeScale = 1f;

        // Destroy the GameStateManager object
        Destroy(GameStateManager.Instance.gameObject);

        // Load the title scene
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }
}
