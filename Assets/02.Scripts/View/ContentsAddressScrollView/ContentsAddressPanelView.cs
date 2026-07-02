using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentsAddressPanelView : MonoBehaviour
{
    [Header("Preset Select")]
    [SerializeField] private TMP_Dropdown presetDropdown;

    [Header("Preset Edit")]
    [SerializeField] private TMP_InputField titleInput;
    [SerializeField] private Button createButton;
    [SerializeField] private Button deleteButton;

    [Header("UI References")]
    [SerializeField] private ContentsAddressLineView linePrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private Button addButton;

    private ContentsAddressPresetListViewModel _presetVm;
    private ContentsAddressConfigViewModel _viewModel;
    private readonly Dictionary<ContentsAddressLineViewModel, ContentsAddressLineView> _lineViews = new();

    private bool _suppressDropdownCallback;
    private int _lastShownIndex = -1;

    private void Start()
    {
        StartCoroutine(StartRoutine());
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitUntil(() => GameManager.Instance.is_JsonLoad);

        _presetVm = new ContentsAddressPresetListViewModel();
        _presetVm.Init(GameManager.Instance.data.GetContentsAddressPresets());

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

        GameManager.Instance.GetContentsAddressLine += GetCurrentContentsAddressLines;
        GameManager.Instance.GetContentsAddressPresets += GetCurrentContentsAddressPresets;
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
            GameManager.Instance.GetContentsAddressLine -= GetCurrentContentsAddressLines;
            GameManager.Instance.GetContentsAddressPresets -= GetCurrentContentsAddressPresets;
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
        GameManager.Instance.SelectedContentsAddressPresetIndex = _presetVm.SelectedIndex;

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

            SetLines(new List<ContentsAddressLine>());
            _lastShownIndex = -1;
            return;
        }

        if (titleInput != null)
            titleInput.SetTextWithoutNotify(selected.Title);

        SetLines(selected.Contents.Select(line => new ContentsAddressLine(line)).ToList());
        _lastShownIndex = _presetVm.SelectedIndex;
    }

    private void SetLines(List<ContentsAddressLine> lines)
    {
        ClearAllLineViews();
        UnbindViewModel();

        _viewModel = new ContentsAddressConfigViewModel();
        _viewModel.LineAdded += HandleLineAdded;
        _viewModel.LineRemoved += HandleLineRemoved;
        _viewModel.initOscConfigViewModel(lines ?? new List<ContentsAddressLine>());
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

        if (index == GameManager.Instance.SelectedContentsAddressPresetIndex)
            GameManager.Instance.data.ContentsAddressLines = target.Contents.Select(line => new ContentsAddressLine(line)).ToList();
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
            : $"Contents {_presetVm.Presets.Count + 1}";

        _presetVm.AddPreset(new ContentsAddressPreset(title, new List<ContentsAddressLine>()));
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
            .Select((p, i) => string.IsNullOrWhiteSpace(p.Title) ? $"Contents {i + 1}" : p.Title)
            .ToList();

        if (names.Count == 0)
            names.Add("(No Contents)");

        presetDropdown.AddOptions(names);
        presetDropdown.SetValueWithoutNotify(_presetVm.SelectedIndex);
        presetDropdown.RefreshShownValue();

        _suppressDropdownCallback = false;
    }

    private void HandleLineAdded(ContentsAddressLineViewModel vm)
    {
        var view = Instantiate(linePrefab, contentParent);
        view.Bind(vm);
        _lineViews[vm] = view;
    }

    private void HandleLineRemoved(ContentsAddressLineViewModel vm)
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

    public List<ContentsAddressLine> GetCurrentContentsAddressLines()
    {
        CommitIndex(_lastShownIndex);
        return GameManager.Instance.GetSelectedContentsAddressLines();
    }

    public List<ContentsAddressPreset> GetCurrentContentsAddressPresets()
    {
        CommitIndex(_lastShownIndex);
        return _presetVm != null ? _presetVm.ToModelList() : new List<ContentsAddressPreset>();
    }
}
