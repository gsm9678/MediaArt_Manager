using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleSetPresetListViewModel
{
    public List<ParticleSetPresetViewModel> Presets { get; } = new();

    private int _selectedIndex = 0;

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (_selectedIndex == value) return;

            _selectedIndex = value;

            // 누가 SelectedIndex를 바꾸는지 확정 추적
            Debug.Log($"[VM] SelectedIndex <- {_selectedIndex}\n{Environment.StackTrace}");
        }
    }

    public ParticleSetPresetViewModel Selected =>
        (Presets.Count == 0 || SelectedIndex < 0 || SelectedIndex >= Presets.Count)
            ? null
            : Presets[SelectedIndex];

    public void Init(IEnumerable<ParticleSetPreset> initial)
    {
        Presets.Clear();

        if (initial != null)
            Presets.AddRange(initial.Select(p => new ParticleSetPresetViewModel(p)));

        if (Presets.Count == 0)
            SelectedIndex = 0;
        else if (SelectedIndex >= Presets.Count)
            SelectedIndex = Presets.Count - 1;
    }

    public void AddPreset(ParticleSetPreset preset)
    {
        Presets.Add(new ParticleSetPresetViewModel(preset));
        // 여기서 SelectedIndex 절대 변경 X
    }

    public bool RemoveAt(int index)
    {
        if (index < 0 || index >= Presets.Count)
            return false;

        Presets.RemoveAt(index);

        // 선택 인덱스 보정
        if (Presets.Count == 0)
            SelectedIndex = 0;
        else if (SelectedIndex >= Presets.Count)
            SelectedIndex = Presets.Count - 1;

        return true;
    }

    public List<ParticleSetPreset> ToModelList()
    {
        return Presets.Select(p => p.GetModel()).ToList();
    }
}
