using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    public static bool IsLoadingSave = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =========================
    // SAVE
    // =========================

    public void SaveGame(
        Vector3 playerPos,
        int hp,
        int coins,
        int roomIndex)
    {
        // POSITION
        PlayerPrefs.SetFloat("PlayerX", playerPos.x);
        PlayerPrefs.SetFloat("PlayerY", playerPos.y);
        PlayerPrefs.SetFloat("PlayerZ", playerPos.z);

        // HP
        PlayerPrefs.SetInt("PlayerHP", hp);

        // COINS
        PlayerPrefs.SetInt("Coins", coins);

        // ROOM
        PlayerPrefs.SetInt("RoomIndex", roomIndex);

        PlayerPrefs.Save();

        Debug.Log("GAME SAVED");
    }

    // =========================
    // LOAD
    // =========================

    public Vector3 LoadPosition()
    {
        return new Vector3(
            PlayerPrefs.GetFloat("PlayerX", 0),
            PlayerPrefs.GetFloat("PlayerY", 0),
            PlayerPrefs.GetFloat("PlayerZ", 0)
        );
    }

    public int LoadHP()
    {
        return PlayerPrefs.GetInt("PlayerHP", 3);
    }

    public int LoadCoins()
    {
        return PlayerPrefs.GetInt("Coins", 0);
    }

    public int LoadRoom()
    {
        return PlayerPrefs.GetInt("RoomIndex", 0);
    }

    public bool HasSave()
    {
        return PlayerPrefs.HasKey("PlayerX");
    }

    // =========================
    // DELETE SAVE
    // =========================

    public void DeleteSave()
    {
        PlayerPrefs.DeleteAll();
    }
}