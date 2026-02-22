using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float smoothTime = 0.125f;
    
    private Transform target;
    private Vector3 currentVelocity = Vector3.zero;

    void LateUpdate()
    {
        if(target == null) return;

        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }

    public void InjectTarget(Transform tar)
    {
        target = tar;
    }
}
