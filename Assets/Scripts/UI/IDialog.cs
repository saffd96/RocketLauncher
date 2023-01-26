using System;

namespace UI
{
    public interface IDialog<in TSignal>
    {
        void Show(TSignal @params);

        void Hide();
        
        event Action WillShow;

        event Action Shown;

        event Action WillHide;

        event Action Hidden;
    }
}