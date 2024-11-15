using NATS.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum PositionType
{
    X, Y, Z
}
public class NatsReceiver : MonoBehaviour
{
    [SerializeField] private Transform user;
    [SerializeField] private Transform drone;

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
        await using var nc = new NatsClient("nats://127.0.0.1:4222");
        cts = new CancellationTokenSource();

        Debug.Log($"Listening for messages...");


        var userTasks = userSubjects.Select(async subject =>
        {
            await foreach (var msg in nc.SubscribeAsync<string>(subject).WithCancellation(cts.Token))
            {
                Debug.Log($"Received from {subject}: {msg.Subject}: {msg.Data}");
                if (subject.Equals("drone.ground_station_1.user_1.measurement.latitude"))
                {
                    OnReceiveUserPosition(float.Parse(msg.Data), PositionType.Z);
                }
                else if (subject.Equals("drone.ground_station_1.user_1.measurement.longitude"))
                {
                    OnReceiveUserPosition(float.Parse(msg.Data), PositionType.X);

                }
                else if (subject.Equals("drone.ground_station_1.user_1.measurement.altitude"))
                {
                    OnReceiveUserPosition(float.Parse(msg.Data), PositionType.Y);
                }
            }
        });

        var droneTasks = droneSubjects.Select(async subject =>
        {
            await foreach (var msg in nc.SubscribeAsync<string>(subject).WithCancellation(cts.Token))
            {
                Debug.Log($"Received from {subject}: {msg.Subject}: {msg.Data}");
                switch (subject)
                {
                    case "drone.ground_station_1.drone_1.measurement.latitude":
                        {
                            OnReceiveDronePosition(float.Parse(msg.Data), PositionType.Z);
                            break;
                        }
                    case "drone.ground_station_1.drone_1.measurement.longitude":
                        {
                            OnReceiveDronePosition(float.Parse(msg.Data), PositionType.X);
                            break;
                        }
                    case "drone.ground_station_1.drone_1.measurement.altitude":
                        {
                            OnReceiveDronePosition(float.Parse(msg.Data), PositionType.Y);
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

    private void OnReceiveUserPosition(float data, PositionType type)
    {
        switch (type)
        {
            case PositionType.X:
                {
                    user.transform.localPosition = new Vector3(data, user.transform.position.y, user.transform.position.z);
                    break;
                }
            case PositionType.Y:
                {
                    user.transform.localPosition = new Vector3(user.transform.position.x, data, user.transform.position.z);
                    break;
                }
            case PositionType.Z:
                {
                    user.transform.localPosition = new Vector3(user.transform.position.x, user.transform.position.y, data);
                    break;
                }
            default:
                break;
        }
    }
    private void OnReceiveDronePosition(float data, PositionType type)
    {
        switch (type)
        {
            case PositionType.X:
                {
                    drone.transform.localPosition = new Vector3(data, drone.transform.position.y, drone.transform.position.z);
                    break;
                }
            case PositionType.Y:
                {
                    drone.transform.localPosition = new Vector3(drone.transform.position.x, data, drone.transform.position.z);
                    break;
                }
            case PositionType.Z:
                {
                    drone.transform.localPosition = new Vector3(drone.transform.position.x, drone.transform.position.y, data);
                    break;
                }
            default:
                break;
        }
    }

    public static double CalculateBearing(double lat1, double lon1, double lat2, double lon2)
    {
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var dPhi = Math.Log(
            Math.Tan(lat2 * Math.PI / 360 + Math.PI / 4) / Math.Tan(lat1 * Math.PI / 360 + Math.PI / 4)
        );
        if (Math.Abs(dLon) > Math.PI)
        {
            dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);
        }
        return (Math.Atan2(dLon, dPhi) * 180 / Math.PI + 360) % 360; // Bearing in degrees
    }
}
