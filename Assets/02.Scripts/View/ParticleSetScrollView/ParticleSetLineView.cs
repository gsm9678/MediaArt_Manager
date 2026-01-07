using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ParticleSetLineView : MonoBehaviour
{
    [SerializeField] private TMP_InputField numInput;
    [SerializeField] private TMP_InputField particleTimeInput;
    [SerializeField] private Button deleteButton;

    private ParticleSetLineViewModel _viewModel;
    private bool _updatingFromViewModel;

    public void Bind(ParticleSetLineViewModel vm)
    {
        if (_viewModel != null) Unbind();

        _viewModel = vm;

        _viewModel.OnUpdated += RefreshView;

        numInput.onValueChanged.AddListener(OnNumChanged);
        particleTimeInput.onValueChanged.AddListener(OnParticleTimeChanged);
        deleteButton.onClick.AddListener(OnDeleteClicked);

        RefreshView();
    }

    private void Unbind()
    {
        if (_viewModel == null) return;

        _viewModel.OnUpdated -= RefreshView;

        numInput.onValueChanged.RemoveListener(OnNumChanged);
        particleTimeInput.onValueChanged.RemoveListener(OnParticleTimeChanged);
        deleteButton.onClick.RemoveListener(OnDeleteClicked);

        _viewModel = null;
    }

    private void OnDestroy() => Unbind();

    private void RefreshView()
    {
        if (_viewModel == null) return;

        _updatingFromViewModel = true;
        numInput.text = _viewModel.Num.ToString();
        particleTimeInput.text = _viewModel.Time.ToString();
        _updatingFromViewModel = false;
    }

    private void OnNumChanged(string value)
    {
        if (_updatingFromViewModel) return;
        if (int.TryParse(value, out int num))
            _viewModel.Num = num;
    }

    private void OnParticleTimeChanged(string value)
    {
        if (_updatingFromViewModel) return;
        if (float.TryParse(value, out float time))
            _viewModel.Time = time;
    }

    private void OnDeleteClicked() => _viewModel.RequestDelete();
}