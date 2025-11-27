using System;
using System.Collections.Generic;
using System.Linq;

public class OscConfigViewModel
{
    private readonly List<OscLineViewModel> _lines = new();

    public IReadOnlyList<OscLineViewModel> Lines => _lines;

    public event Action<OscLineViewModel> LineAdded;
    public event Action<OscLineViewModel> LineRemoved;

    public void initOscConfigViewModel(IEnumerable<OscLine> initialLines = null)
    {
        if (initialLines == null) return;

        foreach (var m in initialLines)
        {
            AddLineFromModel(m);
        }
    }

    private OscLineViewModel AddLineFromModel(OscLine model)
    {
        var vm = new OscLineViewModel(model);
        vm.OnDelete += HandleLineDeleteRequest;

        _lines.Add(vm);
        LineAdded?.Invoke(vm);
        return vm;
    }

    public OscLineViewModel AddEmptyLine()
    {
        var model = new OscLine("", "", 9000);
        return AddLineFromModel(model);
    }

    private void HandleLineDeleteRequest(OscLineViewModel vm)
    {
        RemoveLine(vm);
    }

    public void RemoveLine(OscLineViewModel vm)
    {
        if (!_lines.Remove(vm)) return;

        vm.OnDelete -= HandleLineDeleteRequest;
        LineRemoved?.Invoke(vm);
    }

    public List<OscLine> ToModelList()
    {
        return _lines.Select(l => l.GetModel()).ToList();
    }
}