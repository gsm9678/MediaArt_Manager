using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevicePanelView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private DeviceLineView linePrefab;
    [SerializeField] private Transform contentParent;   // ScrollView Content
    [SerializeField] private Button addButton;

    [Header("Device Line Type")]
    [SerializeField] private DeviceType type;

    private DeviceConfigViewModel _viewModel;
    private readonly Dictionary<DeviceLineViewModel, DeviceLineView> _lineViews = new();

    private void Start()
    {
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        _viewModel = new DeviceConfigViewModel();

        _viewModel.LineAdded += HandleLineAdded;
        _viewModel.LineRemoved += HandleLineRemoved;
        _viewModel.initOscConfigViewModel(GameManager.Instance.DeviceLineDictionary[type]);

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

    private void HandleLineAdded(DeviceLineViewModel vm)
    {
        var view = Instantiate(linePrefab, contentParent);
        view.Bind(vm);
        _lineViews[vm] = view;
    }

    private void HandleLineRemoved(DeviceLineViewModel vm)
    {
        if (_lineViews.TryGetValue(vm, out var view))
        {
            Destroy(view.gameObject);
            _lineViews.Remove(vm);
        }
    }

    // 필요하면 외부에서 현재 설정값을 Model 리스트로 뽑기
    public List<DeviceLine> GetCurrentOscLines()
    {
        return _viewModel.ToModelList();
    }
}
