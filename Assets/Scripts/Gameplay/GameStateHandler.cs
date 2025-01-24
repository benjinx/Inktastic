public static class GameplayStates
{

    public delegate void GameFinishedHandler(bool win);
    public delegate void GamePausedHandler();

    private static event GameFinishedHandler _OnGameFinished;
    private static event GamePausedHandler _OnGamePaused;

    public static event GamePausedHandler OnGamePaused {
        add
        {
            _OnGamePaused -= value;
            _OnGamePaused += value;
        }
        remove
        {
            _OnGamePaused -= value;
        }
    }

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

    internal static void PauseGame()
    {
        _OnGamePaused?.Invoke();
    }
}
