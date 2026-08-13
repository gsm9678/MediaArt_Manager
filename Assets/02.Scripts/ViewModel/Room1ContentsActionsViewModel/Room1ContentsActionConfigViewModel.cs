using System;
using System.Collections.Generic;
using System.Linq;

public class Room1ContentsActionConfigViewModel
{
    private readonly List<Room1ContentsActionLineViewModel> _lines = new();

    public IReadOnlyList<Room1ContentsActionLineViewModel> Lines => _lines;

    public event Action<Room1ContentsActionLineViewModel> LineAdded;
    public event Action<Room1ContentsActionLineViewModel> LineRemoved;

    public void initOscConfigViewModel(IEnumerable<Room1AddressLine> initialLines = null)
    {
        Clear();

        if (initialLines == null) return;

        foreach (var model in initialLines)
            AddRoom1ActionLineFromModel(model);
    }

    public void Clear()
    {
        foreach (var line in _lines)
            line.OnDelete -= HandleLineDeleteRequest;

        _lines.Clear();
    }

    private Room1ContentsActionLineViewModel AddRoom1ActionLineFromModel(Room1AddressLine model)
    {
        var vm = new Room1ContentsActionLineViewModel(model);
        vm.OnDelete += HandleLineDeleteRequest;

        _lines.Add(vm);
        LineAdded?.Invoke(vm);
        return vm;
    }

    private void HandleLineDeleteRequest(Room1ContentsActionLineViewModel vm)
    {
        RemoveLine(vm);
    }

    public void RemoveLine(Room1ContentsActionLineViewModel vm)
    {
        if (!_lines.Remove(vm)) return;

        vm.OnDelete -= HandleLineDeleteRequest;
        LineRemoved?.Invoke(vm);
    }

    public List<Room1AddressLine> ToModelList()
    {
        return _lines.Select(line => line.GetModel()).ToList();
    }
}

