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

    [Header("Mind Tranning Setting")]
    [SerializeField] private Button MindTranningSettingWindowButton;
    [SerializeField] private GameObject MindTranningSettingWindow;

    [Header("Room 1 Setting")]
    [SerializeField] private Button Room1SettingWindowButton;
    [SerializeField] private GameObject Room1SettingWindow;

    [Header("MediaArtSetting")]
    [SerializeField] private Button MediaArtSettingWindowButton;
    [SerializeField] private GameObject MediaArtSettingWindow;

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
        MindTranningSettingWindowButton.onClick.AddListener(MindTranningWindowOpwn);
        Room1SettingWindowButton.onClick.AddListener(Room1WindowOpen);
        MediaArtSettingWindowButton.onClick.AddListener(MediaArtWindowOpen);
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
        MindTranningSettingWindowButton.onClick.RemoveListener(MindTranningWindowOpwn);
        Room1SettingWindowButton.onClick.RemoveListener(Room1WindowOpen);
        MediaArtSettingWindowButton.onClick.RemoveListener(MediaArtWindowOpen);
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

    private void MindTranningWindowOpwn()
    {
        MindTranningSettingWindow.SetActive(true);
        Room1SettingWindow.SetActive(false);
        MediaArtSettingWindow.SetActive(false);
    }

    private void Room1WindowOpen()
    {
        MindTranningSettingWindow.SetActive(false);
        Room1SettingWindow.SetActive(true);
        MediaArtSettingWindow.SetActive(false);
    }

    private void MediaArtWindowOpen()
    {
        MindTranningSettingWindow.SetActive(false);
        Room1SettingWindow.SetActive(false);
        MediaArtSettingWindow.SetActive(true);
    }
}
