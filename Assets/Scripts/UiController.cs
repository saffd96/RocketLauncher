using System;
using System.Text;
using TMPro;
using UnityEngine;
using Zenject;

public class UiController : MonoBehaviour
{
    [SerializeField] private TMP_Text tmpText;
    [SerializeField] private GameObject tapToStartImage;
    
    [Inject] private SignalBus _signalBus;

    private StringBuilder _stringBuilder;

    private void OnEnable()
    {
        _signalBus.Subscribe<Signals.UpdateUIPointsSignal>(UpdateScore);
        _signalBus.Subscribe<Signals.FirstTouchSignal>(HideStartImage);
    }

    private void OnDisable()
    {
        _signalBus.TryUnsubscribe<Signals.UpdateUIPointsSignal>(UpdateScore);
        _signalBus.TryUnsubscribe<Signals.FirstTouchSignal>(HideStartImage);
    }

    private void Start()
    {
        _stringBuilder = new StringBuilder();
    }

    private void HideStartImage()
    {
        tapToStartImage.SetActive(false);
    }
    
    private void UpdateScore(Signals.UpdateUIPointsSignal signal)
    {
        _stringBuilder.Clear();
        _stringBuilder.Append(signal.Points);
        tmpText.text = _stringBuilder.ToString();
    }
}