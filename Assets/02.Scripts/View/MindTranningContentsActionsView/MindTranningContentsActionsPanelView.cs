using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MindTranningContentsActionsPanelView : MonoBehaviour
{
    [Header("Preset Select")]
    [SerializeField] private TMP_Dropdown presetDropdown;
    [SerializeField] private Button startButton;

    [Header("UI References")]
    [SerializeField] private MindTranningContentsActionsLineView linePrefab;
    [SerializeField] private Transform contentParent;

    private MindTranningContentsActionConfigViewModel _viewModel;
    private readonly Dictionary<MindTranningContentsActionLineViewModel, MindTranningContentsActionsLineView> _lineViews = new();

    private List<ContentsAddressPreset> _presets = new();
    private bool _suppressDropdownCallback;

    private void Start()
    {
        StartCoroutine(StartRoutine());
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitUntil(() => GameManager.Instance.is_JsonLoad);

        _presets = GameManager.Instance.data.GetContentsAddressPresets();

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
        SetSelectedPreset(GameManager.Instance.SelectedContentsAddressPresetIndex);
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
            SetLines(new List<ContentsAddressLine>());
            return;
        }

        GameManager.Instance.SelectedContentsAddressPresetIndex = Mathf.Clamp(index, 0, _presets.Count - 1);
        GameManager.Instance.GetSelectedContentsAddressLines();

        if (presetDropdown != null)
        {
            _suppressDropdownCallback = true;
            presetDropdown.SetValueWithoutNotify(GameManager.Instance.SelectedContentsAddressPresetIndex);
            presetDropdown.RefreshShownValue();
            _suppressDropdownCallback = false;
        }

        var selected = _presets[GameManager.Instance.SelectedContentsAddressPresetIndex];
        SetLines(selected.Contents ?? new List<ContentsAddressLine>());
    }

    private void RefreshPresetDropdownOptions()
    {
        if (presetDropdown == null) return;

        _suppressDropdownCallback = true;
        presetDropdown.ClearOptions();

        var names = _presets
            .Select((p, i) => string.IsNullOrWhiteSpace(p.Title) ? $"Contents {i + 1}" : p.Title)
            .ToList();

        if (names.Count == 0)
            names.Add("(No Contents)");

        presetDropdown.AddOptions(names);
        presetDropdown.SetValueWithoutNotify(GameManager.Instance.SelectedContentsAddressPresetIndex);
        presetDropdown.RefreshShownValue();
        _suppressDropdownCallback = false;
    }

    private void SetLines(IEnumerable<ContentsAddressLine> lines)
    {
        ClearAllLineViews();
        UnbindViewModel();

        _viewModel = new MindTranningContentsActionConfigViewModel();
        _viewModel.LineAdded += HandleLineAdded;
        _viewModel.LineRemoved += HandleLineRemoved;
        _viewModel.initOscConfigViewModel(lines);
    }

    private void OnStartClicked()
    {
        GameManager.Instance.ContentsStartAction?.Invoke(0);
    }

    private void HandleLineAdded(MindTranningContentsActionLineViewModel vm)
    {
        var view = Instantiate(linePrefab, contentParent);
        view.lineNum = _lineViews.Count;
        view.Bind(vm);
        _lineViews[vm] = view;
    }

    private void HandleLineRemoved(MindTranningContentsActionLineViewModel vm)
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
        _viewModel = null;
    }
}
