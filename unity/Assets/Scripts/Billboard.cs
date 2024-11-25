using UnityEngine;
public class Billboard : MonoBehaviour 
{
    [SerializeField] private bool shouldMoveWithCamera = false;

    private Vector3 localOffset;
    private void Start()
    {
        localOffset = transform.localPosition;
    }
    void Update()
    {
        if (shouldMoveWithCamera)
        {
            Vector3 camFwd = Camera.main.transform.forward;
            camFwd.y = 0;
            camFwd.Normalize();
            Vector3 offset = Quaternion.LookRotation(camFwd, Vector3.up) * localOffset;
            transform.position = Camera.main.transform.position + offset;
        }
        Vector3 directionToCamera = Camera.main.transform.position - transform.position;
        transform.forward = -directionToCamera; // Flip the direction
    }
}