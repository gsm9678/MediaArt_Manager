using System;
using System.Collections.Generic;

[Serializable]
public enum OscLineType { Video, Sensor, Sound }

[Serializable]
public enum DeviceType { PC, PROJECTOR }

[Serializable]
public class OscLine
{
    public string Name;
    public string IpAddress;
    public int Port;

    public OscLine() { }

    public OscLine(string name, string ipAddress, int port)
    {
        Name = name;
        IpAddress = ipAddress;
        Port = port;
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
}

[Serializable]
public class ContentsAddressLine
{
    public string Num;
    public string Address;
    public int Time;

    public ContentsAddressLine(string num, string address, int time)
    {
        Num = num;
        Address = address;
        Time = time;
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