using TMPro;
using UnityEngine;

public class ManagerDropdownSyncView : MonoBehaviour
{
    private enum DropdownType { ContentsPreset, Room1Preset, ParticlePreset }
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private DropdownType dropdownType = DropdownType.ContentsPreset;
    private bool isUpdating;

    private void Start()
    {
        if (dropdown == null) dropdown = GetComponent<TMP_Dropdown>();
        ResolveDropdownTypeFromPath();
        Subscribe();
        if (dropdown != null) dropdown.onValueChanged.AddListener(OnValueChanged);
        ApplySelectedIndex(GetCurrentIndex());
    }

    private void OnDestroy()
    {
        Unsubscribe();
        if (dropdown != null) dropdown.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void Subscribe()
    {
        if (GameManager.Instance == null) return;
        switch (dropdownType)
        {
            case DropdownType.ContentsPreset: GameManager.Instance.SetContentsPresetDropdownValueAction += ApplySelectedIndex; break;
            case DropdownType.Room1Preset: GameManager.Instance.SetRoom1PresetDropdownValueAction += ApplySelectedIndex; break;
            case DropdownType.ParticlePreset: GameManager.Instance.SetParticlePresetDropdownValueAction += ApplySelectedIndex; break;
        }
    }

    private void Unsubscribe()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.SetContentsPresetDropdownValueAction -= ApplySelectedIndex;
        GameManager.Instance.SetRoom1PresetDropdownValueAction -= ApplySelectedIndex;
        GameManager.Instance.SetParticlePresetDropdownValueAction -= ApplySelectedIndex;
    }

    private void OnValueChanged(int index)
    {
        if (isUpdating || GameManager.Instance == null) return;
        int nextIndex = Mathf.Max(0, index);
        switch (dropdownType)
        {
            case DropdownType.ContentsPreset:
                GameManager.Instance.SelectedContentsAddressPresetIndex = nextIndex;
                GameManager.Instance.SetContentsPresetDropdownValueAction?.Invoke(nextIndex);
                break;
            case DropdownType.Room1Preset:
                GameManager.Instance.SelectedRoom1ContentsAddressPresetIndex = nextIndex;
                GameManager.Instance.SetRoom1PresetDropdownValueAction?.Invoke(nextIndex);
                break;
            case DropdownType.ParticlePreset:
                GameManager.Instance.SelectedParticleSetPresetIndex = nextIndex;
                GameManager.Instance.SetParticlePresetDropdownValueAction?.Invoke(nextIndex);
                break;
        }
        Debug.Log($"[ManagerDropdownSyncView] {dropdownType} selected={nextIndex}");
        OSCManager.Instance?.SendRemoteDataSet();
    }

    private void ApplySelectedIndex(int index)
    {
        if (dropdown == null) return;
        int nextValue = dropdown.options.Count > 0 ? Mathf.Clamp(index, 0, dropdown.options.Count - 1) : 0;
        isUpdating = true;
        if (dropdown.value != nextValue)
            dropdown.value = nextValue;
        else
            dropdown.onValueChanged.Invoke(nextValue);
        dropdown.RefreshShownValue();
        isUpdating = false;
    }

    private int GetCurrentIndex()
    {
        if (GameManager.Instance == null) return 0;
        switch (dropdownType)
        {
            case DropdownType.ContentsPreset: return GameManager.Instance.SelectedContentsAddressPresetIndex;
            case DropdownType.Room1Preset: return GameManager.Instance.SelectedRoom1ContentsAddressPresetIndex;
            case DropdownType.ParticlePreset: return GameManager.Instance.SelectedParticleSetPresetIndex;
            default: return 0;
        }
    }

    private void ResolveDropdownTypeFromPath()
    {
        string path = GetTransformPath(transform);
        if (path.Contains("Room1")) dropdownType = DropdownType.Room1Preset;
        else if (path.Contains("MediaArt")) dropdownType = DropdownType.ParticlePreset;
        else dropdownType = DropdownType.ContentsPreset;
    }

    private string GetTransformPath(Transform target)
    {
        string path = target.name;
        while (target.parent != null)
        {
            target = target.parent;
            path = target.name + "/" + path;
        }
        return path;
    }
}
