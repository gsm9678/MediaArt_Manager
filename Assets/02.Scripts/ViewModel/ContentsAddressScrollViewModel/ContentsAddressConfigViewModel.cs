using System;
using System.Collections.Generic;
using System.Linq;

public class ContentsAddressConfigViewModel
{
    private readonly List<ContentsAddressLineViewModel> _lines = new();

    public IReadOnlyList<ContentsAddressLineViewModel> Lines => _lines;

    public event Action<ContentsAddressLineViewModel> LineAdded;
    public event Action<ContentsAddressLineViewModel> LineRemoved;

    public void initOscConfigViewModel(IEnumerable<ContentsAddressLine> initialLines = null)
    {
        if (initialLines == null) return;

        foreach (var m in initialLines)
        {
            AddLineFromModel(m);
        }
    }

    private ContentsAddressLineViewModel AddLineFromModel(ContentsAddressLine model)
    {
        var vm = new ContentsAddressLineViewModel(model);
        vm.OnDelete += HandleLineDeleteRequest;

        _lines.Add(vm);
        LineAdded?.Invoke(vm);
        return vm;
    }

    public ContentsAddressLineViewModel AddEmptyLine()
    {
        var model = new ContentsAddressLine("", "", "", "", 0);
        return AddLineFromModel(model);
    }

    private void HandleLineDeleteRequest(ContentsAddressLineViewModel vm)
    {
        RemoveLine(vm);
    }

    public void RemoveLine(ContentsAddressLineViewModel vm)
    {
        if (!_lines.Remove(vm)) return;

        vm.OnDelete -= HandleLineDeleteRequest;
        LineRemoved?.Invoke(vm);
    }

    public List<ContentsAddressLine> ToModelList()
    {
        return _lines.Select(l => l.GetModel()).ToList();
    }
}
