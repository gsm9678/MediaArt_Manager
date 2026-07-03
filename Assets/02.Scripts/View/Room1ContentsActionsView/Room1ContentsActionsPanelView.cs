using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Room1ContentsActionsPanelView : MonoBehaviour
{
    [Header("Preset Select")]
    [SerializeField] private TMP_Dropdown presetDropdown;
    [SerializeField] private Button startButton;

    [Header("UI References")]
    [SerializeField] private Room1ContentsActionsLineView linePrefab;
    [SerializeField] private Transform contentParent;

    private Room1ContentsActionConfigViewModel _viewModel;
    private readonly Dictionary<Room1ContentsActionLineViewModel, Room1ContentsActionsLineView> _lineViews = new();

    private List<Room1AddressPreset> _presets = new();
    private bool _suppressDropdownCallback;

    private void Start()
    {
        StartCoroutine(StartRoutine());
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitUntil(() => GameManager.Instance.is_JsonLoad);

        _presets = GameManager.Instance.data.GetRoom1ContentsAddressPresets();

        if (presetDropdown != null)
        {
            presetDropdown.onValueChanged.RemoveAllListeners();
            presetDropdown.onValueChanged.AddListener(OnPresetDropdownChanged);
        }

        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(OnStartClicked);
        }

        RefreshPresetDropdownOptions();
        SetSelectedPreset(GameManager.Instance.SelectedRoom1ContentsAddressPresetIndex);
    }

    private void OnDestroy()
    {
        if (presetDropdown != null)
            presetDropdown.onValueChanged.RemoveListener(OnPresetDropdownChanged);

        if (startButton != null)
            startButton.onClick.RemoveListener(OnStartClicked);

        UnbindViewModel();
    }

    private void OnPresetDropdownChanged(int value)
    {
        if (_suppressDropdownCallback) return;
        SetSelectedPreset(value);
    }

    private void SetSelectedPreset(int index)
    {
        if (_presets == null || _presets.Count == 0)
        {
            SetLines(new List<Room1AddressLine>());
            return;
        }

        GameManager.Instance.SelectedRoom1ContentsAddressPresetIndex = Mathf.Clamp(index, 0, _presets.Count - 1);
        GameManager.Instance.GetSelectedRoom1ContentsAddressLines();

        if (presetDropdown != null)
        {
            _suppressDropdownCallback = true;
            presetDropdown.SetValueWithoutNotify(GameManager.Instance.SelectedRoom1ContentsAddressPresetIndex);
            presetDropdown.RefreshShownValue();
            _suppressDropdownCallback = false;
        }

        var selected = _presets[GameManager.Instance.SelectedRoom1ContentsAddressPresetIndex];
        SetLines(selected.Contents ?? new List<Room1AddressLine>());
    }

    private void RefreshPresetDropdownOptions()
    {
        if (presetDropdown == null) return;

        _suppressDropdownCallback = true;
        presetDropdown.ClearOptions();

        var names = _presets
            .Select((preset, index) => string.IsNullOrWhiteSpace(preset.Title) ? $"Room1 Contents {index + 1}" : preset.Title)
            .ToList();

        if (names.Count == 0)
            names.Add("(No Contents)");

        presetDropdown.AddOptions(names);
        presetDropdown.SetValueWithoutNotify(GameManager.Instance.SelectedRoom1ContentsAddressPresetIndex);
        presetDropdown.RefreshShownValue();
        _suppressDropdownCallback = false;
    }

    private void SetLines(IEnumerable<Room1AddressLine> lines)
    {
        ClearAllLineViews();
        UnbindViewModel();

        _viewModel = new Room1ContentsActionConfigViewModel();
        _viewModel.LineAdded += HandleLineAdded;
        _viewModel.LineRemoved += HandleLineRemoved;
        _viewModel.initOscConfigViewModel(lines);
    }

    private void OnStartClicked()
    {
        GameManager.Instance.Room1ContentsStartAction?.Invoke(0);
    }

    private void HandleLineAdded(Room1ContentsActionLineViewModel vm)
    {
        var view = Instantiate(linePrefab, contentParent);
        view.lineNum = _lineViews.Count;
        view.Bind(vm);
        _lineViews[vm] = view;
    }

    private void HandleLineRemoved(Room1ContentsActionLineViewModel vm)
    {
        if (_lineViews.TryGetValue(vm, out var view))
        {
            Destroy(view.gameObject);
            _lineViews.Remove(vm);
        }
    }

    private void ClearAllLineViews()
    {
        foreach (var kv in _lineViews)
        {
            if (kv.Value != null)
                Destroy(kv.Value.gameObject);
        }

        _lineViews.Clear();

        if (contentParent == null) return;

        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);
    }

    private void UnbindViewModel()
    {
        if (_viewModel == null) return;

        _viewModel.LineAdded -= HandleLineAdded;
        _viewModel.LineRemoved -= HandleLineRemoved;
        _viewModel.Clear();
        _viewModel = null;
    }
}


