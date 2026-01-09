using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MindTranningContentsActionsLineView : MonoBehaviour
{
    [SerializeField] private TMP_Text numTxt;
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private Button StartButton;

    public int lineNum;

    private MindTranningContentsActionLineViewModel _viewModel;
    private bool _updatingFromViewModel;

    public void Bind(MindTranningContentsActionLineViewModel vm)
    {
        if (_viewModel != null)
            Unbind();

        _viewModel = vm;

        // VM 이벤트 구독
        _viewModel.OnUpdated += RefreshView;

        // UI → VM
        StartButton.onClick.AddListener (() => GameManager.Instance.ContentsStartAction?.Invoke(lineNum));

        // 초기 값 반영
        RefreshView();
    }

    private void Unbind()
    {
        if (_viewModel == null) return;

        _viewModel.OnUpdated -= RefreshView;

        StartButton.onClick.RemoveListener(OnDeleteClicked);

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

        numTxt.text = lineNum.ToString();
        nameTxt.text = _viewModel.Name;

        _updatingFromViewModel = false;
    }

    private void OnDeleteClicked()
    {
        _viewModel.RequestDelete();
    }
}
