using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PCDeviceLineView : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField macInput;
    [SerializeField] private Button OnButton;
    [SerializeField] private Button OffButton;
    [SerializeField] private Button deleteButton;

    private PCDeviceLineViewModel _viewModel;
    private bool _updatingFromViewModel;

    public void Bind(PCDeviceLineViewModel vm)
    {
        if (_viewModel != null)
            Unbind();

        _viewModel = vm;

        // VM 이벤트 구독
        _viewModel.OnUpdated += RefreshView;

        // UI → VM
        nameInput.onValueChanged.AddListener(OnNameChanged);
        ipInput.onValueChanged.AddListener(OnIpChanged);
        macInput.onValueChanged.AddListener(OnMacChanged);
        OnButton.onClick.AddListener(OnClicked);
        OffButton.onClick.AddListener(OffClicked);
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
        macInput.onValueChanged.RemoveListener(OnMacChanged);
        OnButton.onClick.RemoveListener(OnClicked);
        OffButton.onClick.RemoveListener(OffClicked);
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
        macInput.text = _viewModel.MacAddress;

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

    private void OnMacChanged(string value)
    {
        if (_updatingFromViewModel) return;
        _viewModel.MacAddress = value;
    }

    private void OnClicked()
    {
        MultiPcRemoteController.Instance.WakeSingle(GameManager.Instance.data.PC_DeviceLines.IndexOf(_viewModel.GetModel()));
    }

    private void OffClicked()
    {
        MultiPcRemoteController.Instance.ShutdownSingle(GameManager.Instance.data.PC_DeviceLines.IndexOf(_viewModel.GetModel()));
    }

    private void OnDeleteClicked()
    {
        _viewModel.RequestDelete();
    }
}
