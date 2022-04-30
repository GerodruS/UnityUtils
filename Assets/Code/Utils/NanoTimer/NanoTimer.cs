using UnityEngine;

public struct NanoTimer
{
    private float _remaining;
    private float _total;

    public bool IsRunning
        => 0 < _remaining;

    public float Remaining
        => _remaining;

    public float ElapsedTime
        => _total - _remaining;

    public float Progress
        => IsRunning
            ? 1.0f - Mathf.Clamp01(_remaining / _total)
            : 0;

    public void Restart(float duration)
    {
        _remaining = duration;
        _total = duration;
    }

    public void Stop()
    {
        _remaining = 0;
        _total = 0;
    }

    public bool Tick(float deltaTime)
    {
        if (IsRunning)
        {
            _remaining -= deltaTime;
            if (_remaining <= 0)
            {
                Stop();
                return true;
            }
        }
        return false;
    }

    #region ValueType
    public override string ToString()
    {
        return IsRunning
            ? $"[NanoTimer: IsRunning=true Remaining={_remaining:0.00}s]"
            : "[NanoTimer: IsRunning=false]";
    }
    #endregion ValueType
}
