using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class LoseDialog : Dialog<Signals.ShowLoseDialogSignal>
    {
        [SerializeField] private Button loseButton;

        [Inject] private SignalBus _signalBus;

        private void OnEnable()
        {
            loseButton.onClick.AddListener(Hide);
        }

        private void OnDisable()
        {
            loseButton.onClick.RemoveAllListeners();
        }

        public override void Hide()
        {
            base.Hide();
            _signalBus.Fire(new Signals.ResetGameSignal());

        }
    }
}