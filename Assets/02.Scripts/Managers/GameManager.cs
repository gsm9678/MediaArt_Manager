using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public JsonDataformat data;

    [HideInInspector] public bool is_JsonLoad = false;
    [HideInInspector] public bool is_ContentsPlayed = false;
    public bool[] is_ContentsCheck = new bool[3];

    public Action<int> ContentsStartAction;
    public Action<int> MediaArtStartAction;
    public Action<int> SoloContentsAction;
    public Action ResumeAction;
    public Action PauseAction;
    public Action StopAction;
    public Action DeviceOnAction;
    public Action DeviceOffAction;

    public Dictionary<OscLineType, List<OscLine>> OscLineDictionary = new();

    public Dictionary<OscLineType, Func<List<OscLine>>> GetOscLine = new();
    public event Func<List<PCDeviceLine>> GetDeviceLine;
    public event Func<List<ProjectorDeviceLine>> GetProjectorDeviceLine;
    public event Func<List<ContentsAddressLine>> GetContentsAddressLine; 
    public event Func<List<ParticleSetPreset>> GetParticleSetPresets;

    public override void Awake()
    {
        base.Awake();
        DeviceOnAction += StartDeviceOnRoutine;
        DeviceOffAction += StartDeviceOffRoutine;
    }

    private void StartDeviceOnRoutine()
    {
        StartCoroutine(DeviceOnRoutine());
    }

    private void StartDeviceOffRoutine()
    {
        StartCoroutine(DeviceOffRoutine());
    }

    private System.Collections.IEnumerator DeviceOnRoutine()
    {
        int pcCount = MultiPcRemoteController.Instance.targets != null ? MultiPcRemoteController.Instance.targets.Count : 0;
        int projCount = EpsonProjectorPjLinkController.Instance.projectors != null ? EpsonProjectorPjLinkController.Instance.projectors.Count : 0;
        
        ProgressOverlayManager.Instance.ShowProgress("기기 전체 켜기 진행 중...", pcCount + projCount);

        for (int i = 0; i < projCount; i++)
        {
            var t = EpsonProjectorPjLinkController.Instance.projectors[i];
            ProgressOverlayManager.Instance.IncrementProgress($"프로젝터 켜는 중... ({t.Name})");
            EpsonProjectorPjLinkController.Instance.PowerOnSingleAsync(i);
            yield return new WaitForSeconds(0.5f);
        }

        for (int i = 0; i < pcCount; i++)
        {
            var t = MultiPcRemoteController.Instance.targets[i];
            ProgressOverlayManager.Instance.IncrementProgress($"PC 켜는 중... ({t.Name})");
            //MultiPcRemoteController.Instance.WakeSingle(i, MultiPcRemoteController.Instance.forceWakeOnBatchEvenIfOnline);
            yield return MultiPcRemoteController.Instance.WakeSingleRoutine(i, MultiPcRemoteController.Instance.forceWakeOnBatchEvenIfOnline);
            yield return new WaitForSeconds(0.2f);
        }
        
        yield return new WaitForSeconds(1f);
        ProgressOverlayManager.Instance.HideProgress();
    }

    private System.Collections.IEnumerator DeviceOffRoutine()
    {
        int pcCount = MultiPcRemoteController.Instance.targets != null ? MultiPcRemoteController.Instance.targets.Count : 0;
        int projCount = EpsonProjectorPjLinkController.Instance.projectors != null ? EpsonProjectorPjLinkController.Instance.projectors.Count : 0;
        
        ProgressOverlayManager.Instance.ShowProgress("기기 전체 종료 진행 중...", pcCount + projCount);

        for (int i = 0; i < pcCount; i++)
        {
            var t = MultiPcRemoteController.Instance.targets[i];
            ProgressOverlayManager.Instance.IncrementProgress($"PC 종료 중... ({t.Name})");
            MultiPcRemoteController.Instance.ShutdownSingleAsync(i);
            yield return new WaitForSeconds(0.5f);
        }

        for (int i = 0; i < projCount; i++)
        {
            var t = EpsonProjectorPjLinkController.Instance.projectors[i];
            ProgressOverlayManager.Instance.IncrementProgress($"프로젝터 종료 중... ({t.Name})");
            EpsonProjectorPjLinkController.Instance.PowerOffSingleAsync(i);
            yield return new WaitForSeconds(0.5f);
        }
        
        yield return new WaitForSeconds(1f);
        ProgressOverlayManager.Instance.HideProgress();
    }

    public void SetOscLineDictionary()
    {
        OscLineDictionary.Add(OscLineType.Video, data.VideoOscLines);
        OscLineDictionary.Add(OscLineType.Sensor, data.SensorOscLines);
    }

    public void SetContentsCheck()
    {
        is_ContentsCheck = new bool[data.SensorOscLines.Count];
    }

    public void SetCurrentData()
    {
        data.VideoOscLines = GetOscLine[OscLineType.Video]?.Invoke();
        data.SensorOscLines = GetOscLine[OscLineType.Sensor]?.Invoke();
        data.PC_DeviceLines = GetDeviceLine?.Invoke();
        data.Projector_DeviceLines = GetProjectorDeviceLine?.Invoke();
        data.ContentsAddressLines = GetContentsAddressLine?.Invoke();
        data.ParticleSetPresets = GetParticleSetPresets?.Invoke();
    }
}
