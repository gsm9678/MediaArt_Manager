using System;

public class ContentsAddressLineViewModel
{
    public event Action OnUpdated;                    // 이 라인 데이터가 바뀔 때
    public event Action<ContentsAddressLineViewModel> OnDelete;   // 이 라인을 삭제해 달라고 요청할 때

    private ContentsAddressLine _model;

    #region Property
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

    public string Name
    {
        get => _model.Name;
        set
        {
            if (_model.Name == value) return;
            _model.Name = value;
            OnUpdated?.Invoke();
        }
    }

    public string VideoAddress
    {
        get => _model.VideoAddress;
        set
        {
            if (_model.VideoAddress == value) return;
            _model.VideoAddress = value;
            OnUpdated?.Invoke();
        }
    }

    public string SensorAddress
    {
        get => _model.SensorAddress;
        set
        {
            if (_model.SensorAddress == value) return;
            _model.SensorAddress = value;
            OnUpdated?.Invoke();
        }
    }

    public float ContentsTime
    {
        get => _model.ContentsTime;
        set
        {
            if (_model.ContentsTime == value) return;
            _model.ContentsTime = value;
            OnUpdated?.Invoke();
        }
    }

    public float InteractiveTime
    {
        get => _model.InteractiveTime;
        set
        {
            if (_model.InteractiveTime == value) return;
            _model.InteractiveTime = value;
            OnUpdated?.Invoke();
        }
    }
    #endregion

    public ContentsAddressLineViewModel(ContentsAddressLine model)
    {
        _model = model;
    }

    public ContentsAddressLine GetModel() => _model;

    public void RequestDelete()
    {
        OnDelete?.Invoke(this);
    }
}
