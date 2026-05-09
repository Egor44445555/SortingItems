using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager main;
    public Transform spawnPoint;
    public PhysicsMaterial2D bouncyMaterial;
    public AudioSource music;

    [SerializeField] float minX = -5f;
    [SerializeField] float maxX = 5f;
    [SerializeField] GameObject guideLine;
    [SerializeField] float throwForce = 6f;
    [SerializeField] TextMeshProUGUI coinsCountText;
    [SerializeField] Canvas hudCanvas;

    int currentSpawnIndex = 0;
    bool canSpawnNext = true;
    int coins = 0;

    GraphicRaycaster graphicRaycaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;
    bool clickToUI = false;

    void Awake()
    {
        main = this;

        if (PlayerPrefs.GetString("MusicEnable") == "")
        {
            PlayerPrefs.SetString("MusicEnable", "1");
        }
    }

    void Start()
    {
        Canvas[] canvasHUD = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        graphicRaycaster = hudCanvas.transform.GetComponent<GraphicRaycaster>();        
        eventSystem = EventSystem.current;

        if (PlayerPrefs.GetString("MusicEnable") == "0")
        {
            music.Stop();
        }
    }

    void Update()
    {
        
    }

    public bool IsTouchOverUI(Vector2 _position)
    {
        pointerEventData = new PointerEventData(eventSystem) { position = _position };
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);
        return results.Count > 0;
    }

    public void SpendCoins(int value)
    {
        coins -= value;

        if (coinsCountText != null)
        {
            coinsCountText.text = coins.ToString();
        }

        YGPlugin.main.SetPlayerCoins(coins.ToString());
    }

    public void AddCoins(int value)
    {
        coins += value;

        if (coinsCountText != null)
        {
            coinsCountText.text = coins.ToString();
        }
    }

    public int GetCoins()
    {
        return coins;
    }

    void OnApplicationQuit()
    {
        YGPlugin.main.SetPlayerCoins(coins.ToString());
    }
}