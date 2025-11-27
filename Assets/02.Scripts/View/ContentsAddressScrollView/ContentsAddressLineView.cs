using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentsAddressLineView : MonoBehaviour
{

    [SerializeField] private TMP_InputField numInput;
    [SerializeField] private TMP_InputField addressInput;
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
        addressInput.onValueChanged.AddListener(OnAddressChanged);
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
        addressInput.onValueChanged.RemoveListener(OnAddressChanged);
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
        addressInput.text = _viewModel.Address;
        timeInput.text = _viewModel.Time.ToString();

        _updatingFromViewModel = false;
    }

    // View → VM
    private void OnNumChanged(string value)
    {
        if (_updatingFromViewModel) return;
        _viewModel.Num = value;
    }

    private void OnAddressChanged(string value)
    {
        if (_updatingFromViewModel) return;
        _viewModel.Address = value;
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
