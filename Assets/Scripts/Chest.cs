using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour
{
    [Header("Reward")]
    [SerializeField] private int coinsInside = 5;

    [Header("Interaction")]
    [SerializeField] private float interactDistance = 1.5f;

    private bool isOpened = false;
    private Animator animator;
    private Transform player;

    private void Start()
    {
        animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (isOpened || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= interactDistance &&
            Keyboard.current.fKey.wasPressedThisFrame)
        {
            Open();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isOpened) return;
        Open();
    }

    private void Open()
    {
        isOpened = true;

        if (animator != null)
            animator.SetTrigger("Open");

        if (CoinManager.Instance != null)
            CoinManager.Instance.AddCoin(coinsInside);

        Debug.Log("Chest opened! +" + coinsInside + " coins");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}