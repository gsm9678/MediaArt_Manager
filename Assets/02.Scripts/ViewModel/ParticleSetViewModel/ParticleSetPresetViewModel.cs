using System;
using System.Collections.Generic;

public class ParticleSetPresetViewModel
{
    private readonly ParticleSetPreset _model;

    public ParticleSetPresetViewModel(ParticleSetPreset model)
    {
        _model = model;
    }

    public string Title { get => _model.Title; set => _model.Title = value; }
    public string OscAddress { get => _model.OscAddress; set => _model.OscAddress = value; }
    public int OptionIndex { get => _model.OptionIndex; set => _model.OptionIndex = value; }

    public List<ParticleSetLine> Particles => _model.Particles;

    public ParticleSetPreset GetModel() => _model;
}