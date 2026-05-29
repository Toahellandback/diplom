using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance;

    [Header("Room Prefabs")]
    [SerializeField] private GameObject[] randomRooms; // Room1-5
    [SerializeField] private GameObject bossRoom;
    [SerializeField] private GameObject villageRoom;   // стартовая

    [Header("Settings")]
    [SerializeField] private int randomRoomsCount = 2;

    private List<GameObject> spawnedRooms = new List<GameObject>();
    private int currentRoomIndex = 0;
    private Queue<GameObject> roomQueue = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        // Очищаем старые комнаты кроме первой (деревня уже в сцене)
        foreach (var room in spawnedRooms)
            Destroy(room);
        spawnedRooms.Clear();
        roomQueue.Clear();
        currentRoomIndex = 0;

        // Строим очередь: деревня → случайные → арена
        roomQueue.Enqueue(villageRoom);

        List<GameObject> available = new List<GameObject>(randomRooms);
        for (int i = 0; i < randomRoomsCount; i++)
        {
            int idx = Random.Range(0, available.Count);
            roomQueue.Enqueue(available[idx]);
            available.RemoveAt(idx); // без повторов
        }

        roomQueue.Enqueue(bossRoom);

        // Спавним первую комнату (деревня)
        SpawnNextRoom(Vector3.zero);
    }

    public void SpawnNextRoom(Vector3 spawnPosition)
    {
        if (roomQueue.Count == 0)
        {
            // Уровень пройден — перегенерируем
            GenerateLevel();
            return;
        }

        GameObject prefab = roomQueue.Dequeue();
        GameObject room = Instantiate(prefab, spawnPosition, Quaternion.identity);
        spawnedRooms.Add(room);

        // Ставим чекпоинт на EntryPoint комнаты
        Transform entry = room.transform.Find("EntryPoint");
        if (entry != null)
            CheckpointManager.Instance.SetCheckpoint(entry.position);

        // Телепортируем игрока на EntryPoint
        if (currentRoomIndex > 0) // не при старте
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null && entry != null)
                player.transform.position = entry.position;
        }

        currentRoomIndex++;

        // Уничтожаем предыдущую комнату
        if (spawnedRooms.Count > 2)
        {
            Destroy(spawnedRooms[spawnedRooms.Count - 3]);
            spawnedRooms.RemoveAt(spawnedRooms.Count - 3);
        }
    }
}