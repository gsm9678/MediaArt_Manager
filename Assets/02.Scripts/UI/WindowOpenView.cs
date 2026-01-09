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

    [Header("All Off Button Check Panel")]
    [SerializeField] private Button AllOffButton;
    [SerializeField] private Button AllOffYesButton;
    [SerializeField] private Button AllOffCancelButton;
    [SerializeField] private GameObject AllOffPanel;

    [Header("All Off Button Check Panel")]
    [SerializeField] private Button AllOnButton;
    [SerializeField] private Button AllOnYesButton;
    [SerializeField] private Button AllOnCancelButton;
    [SerializeField] private GameObject AllOnPanel;

    private void Awake()
    {
        ContentActionWindowButton.onClick.AddListener(ContentActionWindowOpen);
        ContentManagementWindowButton.onClick.AddListener(ContentManagementWindowOpen);
        DeviceManagementWindowButton.onClick.AddListener(DeviceManagementWindowOpen);
        AllOffButton.onClick.AddListener(() => AllOffPanel.SetActive(true));
        AllOffYesButton.onClick.AddListener(() => AllOffPanel.SetActive(false));
        AllOffCancelButton.onClick.AddListener(() => AllOffPanel.SetActive(false));
        AllOnButton.onClick.AddListener(() => AllOnPanel.SetActive(true));
        AllOnYesButton.onClick.AddListener(() => AllOnPanel.SetActive(false));
        AllOnCancelButton.onClick.AddListener(() => AllOnPanel.SetActive(false));
    }

    private void Start()
    {
        ContentActionWindowOpen();
    }

    private void OnDestroy()
    {
        ContentActionWindowButton.onClick.RemoveListener(ContentActionWindowOpen);
        ContentManagementWindowButton.onClick.RemoveListener(ContentManagementWindowOpen);
        DeviceManagementWindowButton.onClick.RemoveListener(DeviceManagementWindowOpen);
        AllOffButton.onClick.RemoveListener(() => AllOffPanel.SetActive(true));
        AllOffYesButton.onClick.RemoveListener(() => AllOffPanel.SetActive(false));
        AllOffCancelButton.onClick.RemoveListener(() => AllOffPanel.SetActive(false));
        AllOnButton.onClick.RemoveListener(() => AllOnPanel.SetActive(true));
        AllOnYesButton.onClick.RemoveListener(() => AllOnPanel.SetActive(false));
        AllOnCancelButton.onClick.RemoveListener(() => AllOnPanel.SetActive(false));
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
