using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeviceLineView : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private Button OnButton;
    [SerializeField] private Button OffButton;
    [SerializeField] private Button deleteButton;

    private DeviceLineViewModel _viewModel;
    private bool _updatingFromViewModel;

    public void Bind(DeviceLineViewModel vm)
    {
        if (_viewModel != null)
            Unbind();

        _viewModel = vm;

        // VM 이벤트 구독
        _viewModel.OnUpdated += RefreshView;

        // UI → VM
        nameInput.onValueChanged.AddListener(OnNameChanged);
        ipInput.onValueChanged.AddListener(OnIpChanged);
        deleteButton.onClick.AddListener(OnDeleteClicked);

        // 초기 값 반영
        RefreshView();
    }

    private void Unbind()
    {
        if (_viewModel == null) return;

        _viewModel.OnUpdated -= RefreshView;

        nameInput.onValueChanged.RemoveListener(OnNameChanged);
        ipInput.onValueChanged.RemoveListener(OnIpChanged);
        deleteButton.onClick.RemoveListener(OnDeleteClicked);

        _viewModel = null;
    }

    private void OnDestroy()
    {
        Unbind();
    }

    // VM → View
    private void RefreshView()
    {
        if (_viewModel == null) return;

        _updatingFromViewModel = true;

        nameInput.text = _viewModel.Name;
        ipInput.text = _viewModel.IpAddress;

        _updatingFromViewModel = false;
    }

    // View → VM
    private void OnNameChanged(string value)
    {
        if (_updatingFromViewModel) return;
        _viewModel.Name = value;
    }

    private void OnIpChanged(string value)
    {
        if (_updatingFromViewModel) return;
        _viewModel.IpAddress = value;
    }

    private void OnDeleteClicked()
    {
        _viewModel.RequestDelete();
    }
}
