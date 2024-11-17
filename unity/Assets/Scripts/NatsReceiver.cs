using NATS.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

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
        };

    private CancellationTokenSource cts = new();


    private async void Start()
    {
        await using var nc = new NatsClient("nats://" + ipAddress + ":" + 4222);
        cts = new CancellationTokenSource();

        Debug.Log($"Listening for messages...");


        var userTasks = userSubjects.Select(async subject =>
        {
            await foreach (var msg in nc.SubscribeAsync<string>(subject).WithCancellation(cts.Token))
            {
                Debug.Log($"Received from {subject}: {msg.Data}");
                if (subject.Equals("drone.ground_station_1.user_1.measurement.latitude"))
                {
                    OnReceiveUserPosition(double.Parse(msg.Data, CultureInfo.InvariantCulture), PositionType.Latitude);
                }
                else if (subject.Equals("drone.ground_station_1.user_1.measurement.longitude"))
                {
                    OnReceiveUserPosition(double.Parse(msg.Data, CultureInfo.InvariantCulture), PositionType.Longitude);

                }
                else if (subject.Equals("drone.ground_station_1.user_1.measurement.altitude"))
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
                switch (subject)
                {
                    case "drone.ground_station_1.drone_1.measurement.latitude":
                        {
                            Debug.Log("GOT LAT");
                            OnReceiveDronePosition(double.Parse(msg.Data, CultureInfo.InvariantCulture), PositionType.Latitude);
                            break;
                        }
                    case "drone.ground_station_1.drone_1.measurement.longitude":
                        {
                            OnReceiveDronePosition(double.Parse(msg.Data, CultureInfo.InvariantCulture), PositionType.Longitude);
                            break;
                        }
                    case "drone.ground_station_1.drone_1.measurement.altitude":
                        {
                            OnReceiveDronePosition(double.Parse(msg.Data, CultureInfo.InvariantCulture), PositionType.Altitude);
                            break;
                        }
                    default:
                        break;
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
        switch (type)
        {
            case PositionType.Latitude:
                {
                    userDataEntity.Latitude = data;
                    break;
                }
            case PositionType.Longitude:
                {
                    userDataEntity.Longitude = data;
                    break;
                }
            case PositionType.Altitude:
                {
                    userDataEntity.Altitude = data;
                    break;
                }
            default:
                break;
        }
    }
    private void OnReceiveDronePosition(double data, PositionType type)
    {
        switch (type)
        {
            case PositionType.Latitude:
                {
                    droneDataEntity.Latitude = data;
                    break;
                }
            case PositionType.Longitude:
                {
                    droneDataEntity.Longitude = data;
                    break;
                }
            case PositionType.Altitude:
                {
                    droneDataEntity.Altitude = data;
                    break;
                }
            default:
                break;
        }
    }
}
