using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room1AddressPresetListViewModel
{
    public List<Room1AddressPresetViewModel> Presets { get; } = new();

    private int _selectedIndex;

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            int maxIndex = Presets.Count == 0 ? 0 : Presets.Count - 1;
            _selectedIndex = Mathf.Clamp(value, 0, maxIndex);
        }
    }

    public Room1AddressPresetViewModel Selected =>
        Presets.Count == 0 || SelectedIndex < 0 || SelectedIndex >= Presets.Count
            ? null
            : Presets[SelectedIndex];

    public void Init(IEnumerable<Room1AddressPreset> initial)
    {
        Presets.Clear();

        if (initial != null)
        {
            foreach (var preset in initial)
            {
                if (preset == null) continue;
                preset.Contents ??= new List<Room1AddressLine>();
                Presets.Add(new Room1AddressPresetViewModel(preset));
            }
        }

        if (Presets.Count == 0)
            AddPreset(new Room1AddressPreset("Room1 Contents 1", new List<Room1AddressLine>()));

        SelectedIndex = 0;
    }

    public void AddPreset(Room1AddressPreset preset)
    {
        Presets.Add(new Room1AddressPresetViewModel(preset));
    }

    public bool RemoveAt(int index)
    {
        if (index < 0 || index >= Presets.Count)
            return false;

        Presets.RemoveAt(index);

        if (Presets.Count == 0)
            AddPreset(new Room1AddressPreset("Room1 Contents 1", new List<Room1AddressLine>()));

        SelectedIndex = Mathf.Min(index, Presets.Count - 1);
        return true;
    }

    public List<Room1AddressPreset> ToModelList()
    {
        return Presets.Select(preset => preset.GetModel()).ToList();
    }
}

