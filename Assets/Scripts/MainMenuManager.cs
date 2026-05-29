using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Статична змінна, яка покаже грі, чи треба завантажувати координати
    public static bool IsLoadingSave = false;

    public void StartNewGame()
    {
        // Це нова гра, прапорець вимкнено
        IsLoadingSave = false;

        // Очищаємо старі збереження з пам'яті
        PlayerPrefs.DeleteAll();

        SceneManager.LoadScene("GameScene");
    }

    public void ContinueGame()
    {
        // Якщо в пам'яті є збережена координата Х — завантажуємо сейв
        if (PlayerPrefs.HasKey("PlayerX"))
        {
            IsLoadingSave = true;
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            Debug.Log("Сейвів немає, запускаємо нову гру.");
            StartNewGame();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("GAME CLOSED");
    }
}