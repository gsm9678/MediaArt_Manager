using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Room1AddressPanelView : MonoBehaviour
{
    [Header("Preset Select")]
    [SerializeField] private TMP_Dropdown presetDropdown;

    [Header("Preset Edit")]
    [SerializeField] private TMP_InputField titleInput;
    [SerializeField] private Button createButton;
    [SerializeField] private Button deleteButton;

    [Header("UI References")]
    [SerializeField] private Room1AddressLineView linePrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private Button addButton;

    private Room1AddressPresetListViewModel _presetVm;
    private Room1AddressConfigViewModel _viewModel;
    private readonly Dictionary<Room1AddressLineViewModel, Room1AddressLineView> _lineViews = new();

    private bool _suppressDropdownCallback;
    private int _lastShownIndex = -1;

    private void Start()
    {
        StartCoroutine(StartRoutine());
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitUntil(() => GameManager.Instance.is_JsonLoad);

        _presetVm = new Room1AddressPresetListViewModel();
        _presetVm.Init(GameManager.Instance.data.GetRoom1ContentsAddressPresets());

        if (presetDropdown != null)
        {
            presetDropdown.onValueChanged.RemoveAllListeners();
            presetDropdown.onValueChanged.AddListener(OnPresetDropdownChanged);
        }

        if (createButton != null)
        {
            createButton.onClick.RemoveAllListeners();
            createButton.onClick.AddListener(OnCreateClicked);
        }

        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(OnDeleteClicked);
        }

        if (titleInput != null)
        {
            titleInput.onValueChanged.RemoveAllListeners();
            titleInput.onValueChanged.AddListener(OnTitleChanged);
        }

        if (addButton != null)
        {
            addButton.onClick.RemoveAllListeners();
            addButton.onClick.AddListener(OnAddClicked);
        }

        RefreshPresetDropdownOptions();
        SetSelectedPreset(0);

        GameManager.Instance.GetRoom1ContentsAddressLine += GetCurrentRoom1AddressLines;
        GameManager.Instance.GetRoom1ContentsAddressPresets += GetCurrentRoom1AddressPresets;
    }

    private void OnDestroy()
    {
        if (presetDropdown != null)
            presetDropdown.onValueChanged.RemoveListener(OnPresetDropdownChanged);

        if (createButton != null)
            createButton.onClick.RemoveListener(OnCreateClicked);

        if (deleteButton != null)
            deleteButton.onClick.RemoveListener(OnDeleteClicked);

        if (titleInput != null)
            titleInput.onValueChanged.RemoveListener(OnTitleChanged);

        if (addButton != null)
            addButton.onClick.RemoveListener(OnAddClicked);

        UnbindViewModel();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GetRoom1ContentsAddressLine -= GetCurrentRoom1AddressLines;
            GameManager.Instance.GetRoom1ContentsAddressPresets -= GetCurrentRoom1AddressPresets;
        }
    }

    private void OnPresetDropdownChanged(int value)
    {
        if (_suppressDropdownCallback) return;

        CommitIndex(_lastShownIndex);
        SetSelectedPreset(value);
    }

    private void SetSelectedPreset(int index)
    {
        if (_presetVm == null) return;

        _presetVm.SelectedIndex = index;
        GameManager.Instance.SelectedRoom1ContentsAddressPresetIndex = _presetVm.SelectedIndex;

        ForceDropdownValue(_presetVm.SelectedIndex);
        UpdateDisplay();
    }

    private void ForceDropdownValue(int index)
    {
        if (presetDropdown == null) return;

        _suppressDropdownCallback = true;
        presetDropdown.SetValueWithoutNotify(index);
        presetDropdown.RefreshShownValue();
        _suppressDropdownCallback = false;
    }

    private void UpdateDisplay()
    {
        var selected = _presetVm?.Selected;
        if (selected == null)
        {
            if (titleInput != null)
                titleInput.SetTextWithoutNotify("");

            SetLines(new List<Room1AddressLine>());
            _lastShownIndex = -1;
            return;
        }

        if (titleInput != null)
            titleInput.SetTextWithoutNotify(selected.Title);

        SetLines(selected.Contents.Select(line => new Room1AddressLine(line)).ToList());
        _lastShownIndex = _presetVm.SelectedIndex;
    }

    private void SetLines(List<Room1AddressLine> lines)
    {
        ClearAllLineViews();
        UnbindViewModel();

        _viewModel = new Room1AddressConfigViewModel();
        _viewModel.LineAdded += HandleLineAdded;
        _viewModel.LineRemoved += HandleLineRemoved;
        _viewModel.initOscConfigViewModel(lines ?? new List<Room1AddressLine>());
    }

    private void CommitIndex(int index)
    {
        if (_presetVm == null || index < 0 || index >= _presetVm.Presets.Count || _viewModel == null)
            return;

        var target = _presetVm.Presets[index];

        if (titleInput != null)
            target.Title = titleInput.text;

        target.Contents.Clear();
        target.Contents.AddRange(_viewModel.ToModelList());

        if (index == GameManager.Instance.SelectedRoom1ContentsAddressPresetIndex)
            GameManager.Instance.data.Room1ContentsAddressLines = target.Contents.Select(line => new Room1AddressLine(line)).ToList();
    }

    private void OnAddClicked()
    {
        _viewModel?.AddEmptyLine();
    }

    private void OnCreateClicked()
    {
        CommitIndex(_lastShownIndex);

        var title = titleInput != null && !string.IsNullOrWhiteSpace(titleInput.text)
            ? titleInput.text
            : $"Room1 Contents {_presetVm.Presets.Count + 1}";

        _presetVm.AddPreset(new Room1AddressPreset(title, new List<Room1AddressLine>()));
        RefreshPresetDropdownOptions();
        SetSelectedPreset(_presetVm.Presets.Count - 1);
    }

    private void OnDeleteClicked()
    {
        if (_presetVm == null) return;

        _presetVm.RemoveAt(_presetVm.SelectedIndex);
        RefreshPresetDropdownOptions();
        SetSelectedPreset(_presetVm.SelectedIndex);
    }

    private void OnTitleChanged(string value)
    {
        if (_presetVm == null || _lastShownIndex < 0 || _lastShownIndex >= _presetVm.Presets.Count)
            return;

        _presetVm.Presets[_lastShownIndex].Title = value;
        RefreshPresetDropdownOptions();
        ForceDropdownValue(_presetVm.SelectedIndex);
    }

    private void RefreshPresetDropdownOptions()
    {
        if (presetDropdown == null || _presetVm == null) return;

        _suppressDropdownCallback = true;

        presetDropdown.ClearOptions();
        var names = _presetVm.Presets
            .Select((p, i) => string.IsNullOrWhiteSpace(p.Title) ? $"Room1 Contents {i + 1}" : p.Title)
            .ToList();

        if (names.Count == 0)
            names.Add("(No Contents)");

        presetDropdown.AddOptions(names);
        presetDropdown.SetValueWithoutNotify(_presetVm.SelectedIndex);
        presetDropdown.RefreshShownValue();

        _suppressDropdownCallback = false;
    }

    private void HandleLineAdded(Room1AddressLineViewModel vm)
    {
        var view = Instantiate(linePrefab, contentParent);
        view.Bind(vm);
        _lineViews[vm] = view;
    }

    private void HandleLineRemoved(Room1AddressLineViewModel vm)
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

    public List<Room1AddressLine> GetCurrentRoom1AddressLines()
    {
        CommitIndex(_lastShownIndex);
        return GameManager.Instance.GetSelectedRoom1ContentsAddressLines();
    }

    public List<Room1AddressPreset> GetCurrentRoom1AddressPresets()
    {
        CommitIndex(_lastShownIndex);
        return _presetVm != null ? _presetVm.ToModelList() : new List<Room1AddressPreset>();
    }
}



