using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool isStartCheckpoint = false;

    private void Start()
    {
        if (isStartCheckpoint)
            CheckpointManager.Instance.SetCheckpoint(transform.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            CheckpointManager.Instance.SetCheckpoint(transform.position);
    }
}