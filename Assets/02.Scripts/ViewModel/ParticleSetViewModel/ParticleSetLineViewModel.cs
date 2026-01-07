using System;

public class ParticleSetLineViewModel
{
    public event Action OnUpdated;
    public event Action<ParticleSetLineViewModel> OnDelete;

    private ParticleSetLine _model;

    public int Num
    {
        get => _model.Num;
        set
        {
            if (_model.Num == value) return;
            _model.Num = value;
            OnUpdated?.Invoke();
        }
    }

    public float Time
    {
        get => _model.Time;
        set
        {
            if (_model.Time == value) return;
            _model.Time = value;
            OnUpdated?.Invoke();
        }
    }

    public ParticleSetLineViewModel(ParticleSetLine model)
    {
        _model = model;
    }

    public ParticleSetLine GetModel() => _model;

    public void RequestDelete() => OnDelete?.Invoke(this);
}