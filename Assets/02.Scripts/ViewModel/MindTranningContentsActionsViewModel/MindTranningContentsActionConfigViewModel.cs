using System;
using System.Collections.Generic;
using System.Linq;

public class MindTranningContentsActionConfigViewModel
{
    private readonly List<MindTranningContentsActionLineViewModel> _lines = new();

    public IReadOnlyList<MindTranningContentsActionLineViewModel> Lines => _lines;

    public event Action<MindTranningContentsActionLineViewModel> LineAdded;
    public event Action<MindTranningContentsActionLineViewModel> LineRemoved;

    public void initOscConfigViewModel(IEnumerable<ContentsAddressLine> initialLines = null)
    {
        if (initialLines == null) return;

        foreach (var m in initialLines)
        {
            AddLineFromModel(m);
        }
    }

    private MindTranningContentsActionLineViewModel AddLineFromModel(ContentsAddressLine model)
    {
        var vm = new MindTranningContentsActionLineViewModel(model);
        vm.OnDelete += HandleLineDeleteRequest;

        _lines.Add(vm);
        LineAdded?.Invoke(vm);
        return vm;
    }

    public MindTranningContentsActionLineViewModel AddEmptyLine()
    {
        var model = new ContentsAddressLine(0, "", "", "", "", 0, 0);
        return AddLineFromModel(model);
    }

    private void HandleLineDeleteRequest(MindTranningContentsActionLineViewModel vm)
    {
        RemoveLine(vm);
    }

    public void RemoveLine(MindTranningContentsActionLineViewModel vm)
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
