using System;
using System.Collections.Generic;
using System.Linq;

public class ParticleSetConfigViewModel
{
    private readonly List<ParticleSetLineViewModel> _lines = new();
    public IReadOnlyList<ParticleSetLineViewModel> Lines => _lines;

    public event Action<ParticleSetLineViewModel> LineAdded;
    public event Action<ParticleSetLineViewModel> LineRemoved;

    public void initOscConfigViewModel(IEnumerable<ParticleSetLine> initialLines = null)
    {
        if (initialLines == null) return;

        foreach (var m in initialLines)
            AddLineFromModel(m);
    }

    private ParticleSetLineViewModel AddLineFromModel(ParticleSetLine model)
    {
        var vm = new ParticleSetLineViewModel(model);
        vm.OnDelete += HandleLineDeleteRequest;

        _lines.Add(vm);
        LineAdded?.Invoke(vm);
        return vm;
    }

    public ParticleSetLineViewModel AddEmptyLine()
    {
        var model = new ParticleSetLine(0, 0);
        return AddLineFromModel(model);
    }

    private void HandleLineDeleteRequest(ParticleSetLineViewModel vm) => RemoveLine(vm);

    public void RemoveLine(ParticleSetLineViewModel vm)
    {
        if (!_lines.Remove(vm)) return;

        vm.OnDelete -= HandleLineDeleteRequest;
        LineRemoved?.Invoke(vm);
    }

    public List<ParticleSetLine> ToModelList()
        => _lines.Select(l => l.GetModel()).ToList();
}