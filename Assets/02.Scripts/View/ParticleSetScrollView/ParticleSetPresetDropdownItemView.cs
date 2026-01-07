using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ParticleSetPresetDropdownItemView : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Button deleteButton; // 없어도 됨(원하면 null 가능)

    private ParticleSetPresetViewModel _bindVm;
    private Action<ParticleSetPresetViewModel> _onSelected;

    public void Bind(
        ParticleSetPresetViewModel bindVm,
        List<string> allPresetTitles,
        int currentIndex,
        Action<ParticleSetPresetViewModel> onSelected)
    {
        _bindVm = bindVm;
        _onSelected = onSelected;

        dropdown.onValueChanged.RemoveAllListeners();

        dropdown.ClearOptions();
        dropdown.AddOptions(allPresetTitles);
        dropdown.SetValueWithoutNotify(currentIndex);

        dropdown.onValueChanged.AddListener(_ =>
        {
            // 이 드랍다운이 눌렸다 = 이 프리셋 선택으로 간주
            _onSelected?.Invoke(_bindVm);
        });

        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
        }
    }
}