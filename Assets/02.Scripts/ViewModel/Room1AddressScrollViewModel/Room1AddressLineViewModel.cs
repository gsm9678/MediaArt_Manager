using System;

public class Room1AddressLineViewModel
{
    public event Action OnUpdated;
    public event Action<Room1AddressLineViewModel> OnDelete;

    private readonly Room1AddressLine _model;


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

    public Room1AddressLineViewModel(Room1AddressLine model)
    {
        _model = model;
    }

    public Room1AddressLine GetModel() => _model;

    public void RequestDelete()
    {
        OnDelete?.Invoke(this);
    }
}


