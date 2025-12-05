using System;

public class OscLineViewModel
{
    public event Action OnUpdated;                    // 이 라인 데이터가 바뀔 때
    public event Action<OscLineViewModel> OnDelete;   // 이 라인을 삭제해 달라고 요청할 때

    private OscLine _model;

    #region Property
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

    public string IpAddress
    {
        get => _model.IpAddress;
        set
        {
            if (_model.IpAddress == value) return;
            _model.IpAddress = value;
            OnUpdated?.Invoke();
        }
    }

    public int Port
    {
        get => _model.Port;
        set
        {
            if (_model.Port == value) return;
            _model.Port = value;
            OnUpdated?.Invoke();
        }
    }
    #endregion

    public OscLineViewModel(OscLine model)
    {
        _model = model;
    }

    public OscLine GetModel() => _model;

    public void RequestDelete()
    {
        OnDelete?.Invoke(this);
    }
}
