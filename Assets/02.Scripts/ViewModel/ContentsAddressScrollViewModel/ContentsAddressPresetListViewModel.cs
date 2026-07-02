using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContentsAddressPresetListViewModel
{
    public List<ContentsAddressPresetViewModel> Presets { get; } = new();

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

    public ContentsAddressPresetViewModel Selected =>
        Presets.Count == 0 || SelectedIndex < 0 || SelectedIndex >= Presets.Count
            ? null
            : Presets[SelectedIndex];

    public void Init(IEnumerable<ContentsAddressPreset> initial)
    {
        Presets.Clear();

        if (initial != null)
        {
            foreach (var preset in initial)
            {
                if (preset == null) continue;
                preset.Contents ??= new List<ContentsAddressLine>();
                Presets.Add(new ContentsAddressPresetViewModel(preset));
            }
        }

        if (Presets.Count == 0)
            AddPreset(new ContentsAddressPreset("Contents 1", new List<ContentsAddressLine>()));

        SelectedIndex = 0;
    }

    public void AddPreset(ContentsAddressPreset preset)
    {
        Presets.Add(new ContentsAddressPresetViewModel(preset));
    }

    public bool RemoveAt(int index)
    {
        if (index < 0 || index >= Presets.Count)
            return false;

        Presets.RemoveAt(index);

        if (Presets.Count == 0)
            AddPreset(new ContentsAddressPreset("Contents 1", new List<ContentsAddressLine>()));

        SelectedIndex = Mathf.Min(index, Presets.Count - 1);
        return true;
    }

    public List<ContentsAddressPreset> ToModelList()
    {
        return Presets.Select(p => p.GetModel()).ToList();
    }
}
