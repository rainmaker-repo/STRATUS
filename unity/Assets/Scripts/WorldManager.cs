using UnityEngine;

public class WorldManager : Singleton<WorldManager>
{
    public float TrueHeading => trueHeading;

    private float trueHeading = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Check if the device has a compass
        if (SystemInfo.supportsGyroscope)
        {
            Debug.Log("DOES SUPPORT GYROSCOPE");
            // Enable the compass
            Input.location.Start();
            Input.compass.enabled = true;
        }
        else
        {
            Debug.Log("DOES NOT SUPPORT GYROSCOPE");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Access the magnetometer via compass
        float magneticHeading = Input.compass.magneticHeading; // Heading in degrees relative to magnetic north
        trueHeading = Input.compass.trueHeading; // Heading in degrees relative to geographic north
        Vector3 rawMagnetometerData = Input.compass.rawVector; // Raw magnetometer data as a vector

        // Log the magnetometer data
        Debug.Log($"Magnetic Heading: {magneticHeading}");
        Debug.Log($"True Heading: {trueHeading}");
        Debug.Log($"Raw Magnetometer Data: {rawMagnetometerData}");
    }
}
