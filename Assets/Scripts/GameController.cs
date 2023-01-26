using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameController : MonoBehaviour
{
    [SerializeField] private UiController uiController;

    [Inject] private SignalBus _signalBus;
    [Inject] private DialogManager _dialogManager;

    private int _points;
    
    private void Awake()
    {
        _dialogManager.SetContainer(uiController.transform);
    }

    private void OnEnable()
    {
        _signalBus.Subscribe<Signals.ResetGameSignal>(ReloadScene);
        _signalBus.Subscribe<Signals.AddPointsSignal>(AddPoints);
    }

    private void OnDisable()
    {
        _signalBus.TryUnsubscribe<Signals.ResetGameSignal>(ReloadScene);
        _signalBus.TryUnsubscribe<Signals.AddPointsSignal>(AddPoints);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void AddPoints(Signals.AddPointsSignal signal)
    {
        _points += signal.Points;
        _signalBus.Fire(new Signals.UpdateUIPointsSignal(_points));
    }
}