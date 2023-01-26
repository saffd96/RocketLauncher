using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(ZenAutoInjecter))]
public class Platform : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private BoxCollider col;
    [SerializeField] private float xOffset;
    [SerializeField] private int points = 1;

    public float XOffset => xOffset;
    public BoxCollider Col => col;
    public bool IsTriggered => _isTriggered;
    public int Points => points;

    [Inject] private SignalBus _signalBus;

    private Tween _tween;
    private bool _isTriggered;

    private void OnEnable()
    {
        _signalBus.Subscribe<Signals.ShowLoseDialogSignal>(Stop);
    }

    public void Init(float velocity, Vector3 pos)
    {
        transform.position = pos;
        _isTriggered = false;
        _tween = transform.DOMoveY(-9999f, velocity).SetSpeedBased();
    }

    public void UsePlatform()
    {
        _isTriggered = true;
    }

    private void OnDisable()
    {
        Stop();
        _signalBus.TryUnsubscribe<Signals.ShowLoseDialogSignal>(Stop);
    }

    private void Stop()
    {
        _tween.Kill();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}