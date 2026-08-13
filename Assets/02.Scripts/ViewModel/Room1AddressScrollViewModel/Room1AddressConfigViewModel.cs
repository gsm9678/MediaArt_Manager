using System;
using System.Collections.Generic;
using System.Linq;

public class Room1AddressConfigViewModel
{
    private readonly List<Room1AddressLineViewModel> _lines = new();

    public IReadOnlyList<Room1AddressLineViewModel> Lines => _lines;

    public event Action<Room1AddressLineViewModel> LineAdded;
    public event Action<Room1AddressLineViewModel> LineRemoved;

    public void initOscConfigViewModel(IEnumerable<Room1AddressLine> initialLines = null)
    {
        Clear();

        if (initialLines == null) return;

        foreach (var model in initialLines)
            AddRoom1LineFromModel(model);
    }

    public void Clear()
    {
        foreach (var line in _lines)
            line.OnDelete -= HandleLineDeleteRequest;

        _lines.Clear();
    }

    private Room1AddressLineViewModel AddRoom1LineFromModel(Room1AddressLine model)
    {
        var vm = new Room1AddressLineViewModel(model);
        vm.OnDelete += HandleLineDeleteRequest;

        _lines.Add(vm);
        LineAdded?.Invoke(vm);
        return vm;
    }

    public Room1AddressLineViewModel AddEmptyLine()
    {
        var model = new Room1AddressLine(0, "", "", 0);
        return AddRoom1LineFromModel(model);
    }

    private void HandleLineDeleteRequest(Room1AddressLineViewModel vm)
    {
        RemoveLine(vm);
    }

    public void RemoveLine(Room1AddressLineViewModel vm)
    {
        if (!_lines.Remove(vm)) return;

        vm.OnDelete -= HandleLineDeleteRequest;
        LineRemoved?.Invoke(vm);
    }

    public List<Room1AddressLine> ToModelList()
    {
        return _lines.Select(line => new Room1AddressLine(line.GetModel())).ToList();
    }
}

