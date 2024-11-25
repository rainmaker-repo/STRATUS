using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    [SerializeField] private NatsReceiver dataReceiver;
    [SerializeField] private DroneController droneControllerInstance;

    private Dictionary<string, DroneController> droneControllers;

    private void Start()
    {
        droneControllers = new();

        dataReceiver.OnNewDroneDetected += SpawnNewDrone;
        dataReceiver.OnDroneLatitudeUpdate += UpdateDroneLatitude;
        dataReceiver.OnDroneLongitudeUpdate += UpdateDroneLongitude;
        dataReceiver.OnDroneAltitudeUpdate += UpdateDroneAltitude;
    }

    private void SpawnNewDrone(string name)
    {
        DroneController newDroneController = Instantiate(droneControllerInstance, transform);
        newDroneController.name = name;
        droneControllers.Add(name, newDroneController);
    }
    private void UpdateDroneLatitude(string name, double data)
    {
        droneControllers[name].Latitude = data;
    }
    private void UpdateDroneLongitude(string name, double data)
    {
        droneControllers[name].Longitude = data;
    }
    private void UpdateDroneAltitude(string name, double data)
    {
        droneControllers[name].Altitude = data;
        droneControllers[name].UpdateVisual();
    }
}
