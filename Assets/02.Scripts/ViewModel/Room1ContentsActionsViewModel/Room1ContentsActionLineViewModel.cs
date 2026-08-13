using System;

public class Room1ContentsActionLineViewModel
{
    public event Action OnUpdated;
    public event Action<Room1ContentsActionLineViewModel> OnDelete;

    private readonly Room1AddressLine _model;

    public string Name => _model.Name;

    public Room1ContentsActionLineViewModel(Room1AddressLine model)
    {
        _model = model;
    }

    public Room1AddressLine GetModel() => _model;

    public void RequestDelete()
    {
        OnDelete?.Invoke(this);
    }
}

