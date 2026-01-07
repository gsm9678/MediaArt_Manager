using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentsAddressLineView : MonoBehaviour
{

    [SerializeField] private TMP_InputField numInput;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField videoAddressInput;
    [SerializeField] private TMP_InputField sensorAddressInput;
    [SerializeField] private TMP_InputField ContentsTimeInput;
    [SerializeField] private TMP_InputField InteractiveTimeInput;
    [SerializeField] private Button deleteButton;

    private ContentsAddressLineViewModel _viewModel;
    private bool _updatingFromViewModel;

    public void Bind(ContentsAddressLineViewModel vm)
    {
        if (_viewModel != null)
            Unbind();

        _viewModel = vm;

        // VM 이벤트 구독
        _viewModel.OnUpdated += RefreshView;

        // UI → VM
        numInput.onValueChanged.AddListener(OnNumChanged);
        nameInput.onValueChanged.AddListener(OnNameChanged);
        videoAddressInput.onValueChanged.AddListener(OnVideoAddressChanged);
        sensorAddressInput.onValueChanged.AddListener(OnSensorAddressChanged);
        ContentsTimeInput.onValueChanged.AddListener(OnContentsTimeChanged);
        InteractiveTimeInput.onValueChanged.AddListener(OnInteractiveTimeChanged);
        deleteButton.onClick.AddListener(OnDeleteClicked);

        // 초기 값 반영
        RefreshView();
    }

    private void Unbind()
    {
        if (_viewModel == null) return;

        _viewModel.OnUpdated -= RefreshView;

        numInput.onValueChanged.RemoveListener(OnNumChanged);
        nameInput.onValueChanged.RemoveListener(OnNameChanged);
        videoAddressInput.onValueChanged.RemoveListener(OnVideoAddressChanged);
        sensorAddressInput.onValueChanged.RemoveListener(OnSensorAddressChanged);
        ContentsTimeInput.onValueChanged.RemoveListener(OnContentsTimeChanged);
        InteractiveTimeInput.onValueChanged.RemoveListener(OnInteractiveTimeChanged);
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

        numInput.text = _viewModel.Num.ToString();
        nameInput.text = _viewModel.Name;
        videoAddressInput.text = _viewModel.VideoAddress;
        sensorAddressInput.text = _viewModel.SensorAddress;
        ContentsTimeInput.text = _viewModel.ContentsTime.ToString();
        InteractiveTimeInput.text = _viewModel.InteractiveTime.ToString();

        _updatingFromViewModel = false;
    }

    // View → VM
    private void OnNumChanged(string value)
    {
        if (_updatingFromViewModel) return;

        if (int.TryParse(value, out int time))
            _viewModel.Num = time;
    }

    private void OnNameChanged(string value)
    {
        if (_updatingFromViewModel) return;
        _viewModel.Name = value;
    }

    private void OnVideoAddressChanged(string value)
    {
        if (_updatingFromViewModel) return;
        _viewModel.VideoAddress = value;
    }

    private void OnSensorAddressChanged(string value)
    {
        if (_updatingFromViewModel) return;
        _viewModel.SensorAddress = value;
    }

    private void OnContentsTimeChanged(string value)
    {
        if (_updatingFromViewModel) return;

        if (float.TryParse(value, out float time))
            _viewModel.ContentsTime = time;
    }

    private void OnInteractiveTimeChanged(string value)
    {
        if (_updatingFromViewModel) return;

        if (float.TryParse(value, out float time))
            _viewModel.InteractiveTime = time;
    }

    private void OnDeleteClicked()
    {
        _viewModel.RequestDelete();
    }
}
