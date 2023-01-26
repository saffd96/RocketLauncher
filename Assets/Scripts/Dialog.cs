using System;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(ZenAutoInjecter))]
public class Dialog : MonoBehaviour
{
    [SerializeField] protected Image background = null;
    [SerializeField] protected Button backgroundButton = null;
    [SerializeField] protected Button closeButton = null;
    [SerializeField] protected bool closeFromTapOnBack = true;
    [SerializeField] private RectTransform contentHolder = null;
    [SerializeField] private Ease _showEaseType = Ease.OutBack;
    [SerializeField] private Ease _hideEaseType = Ease.InBack;
    [SerializeField] private float _hideDuration = 0.25f;
    [SerializeField] private float _showDuration = 0.25f;
    [SerializeField] private float _delayBeforeShow;
    public event Action WillShow;
    public event Action Shown;
    public event Action WillHide;
    public event Action Hidden;
    [Inject] protected SignalBus SignalBus;

    private Color _originalBgColor;
    private Vector3 _cashedContentScale;

    private bool _canHide;

    private Sequence _sequence;
    private Sequence _baseSequence;

    protected bool _initialized = false;

    public virtual void Hide()
    {
        if (!_canHide) return;
        DefaultHide();
    }

    public void ForceHide()
    {
        DefaultHide();
    }

    public Dialog AddShownHandler(Action dialogShownHandler)
    {
        Shown += dialogShownHandler;
        return this;
    }

    public Dialog RemoveShownHandler(Action dialogShownHandler)
    {
        Shown -= dialogShownHandler;
        return this;
    }

    public virtual Dialog AddHiddenHandler(Action dialogHiddenHandler)
    {
        Hidden += dialogHiddenHandler;
        return this;
    }

    public virtual Dialog RemoveHiddenHandler(Action dialogHiddenHandler)
    {
        Hidden -= dialogHiddenHandler;
        return this;
    }


    protected void DefaultShow()
    {
        OnWillShow();
        RunShowAnimation();
    }

    private void DefaultHide()
    {
        OnWillHide();
        RunHideAnimation();
    }

    protected virtual void RunShowAnimation()
    {
        backgroundButton.enabled = false;
        _baseSequence?.Kill();
        _sequence = DOTween.Sequence().SetUpdate(true);
        _sequence.AppendInterval(_delayBeforeShow);
        var c = Color.black;
        c.a = 0;
        background.color = c;

        contentHolder.localScale = Vector3.zero;

        _sequence.Append(background.DOFade(0.85f, _showDuration));
        _sequence.Join(contentHolder.DOScale(_cashedContentScale, _showDuration).SetEase(_showEaseType)
            .OnComplete(OnShown));
        _baseSequence = _sequence;
    }

    protected virtual void RunHideAnimation()
    {
        _baseSequence?.Kill();
        _sequence = DOTween.Sequence().SetUpdate(true);

        _sequence.Append(background.DOFade(0, _hideDuration));
        _sequence.Join(contentHolder.DOScale(Vector3.zero, _hideDuration).SetEase(_hideEaseType)
            .OnComplete(OnHidden));
        _baseSequence = _sequence;
    }


    protected virtual void OnWillShow()
    {
        WillShow?.Invoke();
    }

    protected virtual void OnShown()
    {
        Shown?.Invoke();
        backgroundButton.enabled = true;
        _canHide = true;
    }

    protected virtual void OnWillHide()
    {
        WillHide?.Invoke();
        _canHide = false;
    }

    protected virtual void OnHidden()
    {
        Hidden?.Invoke();
    }

    protected virtual void Start()
    {
    }

    private void OnBtnBackClicked()
    {
        if (closeFromTapOnBack)
        {
            Hide();
        }
    }

    protected void Init()
    {
        if (_initialized)
            return;

        if (backgroundButton != null)
        {
            backgroundButton.onClick.AddListener(OnBtnBackClicked);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnBtnBackClicked);
        }

        if (contentHolder == null)
            contentHolder = (RectTransform) transform;

        _originalBgColor = background.color;
        _cashedContentScale = contentHolder.localScale;

        _initialized = true;
    }
}

public class Dialog<TSignal> : Dialog, IDialog<TSignal>
{
    public virtual void Show(TSignal @params)
    {
        Init();
        Configure(@params);
        DefaultShow();
    }

    protected virtual void Configure(TSignal @params)
    {
    }
}