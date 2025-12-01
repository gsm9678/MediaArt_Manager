using System;
using System.Collections.Generic;
using System.Linq;

public class PCDeviceConfigViewModel
{
    private readonly List<PCDeviceLineViewModel> _lines = new();

    public IReadOnlyList<PCDeviceLineViewModel> Lines => _lines;

    public event Action<PCDeviceLineViewModel> LineAdded;
    public event Action<PCDeviceLineViewModel> LineRemoved;

    public void initOscConfigViewModel(IEnumerable<PCDeviceLine> initialLines = null)
    {
        if (initialLines == null) return;

        foreach (var m in initialLines)
        {
            AddLineFromModel(m);
        }
    }

    private PCDeviceLineViewModel AddLineFromModel(PCDeviceLine model)
    {
        var vm = new PCDeviceLineViewModel(model);
        vm.OnDelete += HandleLineDeleteRequest;

        _lines.Add(vm);
        LineAdded?.Invoke(vm);
        return vm;
    }

    public PCDeviceLineViewModel AddEmptyLine()
    {
        var model = new PCDeviceLine("", "", "");
        return AddLineFromModel(model);
    }

    private void HandleLineDeleteRequest(PCDeviceLineViewModel vm)
    {
        RemoveLine(vm);
    }

    public void RemoveLine(PCDeviceLineViewModel vm)
    {
        if (!_lines.Remove(vm)) return;

        GameManager.Instance.data.PC_DeviceLines.Remove(vm.GetModel());
        vm.OnDelete -= HandleLineDeleteRequest;
        LineRemoved?.Invoke(vm);
    }

    public List<PCDeviceLine> ToModelList()
    {
        return _lines.Select(l => l.GetModel()).ToList();
    }
}
