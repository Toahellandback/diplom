using UnityEngine;

public class BossExit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            RoomManager.Instance.BackToVillage();
        }
    }
}