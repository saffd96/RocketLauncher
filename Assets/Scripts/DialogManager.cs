using System;
using System.Collections.Generic;
using System.Linq;
using Lean.Pool;
using UnityEngine;
using Zenject;

public class DialogManager : MonoBehaviour, IDisposable
{
    public event Action OnDialogOpened;
    public event Action OnDialogClosed;

    private SignalBus _signalBus;
    private DialogContainer _dialogContainer;

    private readonly List<Dialog> _spawnedDialogs = new List<Dialog>();

    private readonly Dictionary<Type, Dialog> dialogByType = new Dictionary<Type, Dialog>();

    private bool _popupsOpeningDenied;

    public bool IsAnyPopupOpened => _spawnedDialogs.Count > 0;
    public Transform PopUpsContainer { get; private set; }

    [Inject]
    private void Construct(SignalBus iSignalBus, DialogContainer dialogContainer)
    {
        _signalBus = iSignalBus;
        _dialogContainer = dialogContainer;

        _signalBus.Subscribe<Signals.CloseAllDialogsSignal>(HideAll);

        CacheDialogTypes();
    }

    public void Dispose()
    {
        _signalBus.TryUnsubscribe<Signals.CloseAllDialogsSignal>(HideAll);
    }

    public void AllowDialogs()
    {
        _popupsOpeningDenied = false;
    }

    public void DenyDialogs()
    {
        _popupsOpeningDenied = true;
    }

    public TDialog ShowDialog<TDialog, TSignal>(TSignal @params)
        where TDialog : Dialog<TSignal>
    {
        if (_popupsOpeningDenied) return null;

        var popup = SpawnDialog<TDialog>();

        if (popup == null)
        {
            Debug.LogError($"Cannot find popup {typeof(TDialog).Name} for signal {typeof(TSignal).Name}");
            return null;
        }

        popup.Show(@params);

        _signalBus.Fire(new Signals.OnNewDialogShowSignal());

        popup.Hidden += DeSpawn;

        void DeSpawn()
        {
            popup.Hidden -= DeSpawn;

            DeSpawnDialog(popup);
        }

        return popup;
    }

    public TDialog Get<TDialog>() => _spawnedDialogs.OfType<TDialog>().FirstOrDefault();

    public Dialog Get(Type popupType) => _spawnedDialogs.FirstOrDefault(p => p.GetType() == popupType);

    public bool IsShown<TDialog>() => Get<TDialog>() != null;

    public bool IsShown(Type popupType) => Get(popupType) != null;

    public void HideAll()
    {
        var popups = new List<Dialog>(_spawnedDialogs);
        for (int i = 0; i < popups.Count; ++i)
        {
            _spawnedDialogs[i].Hide();
        }
    }

    public void SetContainer(Transform container)
    {
        PopUpsContainer = container;
    }

    private TDialog SpawnDialog<TDialog>()
        where TDialog : Dialog
    {
        var dialogType = typeof(TDialog);

        if (!dialogByType.ContainsKey(dialogType)) return null;

        var gObject = LeanPool.Spawn(dialogByType[dialogType], PopUpsContainer);
        var dialogTransform = gObject.GetComponent<Transform>();
        var dialog = dialogTransform.GetComponent<TDialog>();

        dialogTransform.localScale = Vector3.one;
        dialogTransform.localPosition = Vector3.zero;

        OnDialogOpened?.Invoke();

        dialogTransform.SetAsLastSibling();

        _spawnedDialogs.Add(dialog);

        return dialog;
    }

    private void DeSpawnDialog<TDialog>(TDialog popup)
        where TDialog : Dialog
    {
        var dialogType = typeof(TDialog);

        if (!dialogByType.ContainsKey(dialogType)) return;

        LeanPool.Despawn(_spawnedDialogs.Find(x => x == popup));

        OnDialogClosed?.Invoke();

        _spawnedDialogs.Remove(popup);
    }

    private void CacheDialogTypes()
    {
        foreach (var prefab in _dialogContainer.Dialogs)
        {
            var type = prefab.GetType();
            dialogByType[type] = prefab;
        }
    }
}