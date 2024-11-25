using NATS.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public enum PositionType
{
    Latitude, Longitude, Altitude
}
[Serializable]
public class DataEntity
{
    public double Latitude = 0;
    public double Longitude = 0;
    public double Altitude = 0; // in feet
}
public class NatsReceiver : MonoBehaviour
{
    public DataEntity UserEntity => userDataEntity;
    public DataEntity DroneEntity => droneDataEntity;

    [SerializeField] private string ipAddress = "127.0.0.1";
    [SerializeField] private int portNumber = 4222;

    [SerializeField]
    private DataEntity userDataEntity = new();
    [SerializeField]
    private DataEntity droneDataEntity = new();

    private List<string> userSubjects = new List<string>()
        {
            "drone.ground_station_1.user_1.measurement.latitude",
            "drone.ground_station_1.user_1.measurement.longitude",
            "drone.ground_station_1.user_1.measurement.altitude",
        };
    private List<string> droneSubjects = new List<string>()
        {
            "drone.ground_station_1.drone_1.measurement.latitude",
            "drone.ground_station_1.drone_1.measurement.longitude",
            "drone.ground_station_1.drone_1.measurement.altitude",

            "drone.ground_station_1.drone_2.measurement.latitude",
            "drone.ground_station_1.drone_2.measurement.longitude",
            "drone.ground_station_1.drone_2.measurement.altitude",

            "drone.ground_station_1.drone_3.measurement.latitude",
            "drone.ground_station_1.drone_3.measurement.longitude",
            "drone.ground_station_1.drone_3.measurement.altitude",
        };

    private HashSet<string> addedDrones;

    public UnityAction OnUserLatitudeUpdate;
    public UnityAction OnUserLongitudeUpdate;
    public UnityAction OnUserAltitudeUpdate;

    public UnityAction<string> OnNewDroneDetected;
    public UnityAction<string, double> OnDroneLatitudeUpdate;
    public UnityAction<string, double> OnDroneLongitudeUpdate;
    public UnityAction<string, double> OnDroneAltitudeUpdate;

    private CancellationTokenSource cts = new();


    private async void Start()
    {

#if !UNITY_EDITOR
        ipAddress = GlobalConfigData.IP_ADDRESS;
        portNumber = GlobalConfigData.PORT_NUMBER;
#endif
        addedDrones = new();

        await using var nc = new NatsClient("nats://" + ipAddress + ":" + portNumber);
        cts = new CancellationTokenSource();

        Debug.Log($"Listening for messages...");

        var userTasks = userSubjects.Select(async subject =>
        {
            await foreach (var msg in nc.SubscribeAsync<string>(subject).WithCancellation(cts.Token))
            {
                Debug.Log($"Received from {subject}: {msg.Data}");
                if (subject.Equals(userSubjects[0]))
                {
                    OnReceiveUserPosition(double.Parse(msg.Data, CultureInfo.InvariantCulture), PositionType.Latitude);
                }
                else if (subject.Equals(userSubjects[1]))
                {
                    OnReceiveUserPosition(double.Parse(msg.Data, CultureInfo.InvariantCulture), PositionType.Longitude);
                }
                else if (subject.Equals(userSubjects[2]))
                {
                    OnReceiveUserPosition(double.Parse(msg.Data, CultureInfo.InvariantCulture), PositionType.Altitude);
                }
            }
        });

        var droneTasks = droneSubjects.Select(async subject =>
        {
            await foreach (var msg in nc.SubscribeAsync<string>(subject).WithCancellation(cts.Token))
            {
                Debug.Log($"Received from {subject}: {msg.Data}");
                string[] parts = subject.Split('.');
                string name = parts[2];
                string dataType = parts[4];

                if (!addedDrones.Contains(name))
                {
                    addedDrones.Add(name);
                    OnNewDroneDetected?.Invoke(name);
                }
                
                if (dataType.Equals("latitude"))
                {
                    OnReceiveDronePosition(name, double.Parse(msg.Data, CultureInfo.InvariantCulture), PositionType.Latitude);
                }
                else if (dataType.Equals("longitude"))
                {
                    OnReceiveDronePosition(name, double.Parse(msg.Data, CultureInfo.InvariantCulture), PositionType.Longitude);
                }
                else if (dataType.Equals("altitude"))
                {
                    OnReceiveDronePosition(name, double.Parse(msg.Data, CultureInfo.InvariantCulture), PositionType.Altitude);
                }
            }
        });

        await Task.WhenAll(userTasks.Concat(droneTasks));
    }

    private void OnDestroy()
    {
        cts.Cancel();
    }

    private void OnReceiveUserPosition(double data, PositionType type)
    {
        try
        {
            switch (type)
            {
                case PositionType.Latitude:
                    {
                        userDataEntity.Latitude = data;
                        UserManager.Instance.Latitude = data;
                        OnUserLatitudeUpdate?.Invoke();
                        break;
                    }
                case PositionType.Longitude:
                    {
                        userDataEntity.Longitude = data;
                        UserManager.Instance.Longitude = data;
                        OnUserLongitudeUpdate?.Invoke();
                        break;
                    }
                case PositionType.Altitude:
                    {
                        userDataEntity.Altitude = data;
                        UserManager.Instance.Altitude = data;
                        OnUserAltitudeUpdate?.Invoke();
                        break;
                    }
                default:
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    private void OnReceiveDronePosition(string droneName, double data, PositionType type)
    {
        try
        {
            switch (type)
            {
                case PositionType.Latitude:
                    {
                        droneDataEntity.Latitude = data;
                        OnDroneLatitudeUpdate?.Invoke(droneName, data);
                        break;
                    }
                case PositionType.Longitude:
                    {
                        droneDataEntity.Longitude = data;
                        OnDroneLongitudeUpdate?.Invoke(droneName, data);
                        break;
                    }
                case PositionType.Altitude:
                    {
                        droneDataEntity.Altitude = data;
                        OnDroneAltitudeUpdate?.Invoke(droneName, data);
                        break;
                    }
                default:
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
