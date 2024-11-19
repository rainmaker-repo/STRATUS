using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material targetMaterial;
    [SerializeField] private TextMeshProUGUI targetText;

    [SerializeField] private NatsReceiver receiver;

    private const double EarthRadius = 6371000; // meters
    private const double MetersPerDegreeLatitude = 111000; // meters
    private const double FeetToMeters = 0.3048;  // Conversion factor from feet to meters

    private void Start()
    {
        receiver.OnDroneAltitudeUpdate += OnDroneUpdate;
    }
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
        //Debug.Log("Calculating relative pos");
        Vector3 relativePos = DeltaLatLonAltToMeters(receiver.DroneEntity.Latitude - receiver.UserEntity.Latitude,
                                                     receiver.DroneEntity.Longitude - receiver.UserEntity.Longitude,
                                                     receiver.DroneEntity.Altitude - receiver.UserEntity.Altitude,
                                                     receiver.UserEntity.Latitude);

        targetTransform.localPosition = relativePos;

        // transform.rotation = Quaternion.Euler(0, WorldManager.Instance.TrueHeading, 0);
    }

    private Coroutine _fadeCoroutine = null;
    private void OnDroneUpdate()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }
        _fadeCoroutine = StartCoroutine(FadeOpacity(1f));
    }

    private IEnumerator FadeOpacity(float duration)
    {
        float t = 0;
        while (t < duration)
        {
            float opacity = 1 - t / duration;
            Color c = targetMaterial.GetColor("_BaseColor");
            targetMaterial.SetColor("_BaseColor", new Color(c.r, c.g, c.b, opacity));
            targetText.alpha = opacity;

            yield return null;
            t += Time.deltaTime;
        }
        _fadeCoroutine = null;
    }
}
