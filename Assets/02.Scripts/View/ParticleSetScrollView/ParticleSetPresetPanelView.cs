using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ParticleSetPresetPanelView : MonoBehaviour
{
    [Header("Preset Select")]
    [SerializeField] private TMP_Dropdown presetDropdown;

    [Header("Preset Edit")]
    [SerializeField] private TMP_InputField titleInput;
    [SerializeField] private TMP_InputField oscInput;
    [SerializeField] private TMP_Dropdown optionDropdown;
    [SerializeField] private Button createButton; 
    [SerializeField] private Button deleteButton;

    [Header("Particle Editor")]
    [SerializeField] private ParticleSetPanelView particlePanel;

    private ParticleSetPresetListViewModel _vm;

    private bool _suppressDropdownCallback;
    private int _lastShownIndex = -1;

    private Coroutine _syncCoroutine;

    private void Start() => StartCoroutine(CoInit());

    private IEnumerator CoInit()
    {
        yield return new WaitUntil(() => GameManager.Instance.is_JsonLoad);

        _vm = new ParticleSetPresetListViewModel();
        _vm.Init(GameManager.Instance.data.ParticleSetPresets);

        presetDropdown.onValueChanged.RemoveAllListeners();
        presetDropdown.onValueChanged.AddListener(OnPresetDropdownChanged);

        createButton.onClick.RemoveAllListeners();
        createButton.onClick.AddListener(OnCreateClicked);

        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(OnDeleteClicked);

        titleInput.onValueChanged.RemoveAllListeners();
        titleInput.onValueChanged.AddListener(OnTitleChanged);

        RefreshPresetDropdownOptions();
        ForceDropdownValue(_vm.SelectedIndex);
        UpdateDisplay();

        // 프레임 끝 동기화 루틴 (드랍다운이 내부적으로 다시 덮어써도 최종적으로 맞춤)
        _syncCoroutine = StartCoroutine(SyncDropdownEndOfFrameLoop());

        GameManager.Instance.GetParticleSetPresets += OnRequestSave;
    }

    private void OnDestroy()
    {
        if (_syncCoroutine != null)
            StopCoroutine(_syncCoroutine);

        if (presetDropdown != null)
            presetDropdown.onValueChanged.RemoveAllListeners();

        if (createButton != null)
            createButton.onClick.RemoveAllListeners();

        if (deleteButton != null)
            deleteButton.onClick.RemoveListener(OnDeleteClicked);

        if (titleInput != null)
            titleInput.onValueChanged.RemoveListener(OnTitleChanged);

        if (GameManager.Instance != null)
            GameManager.Instance.GetParticleSetPresets -= OnRequestSave;
    }

    // =========================
    // Dropdown
    // =========================

    private void OnPresetDropdownChanged(int value)
    {
        if (_suppressDropdownCallback) return;

        CommitIndex(_lastShownIndex);

        _vm.SelectedIndex = value;
        _lastShownIndex = value;

        ForceDropdownValue(value);
        UpdateDisplay();

        Debug.Log($"[PresetChanged] value={value}, vmIndex={_vm.SelectedIndex}, shown={presetDropdown.value}");
    }

    private void ForceDropdownValue(int index)
    {
        _suppressDropdownCallback = true;
        presetDropdown.SetValueWithoutNotify(index);
        presetDropdown.RefreshShownValue();
        _suppressDropdownCallback = false;

        // 한 프레임 뒤에 또 한 번 강제 (TMP 내부 리빌드/캡션 갱신 타이밍 방어)
        StartCoroutine(ForceDropdownValueNextFrame(index));
    }

    private IEnumerator ForceDropdownValueNextFrame(int index)
    {
        yield return new WaitForEndOfFrame();

        _suppressDropdownCallback = true;
        presetDropdown.SetValueWithoutNotify(index);
        presetDropdown.RefreshShownValue();
        _suppressDropdownCallback = false;
    }

    // =========================
    // Always-sync loop (디버그 + 강제 유지)
    // =========================

    private IEnumerator SyncDropdownEndOfFrameLoop()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (_vm == null || presetDropdown == null) continue;
            if (_suppressDropdownCallback) continue;

            // 옵션 개수 불일치면 옵션 재구성
            int optionCount = presetDropdown.options != null ? presetDropdown.options.Count : 0;
            int presetCount = _vm.Presets.Count == 0 ? 1 : _vm.Presets.Count;

            if (optionCount != presetCount)
            {
                RefreshPresetDropdownOptions();
            }

            // 값이 어긋나면 강제로 맞춤
            if (presetDropdown.value != _vm.SelectedIndex)
            {
                Debug.LogWarning($"[SyncFix] dropdown.value({presetDropdown.value}) != vm.SelectedIndex({_vm.SelectedIndex}) → force");
                _suppressDropdownCallback = true;
                presetDropdown.SetValueWithoutNotify(_vm.SelectedIndex);
                presetDropdown.RefreshShownValue();
                _suppressDropdownCallback = false;
            }
        }
    }

    // =========================
    // UI Update
    // =========================

    private void UpdateDisplay()
    {
        var selected = _vm.Selected;
        if (selected == null)
        {
            titleInput.SetTextWithoutNotify("");
            oscInput.SetTextWithoutNotify("");
            optionDropdown.SetValueWithoutNotify(0);
            particlePanel.SetLines(new List<ParticleSetLine>());
            _lastShownIndex = -1;
            return;
        }

        titleInput.SetTextWithoutNotify(selected.Title);
        oscInput.SetTextWithoutNotify(selected.OscAddress);
        optionDropdown.SetValueWithoutNotify(selected.OptionIndex);

        var copy = selected.Particles.Select(p => new ParticleSetLine(p)).ToList();
        particlePanel.SetLines(copy);

        _lastShownIndex = _vm.SelectedIndex;
    }

    // =========================
    // Commit
    // =========================

    private void CommitIndex(int index)
    {
        if (index < 0 || index >= _vm.Presets.Count) return;

        var target = _vm.Presets[index];

        target.Title = titleInput.text;
        target.OscAddress = oscInput.text;
        target.OptionIndex = optionDropdown.value;

        target.Particles.Clear();
        target.Particles.AddRange(
            particlePanel.GetLines().Select(x => new ParticleSetLine(x))
        );
    }

    // =========================
    // Create
    // =========================

    private void OnCreateClicked()
    {
        CommitIndex(_lastShownIndex);

        var preset = new ParticleSetPreset(
            titleInput.text,
            oscInput.text,
            optionDropdown.value,
            new List<ParticleSetLine>()
        );

        _vm.AddPreset(preset);
        _vm.SelectedIndex = _vm.Presets.Count - 1;

        RefreshPresetDropdownOptions();
        ForceDropdownValue(_vm.SelectedIndex);
        UpdateDisplay();
    }

    // =========================
    // Delete
    // =========================

    private void OnDeleteClicked()
    {
        int deleteIndex = _vm.SelectedIndex;

        if (_vm.Presets.Count == 0)
            return;

        // 현재 선택된 프리셋 삭제
        bool removed = _vm.RemoveAt(deleteIndex);
        if (!removed) return;

        // UI 정합성 복구
        _lastShownIndex = _vm.SelectedIndex;

        RefreshPresetDropdownOptions();
        ForceDropdownValue(_vm.SelectedIndex);
        UpdateDisplay();

        Debug.Log($"[PresetDeleted] index={deleteIndex}, newIndex={_vm.SelectedIndex}");
    }

    // =========================
    // Title Change
    // =========================

    private void OnTitleChanged(string value)
    {
        if (_lastShownIndex < 0 || _lastShownIndex >= _vm.Presets.Count)
            return;

        // 현재 프리셋 모델에 즉시 반영
        _vm.Presets[_lastShownIndex].Title = value;

        // 드랍다운 옵션 텍스트 갱신
        RefreshPresetDropdownOptions();

        // 선택값 유지 (깜빡임 방지)
        ForceDropdownValue(_vm.SelectedIndex);
    }

    // =========================
    // Dropdown Options
    // =========================

    private void RefreshPresetDropdownOptions()
    {
        _suppressDropdownCallback = true;

        presetDropdown.ClearOptions();

        var names = _vm.Presets
            .Select((p, i) => string.IsNullOrWhiteSpace(p.Title) ? $"Preset {i + 1}" : p.Title)
            .ToList();

        if (names.Count == 0) names.Add("(No Presets)");

        presetDropdown.AddOptions(names);

        if (_vm.Presets.Count == 0)
            _vm.SelectedIndex = 0;
        else if (_vm.SelectedIndex >= _vm.Presets.Count)
            _vm.SelectedIndex = _vm.Presets.Count - 1;

        presetDropdown.SetValueWithoutNotify(_vm.SelectedIndex);
        presetDropdown.RefreshShownValue();

        _suppressDropdownCallback = false;
    }

    // =========================
    // Save
    // =========================

    private List<ParticleSetPreset> OnRequestSave()
    {
        CommitIndex(_lastShownIndex);
        return _vm.ToModelList();
    }
}
