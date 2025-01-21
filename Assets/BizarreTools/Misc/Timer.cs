using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public float duration;

    public UnityEvent onFinished;

    public TimerStart timerStart;
    public StartAction startAction;

    private float currentTime;
    private bool timeActive;

    public enum TimerStart
    {
        OnAwake,
        OnStart,
        OnEnable,
        OnCommand
    }

    public enum StartAction
    {
        Disable,
        Destroy,
        OnlyInvokeEvent,
    }

    private void Awake()
    {
        timeActive = false;
        if(timerStart == TimerStart.OnAwake)
        {
            timeActive = true;
            currentTime = 0;
        }
    }

    private void Start()
    {
        if(timerStart == TimerStart.OnStart)
        {
            timeActive = true;
            currentTime = 0;
        }
    }

    private void OnEnable()
    {
        if (timerStart == TimerStart.OnEnable)
        {
            timeActive = true;
            currentTime = 0;
        }
    }

    private void OnDisable()
    {
        if (timerStart == TimerStart.OnEnable)
        {
            timeActive = false;
            currentTime = 0;
        }
    }

    private void Update()
    {
        if (timeActive)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= duration)
            {
                switch (startAction)
                {
                    case StartAction.Disable:
                        currentTime = 0;
                        timeActive = false;
                        this.gameObject.SetActive(false);
                        break;
                    case StartAction.Destroy:
                        Destroy(this.gameObject);
                        break;
                }
                onFinished?.Invoke();
                currentTime = 0;
                timeActive = false;
            }
        }
    }

    public void ResetTime()
    {
        timeActive = false;
        currentTime = 0;
    }

    public void EnableTimer()
    {
        currentTime = 0;
        timeActive = true;
    }

}
