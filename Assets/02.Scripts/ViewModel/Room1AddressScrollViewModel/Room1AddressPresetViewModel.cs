using System.Collections.Generic;

public class Room1AddressPresetViewModel
{
    private readonly Room1AddressPreset _model;

    public Room1AddressPresetViewModel(Room1AddressPreset model)
    {
        _model = model;
        _model.Contents ??= new List<Room1AddressLine>();
    }

    public string Title { get => _model.Title; set => _model.Title = value; }
    public List<Room1AddressLine> Contents => _model.Contents;

    public Room1AddressPreset GetModel() => _model;
}

