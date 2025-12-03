using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentsAddressLineView : MonoBehaviour
{

    [SerializeField] private TMP_InputField numInput;
    [SerializeField] private TMP_InputField videoAddressInput;
    [SerializeField] private TMP_InputField sensorAddressInput;
    [SerializeField] private TMP_InputField audioAddressInput;
    [SerializeField] private TMP_InputField timeInput;
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
        videoAddressInput.onValueChanged.AddListener(OnVideoAddressChanged);
        sensorAddressInput.onValueChanged.AddListener(OnSensorAddressChanged);
        audioAddressInput.onValueChanged.AddListener(OnAudioAddressChanged);
        timeInput.onValueChanged.AddListener(OnTimeChanged);
        deleteButton.onClick.AddListener(OnDeleteClicked);

        // 초기 값 반영
        RefreshView();
    }

    private void Unbind()
    {
        if (_viewModel == null) return;

        _viewModel.OnUpdated -= RefreshView;

        numInput.onValueChanged.RemoveListener(OnNumChanged);
        videoAddressInput.onValueChanged.RemoveListener(OnVideoAddressChanged);
        sensorAddressInput.onValueChanged.RemoveListener(OnSensorAddressChanged);
        audioAddressInput.onValueChanged.RemoveListener(OnAudioAddressChanged);
        timeInput.onValueChanged.RemoveListener(OnTimeChanged);
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

        numInput.text = _viewModel.Num;
        videoAddressInput.text = _viewModel.VideoAddress;
        sensorAddressInput.text = _viewModel.SensorAddress;
        audioAddressInput.text = _viewModel.AudioAddress;
        timeInput.text = _viewModel.Time.ToString();

        _updatingFromViewModel = false;
    }

    // View → VM
    private void OnNumChanged(string value)
    {
        if (_updatingFromViewModel) return;
        _viewModel.Num = value;
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

    private void OnAudioAddressChanged(string value)
    {
        if (_updatingFromViewModel) return;
        _viewModel.AudioAddress = value;
    }

    private void OnTimeChanged(string value)
    {
        if (_updatingFromViewModel) return;

        if (int.TryParse(value, out int port))
            _viewModel.Time = port;
    }

    private void OnDeleteClicked()
    {
        _viewModel.RequestDelete();
    }
}
