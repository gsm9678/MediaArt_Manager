using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public JsonDataformat data;

    [HideInInspector] public bool is_JsonLoad = false;

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
        List<OscLine> oscLines = new();
        foreach(OscLine oscLine in GetOscLine[OscLineType.Video]?.Invoke())
            oscLines.Add(new(oscLine));
        data.VideoOscLines = new(oscLines);

        oscLines = new();
        foreach (OscLine oscLine in GetOscLine[OscLineType.Sensor]?.Invoke())
            oscLines.Add(new(oscLine));
        data.SensorOscLines = new(oscLines);

        oscLines = new();
        foreach (OscLine oscLine in GetOscLine[OscLineType.Sound]?.Invoke())
            oscLines.Add(new(oscLine));
        data.AudioOscLines = new(oscLines);

        List<PCDeviceLine> pcDeviceLines = new();
        foreach (PCDeviceLine pcDeviceLine in GetDeviceLine?.Invoke())
            pcDeviceLines.Add(new(pcDeviceLine));
        data.PC_DeviceLines = new(pcDeviceLines);

        List<ProjectorDeviceLine> projectorDeviceLines = new();
        foreach (ProjectorDeviceLine projectorDeviceLine in GetProjectorDeviceLine?.Invoke())
            projectorDeviceLines.Add(new(projectorDeviceLine));
        data.Projector_DeviceLines = new(projectorDeviceLines);

        List<ContentsAddressLine> contentsAddressLines = new();
        foreach (ContentsAddressLine contentsAddressLine in GetContentsAddressLine?.Invoke())
            contentsAddressLines.Add(new(contentsAddressLine));
        data.ContentsAddressLines = new(contentsAddressLines);
    }
}
