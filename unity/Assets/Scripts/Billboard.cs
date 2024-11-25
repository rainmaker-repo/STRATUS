using UnityEngine;
public class Billboard : MonoBehaviour 
{ 
    void Update()
    {
        Vector3 directionToCamera = Camera.main.transform.position - transform.position;
        transform.forward = -directionToCamera; // Flip the direction
    }
}