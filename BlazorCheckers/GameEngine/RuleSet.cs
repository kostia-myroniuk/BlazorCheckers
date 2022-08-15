namespace BlazorCheckers.GameEngine
{
    public class RuleSet
    {
        public bool KingsFly { get; }
        public bool PawnsCaptureBackwards { get; }
        public bool MaximalSequenceChosen { get; }
        public bool PawnsPromoteDuringMove { get; }

        public RuleSet(GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.Normal:
                    KingsFly = true;
                    PawnsCaptureBackwards = true;
                    MaximalSequenceChosen = false;
                    PawnsPromoteDuringMove = true;
                    break;

                case GameMode.International:
                    KingsFly = true;
                    PawnsCaptureBackwards = true;
                    MaximalSequenceChosen = true;
                    PawnsPromoteDuringMove = false;
                    break;

                case GameMode.English:
                    KingsFly = false;
                    PawnsCaptureBackwards = false;
                    MaximalSequenceChosen = false;
                    PawnsPromoteDuringMove = false;
                    break;
            }
        }
    }
}
