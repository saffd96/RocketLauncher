using UnityEngine;
using Zenject;

public class TouchController : MonoBehaviour
{
    private bool _isWasFirstTouch;

    [Inject] private SignalBus _signalBus;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SendTouch();
        }

        if (Input.GetMouseButtonUp(0))
        {
            ReleaseTouch();
        }
    }

    private void SendTouch()
    {
        if (!_isWasFirstTouch)
        {
            _signalBus.Fire(new Signals.FirstTouchSignal());
            _isWasFirstTouch = true;
        }

        _signalBus.Fire(new Signals.SendIsTouchedSignal(true));
    }

    private void ReleaseTouch()
    {
        _signalBus.Fire(new Signals.SendIsTouchedSignal(false));
    }
}