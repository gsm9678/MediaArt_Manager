using UnityEngine;
using UnityEngine.UI;

public class WindowOpenView : MonoBehaviour
{
    [Header("Device Management")]
    [SerializeField] private Button DeviceManagementWindowButton;
    [SerializeField] private GameObject DeviceManagementWindow;

    [Header("Content Management")]
    [SerializeField] private Button ContentManagementWindowButton;
    [SerializeField] private GameObject ContentManagementWindow;

    [Header("Content Action")]
    [SerializeField] private Button ContentActionWindowButton;
    [SerializeField] private GameObject ContentActionWindow;


    private void Awake()
    {
        ContentActionWindowButton.onClick.AddListener(ContentActionWindowOpen);
        ContentManagementWindowButton.onClick.AddListener(ContentManagementWindowOpen);
        DeviceManagementWindowButton.onClick.AddListener(DeviceManagementWindowOpen);
    }

    private void Start()
    {
        ContentActionWindowOpen();
    }

    private void OnDestroy()
    {
        ContentActionWindowButton.onClick?.RemoveListener(ContentActionWindowOpen);
        ContentManagementWindowButton.onClick?.RemoveListener(ContentManagementWindowOpen);
        DeviceManagementWindowButton.onClick?.RemoveListener(DeviceManagementWindowOpen);
    }

    private void DeviceManagementWindowOpen()
    {
        ContentActionWindow.SetActive(false);
        ContentManagementWindow.SetActive(false);
        DeviceManagementWindow.SetActive(true);
    }

    private void ContentManagementWindowOpen()
    {
        ContentActionWindow.SetActive(false);
        ContentManagementWindow.SetActive(true);
        DeviceManagementWindow.SetActive(false);
    }

    private void ContentActionWindowOpen()
    {
        ContentActionWindow.SetActive(true);
        ContentManagementWindow.SetActive(false);
        DeviceManagementWindow.SetActive(false);
    }
}
