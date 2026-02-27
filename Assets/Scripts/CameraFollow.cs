using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float smoothTime = 0.125f;
    
    private Transform target;

    void LateUpdate()
    {
        if(target == null) return;

        Vector3 targetPosition = target.position + offset;

        transform.position = targetPosition;
    }

    public void InjectTarget(Transform tar)
    {
        target = tar;
    }
}
