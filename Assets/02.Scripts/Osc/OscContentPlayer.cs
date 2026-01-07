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
            GameManager.Instance.ContentsStartAction -= PlaySequence;
            GameManager.Instance.ContentsStopAction -= StopSequence;
        }

        StopSequence();
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitUntil(() =>
            GameManager.Instance.is_JsonLoad);

        contentSequence = GameManager.Instance.data.ContentsAddressLines;
        GameManager.Instance.ContentsStartAction += PlaySequence;
        GameManager.Instance.ContentsStopAction += StopSequence;
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
        OSCManager.Instance.SendOSC(OscLineType.Sensor, "/Contents/Stop", 1);
    }

    private IEnumerator PlayContentRoutine(int num)
    {
        for (int i = num; i < contentSequence.Count; i++)
        {
            OSCManager.Instance.SendOSC(OscLineType.Video, contentSequence[i].VideoAddress, 1);
            OSCManager.Instance.SendOSC(OscLineType.Sensor, "/Contents/Stop", 1);
            yield return new WaitForSecondsRealtime(contentSequence[i].ContentsTime);
            OSCManager.Instance.SendOSC(OscLineType.Sensor, contentSequence[i].SensorAddress, contentSequence[i].Num);
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

        for (int i = 0; i < GameManager.Instance.is_ContentsCheck.Length; i++)
        {
            GameManager.Instance.is_ContentsCheck[i] = false;
        }

        GameManager.Instance.is_ContentsPlayed = false;
    }
}
