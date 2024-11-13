using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System;
using NATS.Net;

public class test : MonoBehaviour
{
    private async void Start()
    {
        await using var nc = new NatsClient("demo.nats.io");

        Debug.Log($"Listening for messages on 'hello.A.>'");

        await foreach (var msg in nc.SubscribeAsync<string>(subject: $"hello.A.>"))
        {
            Debug.Log($"Received: {msg.Subject}: {msg.Data}");
        }
    }
}
