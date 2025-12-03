using System;
using System.Collections.Generic;

[Serializable]
public enum OscLineType { Video, Sensor, Sound }

[Serializable]
public class OscLine
{
    public string Name;
    public string IpAddress;
    public int Port;

    public OscLine(string name, string ipAddress, int port)
    {
        Name = name;
        IpAddress = ipAddress;
        Port = port;
    }

    public OscLine(OscLine oscLine)
    {
        Name = oscLine.Name;
        IpAddress = oscLine.IpAddress;
        Port = oscLine.Port;
    }
}

[Serializable]
public class PCDeviceLine
{
    public string Name;
    public string IpAddress;
    public string MacAddress;

    public PCDeviceLine(string name, string ipAddress, string macAddress)
    {
        Name = name;
        IpAddress = ipAddress;
        MacAddress = macAddress;
    }

    public PCDeviceLine(PCDeviceLine pcDeviceLine)
    {
        Name = pcDeviceLine.Name;
        IpAddress = pcDeviceLine.IpAddress;
        MacAddress = pcDeviceLine.MacAddress;
    }
}

[Serializable]
public class ProjectorDeviceLine
{
    public string Name;
    public string IpAddress;

    public ProjectorDeviceLine(string name, string ipAddress, string macAddress)
    {
        Name = name;
        IpAddress = ipAddress;
    }

    public ProjectorDeviceLine(ProjectorDeviceLine projectorDeviceLine)
    {
        Name = projectorDeviceLine.Name;
        IpAddress = projectorDeviceLine.IpAddress;
    }
}

[Serializable]
public class ContentsAddressLine
{
    public string Num;
    public string VideoAddress;
    public string SensorAddress;
    public string AudioAddress;
    public int Time;

    public ContentsAddressLine(string num, string videoAddress, string sensorAddress, string audioAddress, int time)
    {
        Num = num;
        VideoAddress = videoAddress;
        SensorAddress = sensorAddress;
        AudioAddress = audioAddress;
        Time = time;
    }

    public ContentsAddressLine(ContentsAddressLine contentsAddressLine)
    {
        Num = contentsAddressLine.Num;
        VideoAddress = contentsAddressLine.VideoAddress;
        SensorAddress = contentsAddressLine.SensorAddress;
        AudioAddress = contentsAddressLine.AudioAddress;
        Time = contentsAddressLine.Time;
    }
}

[Serializable]
public class JsonDataformat
{
    public List<OscLine> VideoOscLines = new();
    public List<OscLine> SensorOscLines = new();
    public List<OscLine> AudioOscLines = new();
    public List<PCDeviceLine> PC_DeviceLines = new();
    public List<ProjectorDeviceLine> Projector_DeviceLines = new();
    public List<ContentsAddressLine> ContentsAddressLines = new();
}