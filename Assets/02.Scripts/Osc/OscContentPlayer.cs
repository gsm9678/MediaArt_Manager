using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscContentPlayer : MonoBehaviour
{
    private List<ContentsAddressLine> contentSequence;
    Coroutine coroutine;

    private void Start()
    {
        StartCoroutine(StartRoutine());
    }
    private void OnDestroy()
    {
        if (OSCManager.Instance != null)
        {
            OSCManager.Instance.PlaySequence -= PlaySequence;
            OSCManager.Instance.StopSequence -= StopSequence;
        }

        StopSequence();
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitUntil(() =>
            GameManager.Instance.is_JsonLoad);

        contentSequence = GameManager.Instance.data.ContentsAddressLines;
        OSCManager.Instance.PlaySequence += PlaySequence;
        OSCManager.Instance.StopSequence += StopSequence;
    }

    public void PlaySequence(int i)
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(PlayContentRoutine(i));
    }

    public void StopSequence()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        OSCManager.Instance.SendOSC(OscLineType.Video, "/composition/disconnectall", 1);
        OSCManager.Instance.SendOSC(OscLineType.Sensor, "나중에 주소 추가", 1);
        OSCManager.Instance.SendOSC(OscLineType.Sound, "/stop", 1);
    }

    private IEnumerator PlayContentRoutine(int num)
    {
        for (int i = num; i < contentSequence.Count; i++)
        {
            OSCManager.Instance.SendOSC(OscLineType.Video, contentSequence[i].VideoAddress, 1);
            OSCManager.Instance.SendOSC(OscLineType.Sound, contentSequence[i].AudioAddress, 1);
            yield return new WaitForSecondsRealtime(contentSequence[i].ContentsTime);
            OSCManager.Instance.SendOSC(OscLineType.Sensor, contentSequence[i].SensorAddress, 1);
            yield return WaitForInteractionOrTimeout(contentSequence[i].InteractiveTime);
        }

        StopSequence();
    }

    private IEnumerator WaitForInteractionOrTimeout(float timeout)
    {
        float elapsed = 0f;

        while (true)
        {
            elapsed += Time.fixedDeltaTime;

            if (elapsed > timeout || GameManager.Instance.is_ContentsPlayed)
                break;

            yield return new WaitForFixedUpdate();
        }

        GameManager.Instance.is_ContentsPlayed = false;
    }
}
