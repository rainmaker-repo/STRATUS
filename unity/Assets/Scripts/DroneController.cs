using UnityEngine;

public class DroneController : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;


    [SerializeField] private Transform userDataTransform;
    [SerializeField] private Transform droneDataTransform;


    // Update is called once per frame
    private void Update()
    {
        targetTransform.position = droneDataTransform.position - userDataTransform.position;
        transform.rotation = Quaternion.Euler(0, WorldManager.Instance.TrueHeading, 0);
    }
}
