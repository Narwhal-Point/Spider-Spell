using UnityEngine;

public class CameraPositionFixer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 10, 0);

    void Start()
    {
        if (player == null)
        {
            return;
        }

        offset = transform.position + new Vector3(0, 10, 0);
    }

    void LateUpdate()
    {
        if (player == null) return;

        transform.position = player.position + offset;
        transform.LookAt(player);
    }

    public void ResetCameraPosition()
    {
        if (player == null) return;

        transform.position = player.position + offset;
        transform.LookAt(player);
    }
}
