using System;

public class PCDeviceLineViewModel
{
    public event Action OnUpdated;                    // 이 라인 데이터가 바뀔 때
    public event Action<PCDeviceLineViewModel> OnDelete;   // 이 라인을 삭제해 달라고 요청할 때

    private PCDeviceLine _model;

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

    public string MacAddress
    {
        get => _model.MacAddress;
        set
        {
            if (_model.MacAddress == value) return;
            _model.MacAddress = value;
            OnUpdated?.Invoke();
        }
    }
    #endregion

    public PCDeviceLineViewModel(PCDeviceLine model)
    {
        _model = model;
    }

    public PCDeviceLine GetModel() => _model;

    public void RequestDelete()
    {
        OnDelete?.Invoke(this);
    }
}
