using UnityEngine;
using UnityEngine.UI;

public class ButtonsMapper : MonoBehaviour
{
    [SerializeField] private Button ContentStartButton;
    [SerializeField] private Button MediaArtStartButton;
    [SerializeField] private Button ResumeButton;
    [SerializeField] private Button PauseButton;
    [SerializeField] private Button StopButton;
    [SerializeField] private Button DeviceOnButton;
    [SerializeField] private Button DeviceOffButton;

    private void Awake()
    {
        ContentStartButton.onClick.AddListener(() => GameManager.Instance.ContentsStartAction?.Invoke(0));
        MediaArtStartButton.onClick.AddListener(() => GameManager.Instance.MediaArtStartAction?.Invoke(0));
        ResumeButton.onClick.AddListener(() => GameManager.Instance.ResumeAction?.Invoke());
        PauseButton.onClick.AddListener(() => GameManager.Instance.PauseAction?.Invoke());
        StopButton.onClick.AddListener(() => GameManager.Instance.StopAction?.Invoke());
        DeviceOnButton.onClick.AddListener(() => GameManager.Instance.DeviceOnAction?.Invoke());
        DeviceOffButton.onClick.AddListener(() => GameManager.Instance.DeviceOffAction?.Invoke());
    }

    private void OnDestroy()
    {
        ContentStartButton.onClick.RemoveListener(() => GameManager.Instance.ContentsStartAction?.Invoke(0));
        MediaArtStartButton.onClick.RemoveListener(() => GameManager.Instance.MediaArtStartAction?.Invoke(0));
        ResumeButton.onClick.RemoveListener(() => GameManager.Instance.ResumeAction?.Invoke());
        PauseButton.onClick.RemoveListener(() => GameManager.Instance.PauseAction?.Invoke());
        StopButton.onClick.RemoveListener(() => GameManager.Instance.StopAction?.Invoke());
        DeviceOnButton.onClick.RemoveListener(() => GameManager.Instance.DeviceOnAction?.Invoke());
        DeviceOffButton.onClick.RemoveListener(() => GameManager.Instance.DeviceOffAction?.Invoke());
    }
}