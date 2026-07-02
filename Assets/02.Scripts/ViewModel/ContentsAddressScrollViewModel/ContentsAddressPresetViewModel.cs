using System.Collections.Generic;

public class ContentsAddressPresetViewModel
{
    private readonly ContentsAddressPreset _model;

    public ContentsAddressPresetViewModel(ContentsAddressPreset model)
    {
        _model = model;
        _model.Contents ??= new List<ContentsAddressLine>();
    }

    public string Title { get => _model.Title; set => _model.Title = value; }
    public List<ContentsAddressLine> Contents => _model.Contents;

    public ContentsAddressPreset GetModel() => _model;
}