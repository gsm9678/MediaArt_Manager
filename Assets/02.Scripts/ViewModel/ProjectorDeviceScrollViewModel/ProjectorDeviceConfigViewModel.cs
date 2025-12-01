using System;
using System.Collections.Generic;
using System.Linq;

public class ProjectorDeviceConfigViewModel
{
    private readonly List<ProjectorDeviceLineViewModel> _lines = new();

    public IReadOnlyList<ProjectorDeviceLineViewModel> Lines => _lines;

    public event Action<ProjectorDeviceLineViewModel> LineAdded;
    public event Action<ProjectorDeviceLineViewModel> LineRemoved;

    public void initOscConfigViewModel(IEnumerable<ProjectorDeviceLine> initialLines = null)
    {
        if (initialLines == null) return;

        foreach (var m in initialLines)
        {
            AddLineFromModel(m);
        }
    }

    private ProjectorDeviceLineViewModel AddLineFromModel(ProjectorDeviceLine model)
    {
        var vm = new ProjectorDeviceLineViewModel(model);
        vm.OnDelete += HandleLineDeleteRequest;

        _lines.Add(vm);
        LineAdded?.Invoke(vm);
        return vm;
    }

    public ProjectorDeviceLineViewModel AddEmptyLine()
    {
        var model = new ProjectorDeviceLine("", "", "");
        return AddLineFromModel(model);
    }

    private void HandleLineDeleteRequest(ProjectorDeviceLineViewModel vm)
    {
        RemoveLine(vm);
    }

    public void RemoveLine(ProjectorDeviceLineViewModel vm)
    {
        if (!_lines.Remove(vm)) return;
        GameManager.Instance.data.Projector_DeviceLines.Remove(vm.GetModel());
        vm.OnDelete -= HandleLineDeleteRequest;
        LineRemoved?.Invoke(vm);
    }

    public List<ProjectorDeviceLine> ToModelList()
    {
        return _lines.Select(l => l.GetModel()).ToList();
    }
}
