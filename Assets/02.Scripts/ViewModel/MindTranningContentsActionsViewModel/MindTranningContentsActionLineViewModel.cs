using System;
using System.Collections.Generic;

public class MindTranningContentsActionLineViewModel
{
    public event Action OnUpdated;                    // 이 라인 데이터가 바뀔 때
    public event Action<MindTranningContentsActionLineViewModel> OnDelete;   // 이 라인을 삭제해 달라고 요청할 때

    private ContentsAddressLine _model;

    #region Property

    public string Name
    {
        get => _model.Name;
    }
    #endregion

    public MindTranningContentsActionLineViewModel(ContentsAddressLine model)
    {
        _model = model;
    }

    public ContentsAddressLine GetModel() => _model;

    public void RequestDelete()
    {
        OnDelete?.Invoke(this);
    }
}
