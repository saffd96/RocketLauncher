using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class RocketController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float maxVelocity = 2;
    [SerializeField] private float velocityStep = 1;

    [Inject] private SignalBus _signalBus;

    private bool _isDeath;
    private bool _isNeedToAddVelocity;
    private Camera _mainCam;

    private void Awake()
    {
        _mainCam = Camera.main;
    }

    private void OnEnable()
    {
        _signalBus.Subscribe<Signals.SendIsTouchedSignal>(NeedToAddRocketVelocity);
        _signalBus.Subscribe<Signals.FirstTouchSignal>(Init);
    }

    private void OnDisable()
    {
        _signalBus.TryUnsubscribe<Signals.SendIsTouchedSignal>(NeedToAddRocketVelocity);
        _signalBus.TryUnsubscribe<Signals.FirstTouchSignal>(Init);
    }

    private void OnCollisionEnter(Collision other)
    {
        PlayerDeath();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Platform platform)) return;

        if (platform.IsTriggered) return;

        platform.UsePlatform();
        _signalBus.Fire(new Signals.AddPointsSignal(platform.Points));
    }

    private void FixedUpdate()
    {
        if (_isDeath) return;

        if (!_isNeedToAddVelocity) return;

        SetPos();

        if (rb.velocity.y <= maxVelocity)
        {
            rb.velocity += new Vector3(0, velocityStep, 0);
        }
    }

    private void SetPos()
    {
        var tr = transform;
        var position = tr.position;
        var mousePos = Input.mousePosition;
        var mousePositionVector = new Vector3(mousePos.x, mousePos.y, _mainCam.transform.position.z);
        var mouseXPosInWorld = _mainCam.ScreenToWorldPoint(mousePositionVector).x;
        var neededTransform = new Vector3(-mouseXPosInWorld, position.y, position.z);
        
        var angle = Mathf.Lerp(30, -30, neededTransform.x);

        tr.position = Vector3.MoveTowards(position, neededTransform, 10 * Time.deltaTime);
        tr.eulerAngles = new Vector3(0, angle, 0);
    }

    private void NeedToAddRocketVelocity(Signals.SendIsTouchedSignal signal)
    {
        _isNeedToAddVelocity = signal.IsTouched;
    }

    private void Init()
    {
        rb.useGravity = true;
        _isDeath = false;
    }

    public void PlayerDeath()
    {
        _isDeath = true;
        _signalBus.Fire(new Signals.PlayerDeathSignal());

        StartCoroutine(DeathTime());

        IEnumerator DeathTime()
        {
            yield return new WaitForSeconds(1f);
            _signalBus.Fire(new Signals.ShowLoseDialogSignal());
            rb.useGravity = false;
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
        }
    }
}