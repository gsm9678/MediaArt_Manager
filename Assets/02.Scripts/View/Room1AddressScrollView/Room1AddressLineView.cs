using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Room1AddressLineView : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField videoAddressInput;
    [SerializeField] private TMP_InputField ContentsTimeInput;
    [SerializeField] private Button deleteButton;

    private Room1AddressLineViewModel _viewModel;
    private bool _updatingFromViewModel;

    public void Bind(Room1AddressLineViewModel vm)
    {
        if (_viewModel != null)
            Unbind();

        _viewModel = vm;

        // VM 이벤트 구독
        _viewModel.OnUpdated += RefreshView;

        // UI → VM
        nameInput.onValueChanged.AddListener(OnNameChanged);
        videoAddressInput.onValueChanged.AddListener(OnVideoAddressChanged);
        ContentsTimeInput.onValueChanged.AddListener(OnContentsTimeChanged);
        deleteButton.onClick.AddListener(OnDeleteClicked);

        // 초기 값 반영
        RefreshView();
    }

    private void Unbind()
    {
        if (_viewModel == null) return;

        _viewModel.OnUpdated -= RefreshView;

        nameInput.onValueChanged.RemoveListener(OnNameChanged);
        videoAddressInput.onValueChanged.RemoveListener(OnVideoAddressChanged);
        ContentsTimeInput.onValueChanged.RemoveListener(OnContentsTimeChanged);
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
        videoAddressInput.text = _viewModel.VideoAddress;
        ContentsTimeInput.text = _viewModel.ContentsTime.ToString();

        _updatingFromViewModel = false;
    }

    // View → VM

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

    private void OnContentsTimeChanged(string value)
    {
        if (_updatingFromViewModel) return;

        if (float.TryParse(value, out float time))
            _viewModel.ContentsTime = time;
    }

    private void OnDeleteClicked()
    {
        _viewModel.RequestDelete();
    }
}

