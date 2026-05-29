using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    [SerializeField] private GameObject hud;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (coinText != null)
            coinText.text = "0";

        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    public void SetMaxHealth(int max)
    {
        if (healthBar == null) return;

        healthBar.maxValue = max;
        healthBar.value = max;
    }

    public void SetHealth(int current)
    {
        if (healthBar == null) return;

        healthBar.value = current;
    }

    public void SetCoins(int amount)
    {
        coinText.text = amount.ToString();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        pauseMenu.SetActive(isPaused);

        if (hud != null)
            hud.SetActive(!isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void Resume()
    {
        TogglePause();
    }

    public void Restart()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void Quit()
    {
        Time.timeScale = 1f;

        UnityEngine.Debug.Log("QUIT GAME");

        Application.Quit();
    }
}