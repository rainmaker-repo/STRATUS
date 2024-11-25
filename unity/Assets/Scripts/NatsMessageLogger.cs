using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NatsMessageLogger : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> textLogs;
    [SerializeField] private NatsTranceiver dataTranceiver;

    void Start()
    {
        dataTranceiver.OnEventPublished += OnEventPublished;
    }
    private void OnEventPublished(string subject, string data)
    {
        string prevCached = textLogs[0].text;
        textLogs[0].text = ShowLog(subject, data);

        for (int i = 1; i < textLogs.Count; i++)
        {
            string cache = textLogs[i].text;
            textLogs[i].text = prevCached;
            prevCached = cache;
        }
    }
    private string ShowLog(string subject, string data)
    {
        return $"Published Event::{subject}::{data}";
    }
}
