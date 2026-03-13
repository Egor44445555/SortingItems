using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager main;
        
    [Header("Menu")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject successMenu;

    [Header("Audio")]
    [SerializeField] GameObject sounIconDisabled;
    [SerializeField] AudioSource music;
    [SerializeField] AudioSource effect;
    
    Camera cam;
    Vector2 startPoint = new Vector2();
    int currentLevel = 0;
    bool gamePause = false;
    bool endLevel = false;
    float timer = 0f;
    PlayerData playerData;

    void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (JsonSave.main != null)
        {
            playerData = JsonSave.LoadData<PlayerData>("playerData");
            currentLevel = playerData.currentLevel;
        }

        Time.timeScale = 1f;
        cam = Camera.main;

        if (music != null)
        {
            if (!IsSoundsActive())
            {
                music.Pause();
            }
            else
            {
                music.Play();
            }
        }

        Slot[] allSlots = FindObjectsByType<Slot>(FindObjectsSortMode.None);
    }

    void Update()
    {
        if (endLevel)
        {
            timer += Time.deltaTime;
        }

        if (timer > 1f && endLevel)
        {
            StartCoroutine(SuccessLevel());                       
            endLevel = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePause();
        }

        if (Input.GetMouseButtonDown(0))
        {
            startPoint = Input.mousePosition;
        }
    }

    void TogglePause()
    {
        if (gamePause)
        {
            UnpauseGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (gamePause) return;

        gamePause = true;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
            CheckSoundIcon();
        }

        Time.timeScale = 0f;
    }

    public void UnpauseGame()
    {
        if (!gamePause) return;
        
        gamePause = false;

        if (GameObject.FindGameObjectWithTag("Popup"))
        {
            GameObject.FindGameObjectWithTag("Popup").SetActive(false);
        }

        Time.timeScale = 1f;
    }
    
    public bool IsGamePause()
    {
        return gamePause;
    }

    public void SwitchSound()
    {
        if (!IsSoundsActive())
        {
            sounIconDisabled.SetActive(false);
            PlayerPrefs.SetString("SoundEnable", "1");

            if (music != null)
            {
                music.Play();
            }
        }
        else
        {
            sounIconDisabled.SetActive(true);
            PlayerPrefs.SetString("SoundEnable", "0");

            if (music != null)
            {
                music.Pause();
            }
        }

        CheckSoundIcon();
    }

    public void CheckSoundIcon()
    {
        if (!IsSoundsActive())
        {
            sounIconDisabled.SetActive(true);
        }
    }

    public void StartLevel()
    {
        SceneManager.LoadSceneAsync("Level" + currentLevel);
    }

    public void StartNextLevel()
    {
        if (JsonSave.main != null)
        {
            playerData = JsonSave.LoadData<PlayerData>("playerData");
            playerData.currentLevel += 1;
            JsonSave.SaveData(playerData, "playerData");
        }

        SceneManager.LoadSceneAsync("Level" + (currentLevel + 1));
    }

    public bool IsSoundsActive()
    {
        return PlayerPrefs.GetString("SoundEnable") == "1";
    }

    IEnumerator SuccessLevel()
    {
        yield return new WaitForSeconds(0.5f);
        gamePause = true;

        if (successMenu != null)
        {
            successMenu.SetActive(true);
        }
    }
   
    public void ToMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
