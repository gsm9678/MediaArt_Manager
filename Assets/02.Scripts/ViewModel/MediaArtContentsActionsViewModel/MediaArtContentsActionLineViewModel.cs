using System;
using System.Collections.Generic;

public class MediaArtContentsActionLineViewModel
{
    public event Action OnUpdated;                    // 이 라인 데이터가 바뀔 때
    public event Action<MediaArtContentsActionLineViewModel> OnDelete;   // 이 라인을 삭제해 달라고 요청할 때

    private ParticleSetPreset _model;

    #region Property

    public string Name
    {
        get => _model.Title;
    }
    #endregion

    public MediaArtContentsActionLineViewModel(ParticleSetPreset model)
    {
        _model = model;
    }

    public ParticleSetPreset GetModel() => _model;

    public void RequestDelete()
    {
        OnDelete?.Invoke(this);
    }
}
