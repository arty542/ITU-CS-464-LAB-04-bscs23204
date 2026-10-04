using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0f, 4f, -7f);
    [SerializeField] float smoothSpeed = 5f;

    void Start()
    {
        if (target != null)
            transform.position = target.position + offset;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 wantedPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            wantedPosition,
            smoothSpeed * Time.deltaTime);

        transform.LookAt(target);
    }
}