using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    [SerializeField] private NatsTranceiver dataTranceiver;
    [SerializeField] private DroneController droneControllerInstance;

    private Dictionary<string, DroneController> droneControllers;

    private void Start()
    {
        droneControllers = new();

        dataTranceiver.OnNewDroneDetected += SpawnNewDrone;
        dataTranceiver.OnDroneLatitudeUpdate += UpdateDroneLatitude;
        dataTranceiver.OnDroneLongitudeUpdate += UpdateDroneLongitude;
        dataTranceiver.OnDroneAltitudeUpdate += UpdateDroneAltitude;
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

    public enum ButtonEventType 
    {
        Down, Up    
    }
    public void OnControllerXButtonDown()
    {
        Debug.Log("X Button down");
        FireButtonEvent("X", ButtonEventType.Down);
    }
    public void OnControllerXButtonUp()
    {
        Debug.Log("X Button up");
        FireButtonEvent("X", ButtonEventType.Up);
    }
    public void OnControllerYButtonDown()
    {
        Debug.Log("Y Button down");
        FireButtonEvent("Y", ButtonEventType.Down);
    }
    public void OnControllerYButtonUp()
    {
        Debug.Log("Y Button up");
        FireButtonEvent("Y", ButtonEventType.Up);
    }
    public void OnControllerAButtonDown()
    {
        Debug.Log("A Button down");
        FireButtonEvent("A", ButtonEventType.Down);
    }
    public void OnControllerAButtonUp()
    {
        Debug.Log("A Button up");
        FireButtonEvent("A", ButtonEventType.Up);
    }
    public void OnControllerBButtonDown()
    {
        Debug.Log("B Button down");
        FireButtonEvent("B", ButtonEventType.Down);
    }
    public void OnControllerBButtonUp()
    {
        Debug.Log("B Button up");
        FireButtonEvent("B", ButtonEventType.Up);
    }

    private async void FireButtonEvent(string buttonName, ButtonEventType eventType)
    {
        List<Task> tasks = new();
        foreach (var controller in droneControllers)
        {
            string droneName = controller.Key;
            DroneController drone = controller.Value;
            if (drone.IsSelected)
            {
                string subjectName = $"drone.ground_station_1.{droneName}.buttonEvent.{buttonName}";
                tasks.Add(dataTranceiver.PublishEvent(subjectName, ((int)eventType).ToString()));
            }
        }
        await Task.WhenAll(tasks);
    }
}