using System;
using System.Collections.Generic;
using System.Linq;

public class DeviceConfigViewModel
{
    private readonly List<DeviceLineViewModel> _lines = new();

    public IReadOnlyList<DeviceLineViewModel> Lines => _lines;

    public event Action<DeviceLineViewModel> LineAdded;
    public event Action<DeviceLineViewModel> LineRemoved;

    public void initOscConfigViewModel(IEnumerable<DeviceLine> initialLines = null)
    {
        if (initialLines == null) return;

        foreach (var m in initialLines)
        {
            AddLineFromModel(m);
        }
    }

    private DeviceLineViewModel AddLineFromModel(DeviceLine model)
    {
        var vm = new DeviceLineViewModel(model);
        vm.OnDelete += HandleLineDeleteRequest;

        _lines.Add(vm);
        LineAdded?.Invoke(vm);
        return vm;
    }

    public DeviceLineViewModel AddEmptyLine()
    {
        var model = new DeviceLine("", "");
        return AddLineFromModel(model);
    }

    private void HandleLineDeleteRequest(DeviceLineViewModel vm)
    {
        RemoveLine(vm);
    }

    public void RemoveLine(DeviceLineViewModel vm)
    {
        if (!_lines.Remove(vm)) return;

        vm.OnDelete -= HandleLineDeleteRequest;
        LineRemoved?.Invoke(vm);
    }

    public List<DeviceLine> ToModelList()
    {
        return _lines.Select(l => l.GetModel()).ToList();
    }
}
