namespace Zenject
{
    public static class Signals
    {
        public readonly struct CloseAllDialogsSignal
        {
        }

        public readonly struct OnNewDialogShowSignal
        {
        }

        public readonly struct ResetGameSignal
        {
        }
    
        public readonly struct FirstTouchSignal
        {
        }
    
        public readonly struct SpawnPlatformSignal
        {
        }
        
        public readonly struct PlayerDeathSignal
        {
        }

        public readonly struct SendIsTouchedSignal
        {
            public readonly bool IsTouched;

            public SendIsTouchedSignal(bool isTouched)
            {
                IsTouched = isTouched;
            }
        }
        
        public readonly struct UpdateUIPointsSignal
        {
            public readonly int Points;

            public UpdateUIPointsSignal(int points)
            {
                Points = points;
            }
        }
        
        public readonly struct AddPointsSignal
        {
            public readonly int Points;

            public AddPointsSignal(int points)
            {
                Points = points;
            }
        }
        
        public readonly struct ShowLoseDialogSignal
        {
        }
    }
}