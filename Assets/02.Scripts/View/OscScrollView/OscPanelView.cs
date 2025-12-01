using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OscPanelView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private OscLineView linePrefab;
    [SerializeField] private Transform contentParent;   // ScrollView Content
    [SerializeField] private Button addButton;

    [Header("Osc Line Type")]
    [SerializeField] private OscLineType type;

    private OscConfigViewModel _viewModel;
    private readonly Dictionary<OscLineViewModel, OscLineView> _lineViews = new();

    private void Start()
    {
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        _viewModel = new OscConfigViewModel();

        _viewModel.LineAdded += HandleLineAdded;
        _viewModel.LineRemoved += HandleLineRemoved;
        _viewModel.initOscConfigViewModel(GameManager.Instance.OscLineDictionary[type]);

        addButton.onClick.AddListener(OnAddClicked);
    }

    private void OnDestroy()
    {
        addButton.onClick.RemoveListener(OnAddClicked);

        _viewModel.LineAdded -= HandleLineAdded;
        _viewModel.LineRemoved -= HandleLineRemoved;
    }

    private void OnAddClicked()
    {
        _viewModel.AddEmptyLine();
    }

    private void HandleLineAdded(OscLineViewModel vm)
    {
        var view = Instantiate(linePrefab, contentParent);
        view.Bind(vm);
        _lineViews[vm] = view;
    }

    private void HandleLineRemoved(OscLineViewModel vm)
    {
        if (_lineViews.TryGetValue(vm, out var view))
        {
            Destroy(view.gameObject);
            GameManager.Instance.OscLineDictionary[type].Remove(vm.GetModel());
            _lineViews.Remove(vm);
        }
    }

    // 필요하면 외부에서 현재 설정값을 Model 리스트로 뽑기
    public List<OscLine> GetCurrentOscLines()
    {
        return _viewModel.ToModelList();
    }
}