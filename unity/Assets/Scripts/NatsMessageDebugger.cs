using TMPro;
using UnityEngine;

public class NatsMessageDebugger : MonoBehaviour
{
    [SerializeField] private NatsTranceiver receiver;
    [SerializeField] private TextMeshProUGUI userLatitudeText;
    [SerializeField] private TextMeshProUGUI userLongitudeText;
    [SerializeField] private TextMeshProUGUI userAltitudeText;

    [SerializeField] private TextMeshProUGUI droneLatitudeText;
    [SerializeField] private TextMeshProUGUI droneLongitudeText;
    [SerializeField] private TextMeshProUGUI droneAltitudeText;


    // Update is called once per frame
    void Update()
    {
        userLatitudeText.text = "User Latitude: " + receiver.UserEntity.Latitude.ToString();
        userLongitudeText.text = "User Longitude: " + receiver.UserEntity.Longitude.ToString();
        userAltitudeText.text = "User Altitude (ft): " + receiver.UserEntity.Altitude.ToString();

        droneLatitudeText.text = "Drone Latitude: " + receiver.DroneEntity.Latitude.ToString();
        droneLongitudeText.text = "Drone Longitude: " + receiver.DroneEntity.Longitude.ToString();
        droneAltitudeText.text = "Drone Altitude (ft): " + receiver.DroneEntity.Altitude.ToString();
    }
}
