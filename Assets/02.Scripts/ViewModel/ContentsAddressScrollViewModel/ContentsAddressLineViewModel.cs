using System;

public class ContentsAddressLineViewModel
{
    public event Action OnUpdated;                    // 이 라인 데이터가 바뀔 때
    public event Action<ContentsAddressLineViewModel> OnDelete;   // 이 라인을 삭제해 달라고 요청할 때

    private ContentsAddressLine _model;

    #region Property
    public string Num
    {
        get => _model.Num;
        set
        {
            if (_model.Num == value) return;
            _model.Num = value;
            OnUpdated?.Invoke();
        }
    }

    public string Address
    {
        get => _model.Address;
        set
        {
            if (_model.Address == value) return;
            _model.Address = value;
            OnUpdated?.Invoke();
        }
    }

    public int Time
    {
        get => _model.Time;
        set
        {
            if (_model.Time == value) return;
            _model.Time = value;
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
