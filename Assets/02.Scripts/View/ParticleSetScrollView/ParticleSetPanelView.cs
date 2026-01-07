using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticleSetPanelView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private ParticleSetLineView linePrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private Button addButton;

    private ParticleSetConfigViewModel _viewModel;
    private readonly Dictionary<ParticleSetLineViewModel, ParticleSetLineView> _lineViews = new();

    private void Awake()
    {
        addButton.onClick.AddListener(OnAddClicked);
    }

    private void OnDestroy()
    {
        addButton.onClick.RemoveListener(OnAddClicked);

        if (_viewModel != null)
        {
            _viewModel.LineAdded -= HandleLineAdded;
            _viewModel.LineRemoved -= HandleLineRemoved;
        }
    }

    public void SetLines(List<ParticleSetLine> lines)
    {
        ClearAllLineViews();

        _viewModel = new ParticleSetConfigViewModel();
        _viewModel.LineAdded += HandleLineAdded;
        _viewModel.LineRemoved += HandleLineRemoved;
        _viewModel.initOscConfigViewModel(lines ?? new List<ParticleSetLine>());
    }

    public List<ParticleSetLine> GetLines()
    {
        return _viewModel != null ? _viewModel.ToModelList() : new List<ParticleSetLine>();
    }

    private void OnAddClicked()
    {
        _viewModel?.AddEmptyLine();
    }

    private void HandleLineAdded(ParticleSetLineViewModel vm)
    {
        var view = Instantiate(linePrefab, contentParent);
        view.Bind(vm);
        _lineViews[vm] = view;
    }

    private void HandleLineRemoved(ParticleSetLineViewModel vm)
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
            if (kv.Value != null) Destroy(kv.Value.gameObject);
        }
        _lineViews.Clear();

        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);
    }
}