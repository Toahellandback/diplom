using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;

    private int contactCount = 0;

    public bool IsGrounded => contactCount > 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
            contactCount++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
            contactCount--;
    }
}