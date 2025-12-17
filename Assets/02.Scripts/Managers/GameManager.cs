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
    public Action ContentsStopAction;
    public Action DeviceOnAction;
    public Action DeviceOffAction;

    public Dictionary<OscLineType, List<OscLine>> OscLineDictionary = new();

    public Dictionary<OscLineType, Func<List<OscLine>>> GetOscLine = new();
    public event Func<List<PCDeviceLine>> GetDeviceLine;
    public event Func<List<ProjectorDeviceLine>> GetProjectorDeviceLine;
    public event Func<List<ContentsAddressLine>> GetContentsAddressLine;

    public void SetOscLineDictionary()
    {
        OscLineDictionary.Add(OscLineType.Video, data.VideoOscLines);
        OscLineDictionary.Add(OscLineType.Sensor, data.SensorOscLines);
        OscLineDictionary.Add(OscLineType.Sound, data.AudioOscLines);
    }

    public void SetCurrentData()
    {
        data.VideoOscLines = GetOscLine[OscLineType.Video]?.Invoke();
        data.SensorOscLines = GetOscLine[OscLineType.Sensor]?.Invoke();
        data.AudioOscLines = GetOscLine[OscLineType.Sound]?.Invoke();
        data.PC_DeviceLines = GetDeviceLine?.Invoke();
        data.Projector_DeviceLines = GetProjectorDeviceLine?.Invoke();
        data.ContentsAddressLines = GetContentsAddressLine?.Invoke();
    }
}
