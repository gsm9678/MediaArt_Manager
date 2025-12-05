using UnityEngine;
using UnityEngine.UI;

public class ButtonsMapper : MonoBehaviour
{
    [SerializeField] private Button ContentStartButton;
    [SerializeField] private Button ContentStopButton;
    [SerializeField] private Button DeviceOnButton;
    [SerializeField] private Button DeviceOffButton;

    private void Awake()
    {
        ContentStartButton.onClick.AddListener(() => GameManager.Instance.ContentsStartAction?.Invoke(0));
        ContentStopButton.onClick.AddListener(() => GameManager.Instance.ContentsStopAction?.Invoke());
        DeviceOnButton.onClick.AddListener(() => GameManager.Instance.DeviceOnAction?.Invoke());
        DeviceOffButton.onClick.AddListener(() => GameManager.Instance.DeviceOffAction?.Invoke());
    }

    private void OnDestroy()
    {
        ContentStartButton.onClick.RemoveListener(() => GameManager.Instance.ContentsStartAction?.Invoke(0));
        ContentStopButton.onClick.RemoveListener(() => GameManager.Instance.ContentsStopAction?.Invoke());
        DeviceOnButton.onClick.RemoveListener(() => GameManager.Instance.DeviceOnAction?.Invoke());
        DeviceOffButton.onClick.RemoveListener(() => GameManager.Instance.DeviceOffAction?.Invoke());
    }
}