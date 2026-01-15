using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscContentPlayer : MonoBehaviour
{
    private List<ContentsAddressLine> contentSequence;
    private List<ParticleSetPreset> MediaArtSequence;
    Coroutine coroutine;

    private bool _paused;

    // "재전송"을 위해 마지막으로 보낸 ParticleSelect 값을 기억
    private bool _hasLastSelect;
    private string _lastAddress;
    private int _lastSelectNum;

    private int _lastColumnNum;

    private void Start()
    {
        StartCoroutine(StartRoutine());
    }
    private void OnDestroy()
    {
        if (OSCManager.Instance != null)
        {
            GameManager.Instance.ContentsStartAction -= PlaySequence;
            GameManager.Instance.MediaArtStartAction -= MediaArtPlaySequence;
            GameManager.Instance.ResumeAction -= ResumeSequence;
            GameManager.Instance.PauseAction -= PauseSequence;
            GameManager.Instance.StopAction -= StopSequence;
        }

        StopSequence();
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitUntil(() =>
            GameManager.Instance.is_JsonLoad);

        contentSequence = GameManager.Instance.data.ContentsAddressLines;
        MediaArtSequence = GameManager.Instance.data.ParticleSetPresets;
        GameManager.Instance.ContentsStartAction += PlaySequence;
        GameManager.Instance.MediaArtStartAction += MediaArtPlaySequence;
        GameManager.Instance.ResumeAction += ResumeSequence;
        GameManager.Instance.PauseAction += PauseSequence;
        GameManager.Instance.StopAction += StopSequence;
    }

    public void PlaySequence(int i)
    {
        StopSequence();

        coroutine = StartCoroutine(PlayContentRoutine(i));
    }

    public void MediaArtPlaySequence(int i)
    {
        StopSequence();

        coroutine = StartCoroutine(MediaArtPlayRoutine(i));
    }

    public void PauseSequence()
    {
        for(int i = 1; i <= 5; i++)
            OSCManager.Instance.SendOSC(OscLineType.Video, "/composition/layers/" + i + "/clips/" + _lastColumnNum + "/transport/position/behaviour/playdirection", 1);

        SendSensorOSC("/Contents/Stop", 1);
        SendSensorOSC("/MediaArt/ParticleStop");
        _paused = true;
    }

    public void ResumeSequence()
    {
        if (!_paused) return;
        _paused = false;

        for (int i = 1; i <= 5; i++)
            OSCManager.Instance.SendOSC(OscLineType.Video, "/composition/layers/" + i + "/clips/" + _lastColumnNum + "/transport/position/behaviour/playdirection", 2);

        if (_hasLastSelect)
            SendSensorOSC(_lastAddress, _lastSelectNum);
    }

    public void StopSequence()
    {
        ResumeSequence();

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        OSCManager.Instance.SendOSC(OscLineType.Video, "/composition/columns/1/connect", 1);
        _lastColumnNum = 1;

        SendSensorOSC("/Contents/Stop", 1);
        SendSensorOSC("/MediaArt/ParticleStop");
    }

    private IEnumerator PlayContentRoutine(int num)
    {
        for (int i = num; i < contentSequence.Count; i++)
        {
            yield return WaitWhilePaused();

            OSCManager.Instance.SendOSC(OscLineType.Video, contentSequence[i].VideoAddress, 1);
            _lastColumnNum = int.Parse(contentSequence[i].VideoAddress.Substring(21, 1));

            SendSensorOSC("/Contents/Stop", 1);
            yield return WaitForTimeOut(contentSequence[i].ContentsTime);

            _hasLastSelect = true;

            SendSensorOSC(contentSequence[i].SensorAddress, contentSequence[i].Num);
            yield return WaitForInteractionOrTimeout(contentSequence[i].InteractiveTime);

            _hasLastSelect = false;
        }

        StopSequence();
    }

    private IEnumerator MediaArtPlayRoutine(int num)
    {
        for(int i = num; i < MediaArtSequence.Count; i++)
        {
            yield return WaitWhilePaused();

            OSCManager.Instance.SendOSC(OscLineType.Video, MediaArtSequence[i].OscAddress, 1);
            _lastColumnNum = int.Parse(contentSequence[i].VideoAddress.Substring(contentSequence[i].VideoAddress.IndexOf(',') + 1, 1).Trim());

            for (int j = 0; MediaArtSequence[i].Particles.Count > j; j++)
            {
                yield return WaitWhilePaused();

                // Select 보내고 "재전송용"으로 저장
                _hasLastSelect = true;

                SendSensorOSC("/MediaArt/ParticleSelect", MediaArtSequence[i].Particles[j].Num);
                yield return WaitForTimeOut(MediaArtSequence[i].Particles[j].Time);
            }

            _hasLastSelect = false;
            SendSensorOSC("/MediaArt/ParticleStop");
        }

        StopSequence();
    }

    private void SendSensorOSC(string s, int i = 0)
    {
        _lastSelectNum = i;
        _lastAddress = s;

        OSCManager.Instance.SendOSC(OscLineType.Sensor, s, i);
    }

    private IEnumerator WaitWhilePaused()
    {
        while (_paused)
            yield return null; // 프레임 대기
    }

    private IEnumerator WaitForInteractionOrTimeout(float timeout)
    {
        float elapsed = 0f;

        while (elapsed < timeout && !GameManager.Instance.is_ContentsPlayed)
        {
            // paused면 시간 누적 멈춤
            if (!_paused)
                elapsed += Time.deltaTime;

            yield return null;
        }

        for (int i = 0; i < GameManager.Instance.is_ContentsCheck.Length; i++)
        {
            GameManager.Instance.is_ContentsCheck[i] = false;
        }

        GameManager.Instance.is_ContentsPlayed = false;
    }

    private IEnumerator WaitForTimeOut(float timeout)
    {
        float elapsed = 0f;

        while (elapsed < timeout)
        {
            // paused면 시간 누적 멈춤
            if (!_paused)
                elapsed += Time.deltaTime;

            yield return null;
        }
    }
}
