using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediaArtContentsActionsPanelView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private MediaArtContentsActionsLineView linePrefab;
    [SerializeField] private Transform contentParent;   // ScrollView Content

    private MediaArtContentsActionConfigViewModel _viewModel;
    private readonly Dictionary<MediaArtContentsActionLineViewModel, MediaArtContentsActionsLineView> _lineViews = new();

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

        _viewModel = new MediaArtContentsActionConfigViewModel();

        _viewModel.LineAdded += HandleLineAdded;
        _viewModel.LineRemoved += HandleLineRemoved;
        _viewModel.initOscConfigViewModel(GameManager.Instance.data.ParticleSetPresets);
    }

    private void OnDestroy()
    {
        _viewModel.LineAdded -= HandleLineAdded;
        _viewModel.LineRemoved -= HandleLineRemoved;
    }

    private void HandleLineAdded(MediaArtContentsActionLineViewModel vm)
    {
        var view = Instantiate(linePrefab, contentParent);
        view.lineNum = _lineViews.Count;
        view.Bind(vm);
        _lineViews[vm] = view;
    }

    private void HandleLineRemoved(MediaArtContentsActionLineViewModel vm)
    {
        if (_lineViews.TryGetValue(vm, out var view))
        {
            Destroy(view.gameObject);
            _lineViews.Remove(vm);
        }
    }
}
