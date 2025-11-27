using System.Collections.Generic;
using UnityEngine;

public class OSCManager : Singleton<OSCManager>
{
    [Header("OscIn")]
    [SerializeField] private OscIn _oscIn;

    [Header("OscOut ÇÁ¸®Æé")]
    [SerializeField] private GameObject OSC_ChannelPrefab;

    private List<OscOut> VideoOSCs = new();
    private List<OscOut> SensorOSCs = new();
    private List<OscOut> AudioOSCs = new();

    private Dictionary<OscLineType, List<OscOut>> OscDictionary;

    private void Start()
    {
        if (!_oscIn) _oscIn = gameObject.AddComponent<OscIn>();
            _oscIn.Open(_oscIn.port);

        OscDictionary.Add(OscLineType.Video, VideoOSCs);
        OscDictionary.Add(OscLineType.Sensor, SensorOSCs);
        OscDictionary.Add(OscLineType.Sound, AudioOSCs);
    }

    void OnEnable()
    {
        //_oscIn.MapBool(address1, DebugSiginal);
    }


    void OnDisable()
    {
        //_oscIn.UnmapBool(DebugSiginal);
    }

    public void ResetOSC()
    {
        VideoOSCs.Clear();
        SensorOSCs.Clear();
        AudioOSCs.Clear();

        foreach(Transform child in transform)
            Destroy(child.gameObject);
    }

    public void CreateOSCOut(OscLineType LineType, OscLine oscLine)
    {
        GameObject temp = Instantiate(OSC_ChannelPrefab, this.transform.position, Quaternion.identity);
        SetOSC(temp.GetComponent<OscOut>(), oscLine.Port, oscLine.IpAddress);
        OscDictionary[LineType].Add(temp.GetComponent<OscOut>());
        temp.transform.SetParent(this.transform);
        temp.SetActive(true);
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
}
