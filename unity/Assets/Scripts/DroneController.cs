using System;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;

    [SerializeField] private NatsReceiver receiver;

    //const double MetersPerDegreeLatitude = 111000; // Approximate meters per degree of latitude
    //private const double FeetToMeters = 0.3048;  // Conversion factor from feet to meters

    //public static Vector3 CalculateRelativePosition(
    //    double lat1, double lon1, double alt1,
    //    double lat2, double lon2, double alt2)
    //{
    //    // Convert latitude from degrees to radians
    //    double lat1Radians = lat1 * Math.PI / 180;

    //    // Calculate differences in longitude and latitude
    //    double dLon = (lon2 - lon1) * MetersPerDegreeLatitude * Math.Cos(lat1Radians);
    //    double dLat = (lat2 - lat1) * MetersPerDegreeLatitude;
    //    double dAlt = (alt2 - alt1) * FeetToMeters;

    //    return new Vector3((float)dLon, (float)dAlt, (float)dLat);
    //}
    private const double EarthRadius = 6371000; // meters
    private const double MetersPerDegreeLatitude = 111000; // meters
    private const double FeetToMeters = 0.3048;  // Conversion factor from feet to meters

    public static Vector3 DeltaLatLonAltToMeters(double deltaLat, double deltaLon, double deltaAltitude, double latitude)
    {
        // Convert delta latitude to meters (always the same scale)
        double deltaLatMeters = deltaLat * (float)MetersPerDegreeLatitude;

        // Calculate meters per degree of longitude at the current latitude
        double metersPerDegreeLongitude = (float)(Math.Cos(latitude * Math.PI / 180) * MetersPerDegreeLatitude);

        // Convert delta longitude to meters (scale depends on latitude)
        double deltaLonMeters = deltaLon * metersPerDegreeLongitude;

        double deltaAltitudeMeters = deltaAltitude * FeetToMeters;

        return new Vector3((float)deltaLonMeters, (float)deltaAltitudeMeters, (float)deltaLatMeters);
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 relativePos = DeltaLatLonAltToMeters(receiver.DroneEntity.Latitude - receiver.UserEntity.Latitude,
                                                     receiver.DroneEntity.Longitude - receiver.UserEntity.Longitude,
                                                     receiver.DroneEntity.Altitude - receiver.UserEntity.Altitude,
                                                     receiver.UserEntity.Latitude);

        targetTransform.localPosition = relativePos;

        // transform.rotation = Quaternion.Euler(0, WorldManager.Instance.TrueHeading, 0);
    }
}
