using System.Diagnostics;
using UnityEngine;

public class PlayerSave : MonoBehaviour
{
    private CoinManager coins;

    private void Start()
    {
        health = GetComponent<PlayerHealth>();
        coins = FindObjectOfType<CoinManager>();

        // Проверяем: если игрок нажал "Продолжить" в главном меню — загружаем всё
        if (MainMenuManager.IsLoadingSave)
        {
            LoadGame();
        }
        else
        {
            Debug.Log("Запущена Новая Игра: спавн игрока на EntryPoint контролирует RoomManager.");
        }
    }

    // =========================
    // SAVE (Вызывается на твоих Сейвпоинтах на уровне)
    // =========================
    public void SaveGame()
    {
        if (SaveSystem.Instance == null || RoomManager.Instance == null)
            return;

        SaveSystem.Instance.SaveGame(
            transform.position,
            health.CurrentHP(),
            coins.GetCoins(),
            RoomManager.Instance.GetRoomIndex()
        );
    }

    // =========================
    // LOAD
    // =========================
    public void LoadGame()
    {
        if (SaveSystem.Instance == null)
            return;

        if (!SaveSystem.Instance.HasSave())
            return;

        // 1. Загружаем HP и монеты
        int hp = SaveSystem.Instance.LoadHP();
        health.SetHealth(hp);

        int savedCoins = SaveSystem.Instance.LoadCoins();
        coins.SetCoins(savedCoins);

        // 2. Загружаем индекс комнаты, где был игрок
        int room = SaveSystem.Instance.LoadRoom();
        if (RoomManager.Instance != null)
        {
            // Передаем индекс в RoomManager и заставляем его сгенерировать эту комнату
            RoomManager.Instance.StartLoadedRoom(room);
        }

        // 3. Переносим игрока ТОЧНО на координаты сохраненного Сейвпоинта
        Vector3 savedPos = SaveSystem.Instance.LoadPosition();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.position = new Vector2(savedPos.x, savedPos.y);
        }
        transform.position = new Vector3(savedPos.x, savedPos.y, 0f);

        Debug.Log("Игра загружена. Игрок восстановлен на Сейвпоинте: " + savedPos);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}