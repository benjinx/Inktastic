using UnityEngine.Assertions;

public static class GameplayStates
{

    public delegate void GameFinishedHandler(bool win);
    public delegate void GamePausedHandler();
    public delegate void HealthHandler(float f);
    public delegate void IntegerDeltaHandler(int rawValue, int denominator);

    private static event GameFinishedHandler _OnGameFinished;
    private static event GamePausedHandler _OnGamePaused;
    private static event HealthHandler _BossHealthDelta;
    private static event IntegerDeltaHandler _PlayerHealthDelta;
    private static event IntegerDeltaHandler _PlayerAmmoDelta;

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

    public static event IntegerDeltaHandler OnPlayerHealthDelta
    {
        add
        {
            _PlayerHealthDelta -= value;
            _PlayerHealthDelta += value;
        }
        remove
        {
            _PlayerHealthDelta -= value;
        }
    }

    public static event IntegerDeltaHandler OnPlayerAmmoDelta
    {
        add
        {
            _PlayerAmmoDelta -= value;
            _PlayerAmmoDelta += value;
        }
        remove
        {
            _PlayerAmmoDelta -= value;
        }
    }

    internal static void ChangePlayerAmmo(int rawValue, int denominator)
    {
        Assert.IsTrue(denominator != 0, "Denominator must be a positive integer!");
        _PlayerAmmoDelta?.Invoke(rawValue, denominator);
    }

    internal static void ChangePlayerHealth(int rawValue, int denominator)
    {
        Assert.IsTrue(denominator != 0, "Denominator must be a positive integer!");
        _PlayerHealthDelta?.Invoke(rawValue, denominator);
    }

    internal static void ChangeBossHealth(float f)
    {
        Assert.IsTrue(f <= 1f && f >= 0f, "The value must be between 0 and 1.");
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
