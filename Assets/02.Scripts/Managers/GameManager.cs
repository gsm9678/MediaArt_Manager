using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    public JsonDataformat data;

    public Dictionary<OscLineType, List<OscLine>> OscLineDictionary = new();
    public Dictionary<DeviceType, List<DeviceLine>> DeviceLineDictionary = new();

    private void OnEnable()
    {
        OscLineDictionary.Add(OscLineType.Video, data.VideoOscLines);
        OscLineDictionary.Add(OscLineType.Sensor, data.SensorOscLines);
        OscLineDictionary.Add(OscLineType.Sound, data.AudioOscLines);
        DeviceLineDictionary.Add(DeviceType.PC, data.PC_DeviceLines);
        DeviceLineDictionary.Add(DeviceType.PROJECTOR, data.Project_DeviceLines);
    }
}
