public static class GameplayStates
{

    public delegate void GameFinishedHandler(bool win);

    private static event GameFinishedHandler _OnGameFinished;

    public static event GameFinishedHandler OnGameFinished
    {
        add
        {
            _OnGameFinished -= value;
            _OnGameFinished += value;
        }
        remove
        {
            _OnGameFinished -= value;
        }
    }

    internal static void EndGame(bool win)
    {
        _OnGameFinished?.Invoke(win);
    }
}
