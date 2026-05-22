using UnityEngine;

public class CameraEdgeFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Dead Zone")]
    [SerializeField] private float deadZoneWidth = 3f;
    [SerializeField] private float deadZoneHeight = 2f;

    [Header("Smooth")]
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Offset")]
    [SerializeField] private Vector3 offset = new Vector3(0, 1, -10);

    private Vector3 velocity;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 cameraPos = transform.position;
        Vector3 targetPos = target.position + offset;

        // Horizontal movement
        if (targetPos.x > cameraPos.x + deadZoneWidth)
        {
            cameraPos.x = targetPos.x - deadZoneWidth;
        }
        else if (targetPos.x < cameraPos.x - deadZoneWidth)
        {
            cameraPos.x = targetPos.x + deadZoneWidth;
        }

        // Vertical movement
        if (targetPos.y > cameraPos.y + deadZoneHeight)
        {
            cameraPos.y = targetPos.y - deadZoneHeight;
        }
        else if (targetPos.y < cameraPos.y - deadZoneHeight)
        {
            cameraPos.y = targetPos.y + deadZoneHeight;
        }

        // Smooth movement
        transform.position = Vector3.SmoothDamp(
            transform.position,
            new Vector3(cameraPos.x, cameraPos.y, offset.z),
            ref velocity,
            1f / smoothSpeed
        );
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(deadZoneWidth * 2, deadZoneHeight * 2, 1)
        );
    }
}