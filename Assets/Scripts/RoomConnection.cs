using UnityEngine;

public class RoomConnector : MonoBehaviour
{
    public Transform entryPoint;
    public Transform exitPoint;

    private void OnDrawGizmos()
    {
        if (entryPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(entryPoint.position, 0.3f);
        }

        if (exitPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(exitPoint.position, 0.3f);
        }
    }
}