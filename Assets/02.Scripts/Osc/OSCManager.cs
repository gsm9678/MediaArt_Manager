using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCManager : Singleton<OSCManager>
{
    [Header("OscIn")]
    [SerializeField] private OscIn _oscIn;

    [Header("OscOut ÇÁ¸®Æé")]
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
        Debug.Log("IP: " + ip);

        if (_remoteOscOut != null)
        {
            Destroy( _remoteOscOut );
        }

        _remoteOscOut = CreateOscOut(new OscLine("Remote", ip, port));

        for(int i = 0; i < GameManager.Instance.data.ContentsAddressLines.Count; i++)
        {
            SendRemoteOSC("/Create/CreateMindTranningLine", i, GameManager.Instance.data.ContentsAddressLines[i].Name);
        }
        for(int i = 0; i < GameManager.Instance.data.ParticleSetPresets.Count; i++)
        {
            SendRemoteOSC("/Create/CreateMediaArtLine", i, GameManager.Instance.data.ParticleSetPresets[i].Title);
        }
    }
    void ContentsStart(int i)
    {
        GameManager.Instance.ContentsStartAction?.Invoke(i);
    }
    void MediaArtStart(int i)
    {
        GameManager.Instance.MediaArtStartAction?.Invoke(i);
    }
    void ContentsResume(OscMessage msg)
    {
        GameManager.Instance.ResumeAction?.Invoke();
    }
    void ContentsPause(OscMessage msg)
    {
        GameManager.Instance.PauseAction?.Invoke();
    }
    void ContentsStop(OscMessage msg)
    {
        GameManager.Instance.StopAction?.Invoke();
    }
    void DeviceOn(OscMessage msg)
    {
        GameManager.Instance.DeviceOnAction?.Invoke();
    }
    void DeviceOff(OscMessage msg)
    {
        GameManager.Instance.DeviceOffAction?.Invoke();
    }

    void ContentsPlayedCheck(int value)
    {
        GameManager.Instance.is_ContentsCheck[value] = true;

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

    public void SendRemoteOSC(string Message, int i, string title)
    {
        OscMessage oscMessage = new OscMessage();
        oscMessage.address = Message;
        oscMessage.Add(i);
        oscMessage.Add(title);

        _remoteOscOut.Send(oscMessage);
    }
}
