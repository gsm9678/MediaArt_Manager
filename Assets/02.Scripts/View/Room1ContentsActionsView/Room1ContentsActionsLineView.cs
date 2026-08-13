using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Room1ContentsActionsLineView : MonoBehaviour
{
    [SerializeField] private TMP_Text numTxt;
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private Button StartButton;

    public int lineNum;

    private Room1ContentsActionLineViewModel _viewModel;

    public void Bind(Room1ContentsActionLineViewModel vm)
    {
        if (_viewModel != null)
            Unbind();

        EnsureReferences();

        _viewModel = vm;
        _viewModel.OnUpdated += RefreshView;

        if (StartButton != null)
            StartButton.onClick.AddListener(OnStartClicked);

        RefreshView();
    }

    private void Unbind()
    {
        if (_viewModel == null) return;

        _viewModel.OnUpdated -= RefreshView;

        if (StartButton != null)
            StartButton.onClick.RemoveListener(OnStartClicked);

        _viewModel = null;
    }

    private void OnDestroy()
    {
        Unbind();
    }

    private void EnsureReferences()
    {
        if (numTxt == null)
            numTxt = transform.Find("Num/Text (TMP)")?.GetComponent<TMP_Text>();

        if (nameTxt == null)
            nameTxt = transform.Find("Title/Text (TMP)")?.GetComponent<TMP_Text>();

        if (StartButton == null)
            StartButton = transform.Find("Button")?.GetComponent<Button>();
    }

    private void RefreshView()
    {
        if (_viewModel == null) return;

        if (numTxt != null)
            numTxt.text = lineNum.ToString();

        if (nameTxt != null)
            nameTxt.text = _viewModel.Name;
    }

    private void OnStartClicked()
    {
        GameManager.Instance.Room1SoloContentsAction?.Invoke(lineNum);
    }
}
