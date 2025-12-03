using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCManager : Singleton<OSCManager>
{
    [Header("OscIn")]
    [SerializeField] private OscIn _oscIn;

    [Header("OscOut ÇÁ¸®Æé")]
    [SerializeField] private OscOut OSC_ChannelPrefab;

    private Dictionary<OscLineType, List<OscOut>> OscDictionary = new();

    public Action<int> PlaySequence;
    public Action StopSequence;

    private void Start()
    {
        if (!_oscIn)
        {
            _oscIn = gameObject.AddComponent<OscIn>();
            _oscIn.Open(_oscIn.port);
        }

        OscDictionary.Add(OscLineType.Video, new List<OscOut>());
        OscDictionary.Add(OscLineType.Sensor, new List<OscOut>());
        OscDictionary.Add(OscLineType.Sound, new List<OscOut>());

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
}
