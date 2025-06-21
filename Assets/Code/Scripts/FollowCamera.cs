using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Transform target; 
    [SerializeField] Vector3 offset;   

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
