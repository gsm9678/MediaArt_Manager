using System.Collections.Generic;
using UnityEngine;

public class OSCManager : Singleton<OSCManager>
{
    [SerializeField] GameObject OSC_ChannelPrefab;
    List<OscOut> VideoOSCs = new();
    List<OscOut> SendorOSCs = new();
    List<OscOut> AudioOSCs = new();

    public void ResetOSC()
    {
        VideoOSCs.Clear();
        SendorOSCs.Clear();
        AudioOSCs.Clear();

        foreach(Transform child in transform)
            Destroy(child.gameObject);
    }

    public void CreateOSCOut(List<OscOut> oscOuts)
    {
        GameObject temp = Instantiate(OSC_ChannelPrefab, this.transform.position, Quaternion.identity);
        //SetOSC(temp.GetComponent<OscOut>(), )
        oscOuts.Add(temp.GetComponent<OscOut>());
        temp.transform.SetParent(this.transform);
        temp.SetActive(true);
    }

    private void SetOSC(OscOut osc, int outp, string ip = "127.0.0.1")
    {
        osc.port = outp;
        osc.remoteIpAddress = ip;
    }
}
