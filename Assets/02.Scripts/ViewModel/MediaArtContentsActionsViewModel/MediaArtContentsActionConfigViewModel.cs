using System;
using System.Collections.Generic;
using System.Linq;

public class MediaArtContentsActionConfigViewModel
{
    private readonly List<MediaArtContentsActionLineViewModel> _lines = new();

    public IReadOnlyList<MediaArtContentsActionLineViewModel> Lines => _lines;

    public event Action<MediaArtContentsActionLineViewModel> LineAdded;
    public event Action<MediaArtContentsActionLineViewModel> LineRemoved;

    public void initOscConfigViewModel(IEnumerable<ParticleSetPreset> initialLines = null)
    {
        if (initialLines == null) return;

        foreach (var m in initialLines)
        {
            AddLineFromModel(m);
        }
    }

    private MediaArtContentsActionLineViewModel AddLineFromModel(ParticleSetPreset model)
    {
        var vm = new MediaArtContentsActionLineViewModel(model);
        vm.OnDelete += HandleLineDeleteRequest;

        _lines.Add(vm);
        LineAdded?.Invoke(vm);
        return vm;
    }

    public MediaArtContentsActionLineViewModel AddEmptyLine()
    {
        var model = new ParticleSetPreset( "", "", 0,  null);
        return AddLineFromModel(model);
    }

    private void HandleLineDeleteRequest(MediaArtContentsActionLineViewModel vm)
    {
        RemoveLine(vm);
    }

    public void RemoveLine(MediaArtContentsActionLineViewModel vm)
    {
        if (!_lines.Remove(vm)) return;

        vm.OnDelete -= HandleLineDeleteRequest;
        LineRemoved?.Invoke(vm);
    }

    public List<ParticleSetPreset> ToModelList()
    {
        return _lines.Select(l => l.GetModel()).ToList();
    }
}
