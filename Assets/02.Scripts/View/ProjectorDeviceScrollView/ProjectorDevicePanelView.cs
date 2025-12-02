using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectorDevicePanelView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private ProjectorDeviceLineView linePrefab;
    [SerializeField] private Transform contentParent;   // ScrollView Content
    [SerializeField] private Button addButton;

    private ProjectorDeviceConfigViewModel _viewModel;
    private readonly Dictionary<ProjectorDeviceLineViewModel, ProjectorDeviceLineView> _lineViews = new();

    private void Start()
    {
        StartCoroutine(StartRoutine());
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitUntil(() =>
            GameManager.Instance.is_JsonLoad);

        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        _viewModel = new ProjectorDeviceConfigViewModel();

        _viewModel.LineAdded += HandleLineAdded;
        _viewModel.LineRemoved += HandleLineRemoved;
        _viewModel.initOscConfigViewModel(GameManager.Instance.data.Projector_DeviceLines);

        addButton.onClick.AddListener(OnAddClicked);

        GameManager.Instance.GetProjectorDeviceLine += GetCurrentProjectorDeviceLines;
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

    private void HandleLineAdded(ProjectorDeviceLineViewModel vm)
    {
        var view = Instantiate(linePrefab, contentParent);
        view.Bind(vm);
        _lineViews[vm] = view;
    }

    private void HandleLineRemoved(ProjectorDeviceLineViewModel vm)
    {
        if (_lineViews.TryGetValue(vm, out var view))
        {
            Destroy(view.gameObject);
            _lineViews.Remove(vm);
        }
    }

    // 필요하면 외부에서 현재 설정값을 Model 리스트로 뽑기
    public List<ProjectorDeviceLine> GetCurrentProjectorDeviceLines()
    {
        return _viewModel.ToModelList();
    }
}
