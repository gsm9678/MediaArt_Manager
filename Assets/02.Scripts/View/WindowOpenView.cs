using UnityEngine;
using UnityEngine.UI;

public class WindowOpenView : MonoBehaviour
{
    [Header("Content Management")]
    [SerializeField] private Button ContentManagementWindowButton;
    [SerializeField] private GameObject ContentManagementWindow;

    [Header("Device Management")]
    [SerializeField] private Button DeviceManagementWindowButton;
    [SerializeField] private GameObject DeviceManagementWindow;

    private void Awake()
    {
        ContentManagementWindowButton.onClick.AddListener(ContentManagementWindowOpen);
        DeviceManagementWindowButton.onClick.AddListener(DeviceManagementWindowOpen);
    }

    private void Start()
    {
        ContentManagementWindowOpen();
    }

    private void OnDestroy()
    {
        ContentManagementWindowButton.onClick?.RemoveListener(ContentManagementWindowOpen);
        DeviceManagementWindowButton.onClick?.RemoveListener(DeviceManagementWindowOpen);
    }

    private void ContentManagementWindowOpen()
    {
        ContentManagementWindow.SetActive(true);
        DeviceManagementWindow.SetActive(false);
    }

    private void DeviceManagementWindowOpen()
    {
        ContentManagementWindow.SetActive(false);
        DeviceManagementWindow.SetActive(true);
    }
}
