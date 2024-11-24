using NATS.Net;
using System;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

public class Loader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI errorText;

    [SerializeField] private Text inputText;

    private string text = "";

    public async void ProcessIP()
    {
        try
        {
            string ip = inputText.text;
            int port = 4222;
            await using var nc = new NatsClient("nats://" + ip + ":" + port);
            await nc.ConnectAsync();
            GlobalConfigData.IP_ADDRESS = ip;
            GlobalConfigData.PORT_NUMBER = port;
            await nc.DisposeAsync();
            SceneManager.LoadScene(1);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            errorText.text = e.Message;
        }
    }

    private void Update()
    {
        if (!text.Equals(inputText.text))
        {
            text = inputText.text;
            int dotCount = text.Count(c => c == '.');
            if (dotCount >= 3)
            {
                ProcessIP();
            }
        }
    }
}
