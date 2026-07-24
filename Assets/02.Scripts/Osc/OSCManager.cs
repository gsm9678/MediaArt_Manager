using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCManager : Singleton<OSCManager>
{
    [Header("OscIn")]
    [SerializeField] private OscIn _oscIn;

    [Header("OscOut 프리펩")]
    [SerializeField] private OscOut OSC_ChannelPrefab;

    private OscOut _remoteOscOut = null;

    private Dictionary<OscLineType, List<OscOut>> OscDictionary = new();

    private void Start()
    {
        if (!_oscIn)
        {
            _oscIn = gameObject.AddComponent<OscIn>();
            _oscIn.Open(_oscIn.port);
        }

        OscDictionary.Add(OscLineType.Video, new List<OscOut>());
        OscDictionary.Add(OscLineType.Sensor, new List<OscOut>());
        OscDictionary.Add(OscLineType.Room1Video, new List<OscOut>());

        StartCoroutine(StartRoutine());
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitUntil(() =>
            GameManager.Instance.is_JsonLoad);

        foreach (var key in OscDictionary.Keys)
        {
            foreach (var oscOutLine in GameManager.Instance.OscLineDictionary[key])
            {
                OscDictionary[key].Add(CreateOscOut(oscOutLine));
            }
        }
        _oscIn.MapInt("/Contents/Played", ContentsPlayedCheck);
        _oscIn.MapInt("/Remote/ContentsStart", ContentsStart);
        _oscIn.MapInt("/Remote/MediaArtStart", MediaArtStart);
        _oscIn.MapInt("/Remote/SoloStart", SoloStart);
        _oscIn.MapInt("/Remote/Room1Start", Room1Start);
        _oscIn.MapInt("/Remote/Room1SoloStart", Room1SoloStart);
        _oscIn.MapInt("/Remote/SelectContentsPreset", SelectContentsPreset);
        _oscIn.MapInt("/Remote/SelectRoom1Preset", SelectRoom1Preset);
        _oscIn.MapInt("/Remote/SelectParticlePreset", SelectParticlePreset);
        _oscIn.Map("/Remote/Resume", ContentsResume);
        _oscIn.Map("/Remote/Pause", ContentsPause);
        _oscIn.Map("/Remote/Stop", ContentsStop);
        _oscIn.Map("/Remote/On", DeviceOn);
        _oscIn.Map("/Remote/Off", DeviceOff);
        _oscIn.Map("/Remote/CreatRemote", RemoteStart);
    }

    void OnDestroy()
    {
        _oscIn.UnmapAll("/Contents/Played");
        _oscIn.UnmapAll("/Remote/ContentsStart");
        _oscIn.UnmapAll("/Remote/MediaArtStart");
        _oscIn.UnmapAll("/Remote/SoloStart");
        _oscIn.UnmapAll("/Remote/Room1Start");
        _oscIn.UnmapAll("/Remote/Room1SoloStart");
        _oscIn.UnmapAll("/Remote/SelectContentsPreset");
        _oscIn.UnmapAll("/Remote/SelectRoom1Preset");
        _oscIn.UnmapAll("/Remote/SelectParticlePreset");
        _oscIn.UnmapAll("/Remote/Resume");
        _oscIn.UnmapAll("/Remote/Pause");
        _oscIn.UnmapAll("/Remote/Stop");
        _oscIn.UnmapAll("/Remote/On");
        _oscIn.UnmapAll("/Remote/Off");
        _oscIn.UnmapAll("/Remote/CreatRemote");
    }

    void RemoteStart(OscMessage msg)
    {
        string ip = "";
        int port = 0;
        msg.TryGet(1, ref ip);
        msg.TryGet(0, out port);
        Debug.Log($"[Manager OSC RX] address=/Remote/CreatRemote, remoteIp={ip}, remotePort={port}");

        if (string.IsNullOrWhiteSpace(ip) || port <= 0)
        {
            Debug.LogWarning($"[Manager OSC] Invalid remote registration. IP={ip}, Port={port}");
            return;
        }

        if (_remoteOscOut != null)
        {
            Destroy(_remoteOscOut);
        }

        _remoteOscOut = CreateOscOut(new OscLine("Remote", ip, port));
        SendRemoteDataSet();
    }
    private void SendRemoteDataSet()
    {
        if (_remoteOscOut == null || GameManager.Instance.data == null)
            return;

        SendRemoteClear("MindTranning");
        SendRemoteClear("MediaArt");
        SendRemoteClear("Room1");
        SendRemoteClear("Room1Solo");
        SendRemoteClear("ContentsPresetDropdown");
        SendRemoteClear("Room1PresetDropdown");
        SendRemoteClear("ParticlePresetDropdown");

        var contentsLines = GameManager.Instance.GetSelectedContentsAddressLines();
        for (int i = 0; i < contentsLines.Count; i++)
            SendRemoteOSC("/Create/CreateMindTranningLine", i, contentsLines[i].Name);

        var particlePresets = GameManager.Instance.data.ParticleSetPresets;
        if (particlePresets != null)
        {
            for (int i = 0; i < particlePresets.Count; i++)
            {
                SendRemoteOSC("/Create/CreateMediaArtLine", i, particlePresets[i].Title);
                SendRemoteOSC("/Create/ParticlePresetDropdown", i, particlePresets[i].Title);
            }
        }

        var room1Lines = GameManager.Instance.GetSelectedRoom1ContentsAddressLines();
        for (int i = 0; i < room1Lines.Count; i++)
        {
            SendRemoteOSC("/Create/CreateRoom1Line", i, room1Lines[i].Name);
            SendRemoteOSC("/Create/CreateRoom1SoloLine", i, room1Lines[i].Name);
        }

        var contentsPresets = GameManager.Instance.data.GetContentsAddressPresets();
        for (int i = 0; i < contentsPresets.Count; i++)
            SendRemoteOSC("/Create/ContentsPresetDropdown", i, contentsPresets[i].Title);

        var room1Presets = GameManager.Instance.data.GetRoom1ContentsAddressPresets();
        for (int i = 0; i < room1Presets.Count; i++)
            SendRemoteOSC("/Create/Room1PresetDropdown", i, room1Presets[i].Title);

        SendRemoteSyncComplete("All");
    }

    void ContentsStart(int i)
    {
        Debug.Log($"[Manager OSC RX] address=/Remote/ContentsStart, value={i}");
        GameManager.Instance.ContentsStartAction?.Invoke(i);
    }
    void MediaArtStart(int i)
    {
        Debug.Log($"[Manager OSC RX] address=/Remote/MediaArtStart, value={i}");
        GameManager.Instance.MediaArtStartAction?.Invoke(i);
    }
    void SoloStart(int i)
    {
        Debug.Log($"[Manager OSC RX] address=/Remote/SoloStart, value={i}");
        GameManager.Instance.SoloContentsAction?.Invoke(i);
    }
    void Room1Start(int i)
    {
        Debug.Log($"[Manager OSC RX] address=/Remote/Room1Start, value={i}");
        GameManager.Instance.Room1ContentsStartAction?.Invoke(i);
    }
    void Room1SoloStart(int i)
    {
        Debug.Log($"[Manager OSC RX] address=/Remote/Room1SoloStart, value={i}");
        GameManager.Instance.Room1SoloContentsAction?.Invoke(i);
    }
    void SelectContentsPreset(int i)
    {
        Debug.Log($"[Manager OSC RX] address=/Remote/SelectContentsPreset, value={i}");
        GameManager.Instance.SelectedContentsAddressPresetIndex = i;
        SendRemoteDataSet();
    }

    void SelectRoom1Preset(int i)
    {
        Debug.Log($"[Manager OSC RX] address=/Remote/SelectRoom1Preset, value={i}");
        GameManager.Instance.SelectedRoom1ContentsAddressPresetIndex = i;
        SendRemoteDataSet();
    }

    void SelectParticlePreset(int i)
    {
        Debug.Log($"[Manager OSC RX] address=/Remote/SelectParticlePreset, value={i}");
        GameManager.Instance.SelectedParticleSetPresetIndex = i;
        SendRemoteDataSet();
    }

    void ContentsResume(OscMessage msg)
    {
        Debug.Log("[Manager OSC RX] address=/Remote/Resume");
        GameManager.Instance.ResumeAction?.Invoke();
    }
    void ContentsPause(OscMessage msg)
    {
        Debug.Log("[Manager OSC RX] address=/Remote/Pause");
        GameManager.Instance.PauseAction?.Invoke();
    }
    void ContentsStop(OscMessage msg)
    {
        Debug.Log("[Manager OSC RX] address=/Remote/Stop");
        GameManager.Instance.StopAction?.Invoke();
    }
    void DeviceOn(OscMessage msg)
    {
        Debug.Log("[Manager OSC RX] address=/Remote/On");
        GameManager.Instance.DeviceOnAction?.Invoke();
    }
    void DeviceOff(OscMessage msg)
    {
        Debug.Log("[Manager OSC RX] address=/Remote/Off");
        GameManager.Instance.DeviceOffAction?.Invoke();
    }

    void ContentsPlayedCheck(int value)
    {
        for(int i = 0; i < GameManager.Instance.is_ContentsCheck.Length; i++)
        {
            if (GameManager.Instance.is_ContentsCheck[i] == false)
            {
                GameManager.Instance.is_ContentsCheck[i] = true;
                break;
            }
        }

        for (int i = 0; i < GameManager.Instance.is_ContentsCheck.Length; i++)
            if (GameManager.Instance.is_ContentsCheck[i] == false)
                return;

        GameManager.Instance.is_ContentsPlayed = true;
    }

    public void ResetOSC()
    {
        foreach (var key in OscDictionary.Keys)
            OscDictionary[key].Clear();

        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    public OscOut CreateOscOut(OscLine oscLine)
    {
        OscOut temp = Instantiate(OSC_ChannelPrefab, this.transform);
        SetOSC(temp.GetComponent<OscOut>(), oscLine.Port, oscLine.IpAddress);
        temp.Open(oscLine.Port, oscLine.IpAddress);

        return temp;
    }

    private void SetOSC(OscOut osc, int outp, string ip = "127.0.0.1")
    {
        osc.port = outp;
        osc.remoteIpAddress = ip;
    }

    public void SendOSC(OscLineType LineType, string Message, int i)
    {
        foreach(OscOut oscOut in OscDictionary[LineType])
            oscOut.Send(Message, i);
    }

    private bool IsRemoteReady()
    {
        if (_remoteOscOut != null && _remoteOscOut.isOpen)
            return true;

        Debug.LogWarning("[Manager OSC] Remote OSC output is not open. Skipping remote sync message.");
        return false;
    }

    public void SendRemoteClear(string listType)
    {
        if (!IsRemoteReady())
            return;

        OscMessage oscMessage = new OscMessage();
        oscMessage.address = "/Create/ClearRemoteList";
        oscMessage.Add(listType);
        _remoteOscOut.Send(oscMessage);
    }

    public void SendRemoteSyncComplete(string scope)
    {
        if (!IsRemoteReady())
            return;

        OscMessage oscMessage = new OscMessage();
        oscMessage.address = "/Create/RemoteDataSyncComplete";
        oscMessage.Add(scope);
        _remoteOscOut.Send(oscMessage);
    }

    public void SendRemoteOSC(string Message, int i, string title)
    {
        if (!IsRemoteReady())
            return;

        OscMessage oscMessage = new OscMessage();
        oscMessage.address = Message;
        oscMessage.Add(i);
        oscMessage.Add(title ?? string.Empty);
        _remoteOscOut.Send(oscMessage);
    }
}

