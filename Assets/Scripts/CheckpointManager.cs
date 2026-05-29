using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private Vector3 currentCheckpoint;
    private bool isSet = false;

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

    public void SetCheckpoint(Vector3 position)
    {
        currentCheckpoint = position;
        isSet = true;
        Debug.Log("Checkpoint set: " + position);
    }

    public Vector3 GetCheckpoint()
    {
        if (!isSet)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                return player.transform.position;
        }
        return currentCheckpoint;
    }
}