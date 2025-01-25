using UnityEngine.Assertions;

public static class GameplayStates
{

    public delegate void GameFinishedHandler(bool win);
    public delegate void GamePausedHandler();
    public delegate void HealthHandler(float f);

    private static event GameFinishedHandler _OnGameFinished;
    private static event GamePausedHandler _OnGamePaused;
    private static event HealthHandler _BossHealthDelta;

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

    public static event HealthHandler OnBossHealthDelta
    {
        add
        {
            _BossHealthDelta -= value;
            _BossHealthDelta += value;
        }
        remove
        {
            _BossHealthDelta -= value;
        }
    }

    internal static void ChangeBossHealth(float f)
    {
        Assert.IsTrue(f < 1f && f > 0f, "The value must be between 0 and 1.");
        _BossHealthDelta?.Invoke(f);
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
