using UnityEngine;
using System.Collections;


public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [Header("Rooms")]
    public GameObject villageRoom;
    public GameObject bossRoom;
    public GameObject[] normalRooms;

    [Header("Player")]
    public Transform player;

    private GameObject currentRoom;
    private int roomCounter = 0;
    private bool isSwitchingRoom = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Якщо це НОВА ГРА — створюємо стартову деревню
        if (!MainMenuManager.IsLoadingSave)
        {
            StartCoroutine(SpawnStartRoom());
        }
    }

    IEnumerator SpawnStartRoom()
    {
        yield return new WaitForEndOfFrame();
        roomCounter = 0;
        SpawnRoom(villageRoom);
    }

    // Цей метод викликається з PlayerSave ТОЛЬКО при натисканні "Продовжити"
    public void StartLoadedRoom(int roomIndex)
    {
        roomCounter = roomIndex;

        int phase = roomCounter % 4;
        GameObject targetPrefab;

        if (phase == 0)
        {
            targetPrefab = villageRoom;
        }
        else if (phase == 3)
        {
            targetPrefab = bossRoom;
        }
        else
        {
            int randomIndex = Random.Range(0, normalRooms.Length);
            targetPrefab = normalRooms[randomIndex];
        }

        // Просто створюємо кімнату в нулі сцени
        currentRoom = Instantiate(targetPrefab, Vector3.zero, Quaternion.identity);
        Debug.Log("Кімнату відновлено з індексом: " + roomCounter);
    }

    public void NextRoom()
    {
        if (isSwitchingRoom) return;
        StartCoroutine(NextRoomRoutine());
    }

    IEnumerator NextRoomRoutine()
    {
        isSwitchingRoom = true;
        roomCounter++;

        if (currentRoom != null)
            Destroy(currentRoom);

        yield return new WaitForEndOfFrame();

        int phase = roomCounter % 4;
        GameObject nextRoomPrefab;

        if (phase == 0) nextRoomPrefab = villageRoom;
        else if (phase == 3) nextRoomPrefab = bossRoom;
        else
        {
            int randomIndex = Random.Range(0, normalRooms.Length);
            nextRoomPrefab = normalRooms[randomIndex];
        }

        SpawnRoom(nextRoomPrefab);

        yield return new WaitForSeconds(0.1f);
        isSwitchingRoom = false;
    }

    void SpawnRoom(GameObject roomPrefab)
    {
        currentRoom = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);

        Transform entryPoint = currentRoom.transform.Find("EntryPoint");

        if (entryPoint == null)
        {
            Debug.LogError("EntryPoint не знайдено в: " + roomPrefab.name);
            return;
        }

        TeleportPlayer(entryPoint.position);

        if (CheckpointManager.Instance != null)
            CheckpointManager.Instance.SetCheckpoint(entryPoint.position);
    }

    void TeleportPlayer(Vector3 targetPos)
    {
        if (player == null) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.position = new Vector2(targetPos.x, targetPos.y);
        }

        player.position = new Vector3(targetPos.x, targetPos.y, 0f);
    }

    public void BackToVillage()
    {
        if (isSwitchingRoom) return;
        StartCoroutine(BackToVillageRoutine());
    }

    IEnumerator BackToVillageRoutine()
    {
        isSwitchingRoom = true;
        roomCounter = 0;

        if (currentRoom != null)
            Destroy(currentRoom);

        yield return new WaitForEndOfFrame();

        SpawnRoom(villageRoom);

        yield return new WaitForSeconds(0.1f);
        isSwitchingRoom = false;
    }

    public int GetRoomIndex() => roomCounter;

    public void LoadRoom(int room)
    {
        roomCounter = room;
    }
}