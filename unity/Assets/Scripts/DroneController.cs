using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float originalScale = 0.1f;

    [SerializeField] private MeshRenderer targetMesh;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material hoverMaterial;
    [SerializeField] private Material selectMaterial;

    [SerializeField] private CanvasGroup textCanvasGroup;
    [SerializeField] private TextMeshProUGUI droneNameText;
    [SerializeField] private TextMeshProUGUI latitudeText;
    [SerializeField] private TextMeshProUGUI longitudeText;
    [SerializeField] private TextMeshProUGUI altitudeText;
    [SerializeField] private TextMeshProUGUI distanceText;


    public double Latitude = 0;
    public double Longitude = 0;
    public double Altitude = 0;
    public bool IsSelected => isSelected;

    private const double EarthRadius = 6371000; // meters
    private const double MetersPerDegreeLatitude = 111000; // meters
    private const double FeetToMeters = 0.3048;  // Conversion factor from feet to meters

    private bool isSelected = false;
    private Coroutine fadeCoroutine = null;

    private void Start()
    {
        targetMesh.material = defaultMaterial;
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
        UserManager userManager = UserManager.Instance;
        Vector3 relativePos = DeltaLatLonAltToMeters(Latitude - userManager.Latitude,
                                                     Longitude - userManager.Longitude,
                                                     Altitude - userManager.Altitude,
                                                     userManager.Latitude);
        float distance = Mathf.Abs(relativePos.magnitude);

        targetTransform.localPosition = relativePos;

        float scaleMultiplier = Mathf.Clamp(distance, 1f, Mathf.Infinity);
        float evaluatedScale = originalScale * scaleMultiplier;
        targetTransform.localScale = Vector3.one * evaluatedScale;


        droneNameText.text = gameObject.name;
        latitudeText.text = "Latitude: " + Latitude.ToString();
        longitudeText.text = "Longitude: " + Longitude.ToString();
        altitudeText.text = "Altitude: " + Altitude.ToString();
        distanceText.text = "Distance from user: " + distance.ToString();


        // transform.rotation = Quaternion.Euler(0, WorldManager.Instance.TrueHeading, 0);
    }

    public void UpdateVisual()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        fadeCoroutine = StartCoroutine(FadeOpacity(1f));
    }

    private IEnumerator FadeOpacity(float duration)
    {
        float t = 0;
        while (t < duration)
        {
            float opacity = 1 - t / duration;
            Color c = targetMesh.material.GetColor("_BaseColor");
            targetMesh.material.SetColor("_BaseColor", new Color(c.r, c.g, c.b, opacity));
            textCanvasGroup.alpha = 0.5f + opacity / 2f;

            yield return null;
            t += Time.deltaTime;
        }
        fadeCoroutine = null;
    }

    public void OnDroneHovered()
    {
        targetMesh.material = hoverMaterial;
    }
    public void OnDroneUnhovered()
    {
        if (isSelected)
        {
            targetMesh.material = selectMaterial;
        }
        else
        {
            targetMesh.material = defaultMaterial;
        }
    }
    public void OnDroneSelected()
    {
        isSelected = !isSelected;
        if (isSelected)
        {
            targetMesh.material = selectMaterial;
        }
        else
        {
            targetMesh.material = defaultMaterial;
        }
    }
}
