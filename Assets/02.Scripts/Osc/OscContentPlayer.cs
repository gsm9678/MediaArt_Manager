using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscContentPlayer : MonoBehaviour
{
    private List<ContentsAddressLine> contentSequence;
    private List<Room1AddressLine> room1ContentSequence;
    private List<ParticleSetPreset> MediaArtSequence;
    Coroutine coroutine;
    Coroutine room1Coroutine;

    private bool _paused;

    // "占쏙옙占쏙옙占쏙옙"占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙 ParticleSelect 占쏙옙占쏙옙 占쏙옙占?
    private bool _hasLastSelect;
    private string _lastAddress;
    private int _lastSelectNum;

    private int _lastColumnNum;
    private int _lastRoom1ColumnNum;

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
            GameManager.Instance.SoloContentsAction -= PlaySoloSequence;
            GameManager.Instance.Room1ContentsStartAction -= PlayRoom1Sequence;
            GameManager.Instance.Room1SoloContentsAction -= PlayRoom1SoloSequence;
            GameManager.Instance.ResumeAction -= ResumeSequence;
            GameManager.Instance.PauseAction -= PauseSequence;
            GameManager.Instance.StopAction -= StopSequence;
            GameManager.Instance.StopAction -= GotoIdle;
        }

        StopSequence();
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitUntil(() =>
            GameManager.Instance.is_JsonLoad);

        contentSequence = GameManager.Instance.GetSelectedContentsAddressLines();
        room1ContentSequence = GameManager.Instance.GetSelectedRoom1ContentsAddressLines();
        MediaArtSequence = GameManager.Instance.data.ParticleSetPresets;
        GameManager.Instance.ContentsStartAction += PlaySequence;
        GameManager.Instance.MediaArtStartAction += MediaArtPlaySequence;
        GameManager.Instance.SoloContentsAction += PlaySoloSequence;
        GameManager.Instance.Room1ContentsStartAction += PlayRoom1Sequence;
        GameManager.Instance.Room1SoloContentsAction += PlayRoom1SoloSequence;
        GameManager.Instance.ResumeAction += ResumeSequence;
        GameManager.Instance.PauseAction += PauseSequence;
        GameManager.Instance.StopAction += StopSequence;
        GameManager.Instance.StopAction += GotoIdle;
    }

    public void PlaySequence(int i)
    {
        StopSequence();
        contentSequence = GameManager.Instance.GetSelectedContentsAddressLines();
        room1ContentSequence = GameManager.Instance.GetSelectedRoom1ContentsAddressLines();

        if (contentSequence == null || i < 0 || i >= contentSequence.Count)
            return;

        coroutine = StartCoroutine(PlayContentRoutine(i));
    }

    public void MediaArtPlaySequence(int i)
    {
        StopSequence();

        coroutine = StartCoroutine(MediaArtPlayRoutine(i));
    }

    public void PlaySoloSequence(int i)
    {
        StopSequence();
        contentSequence = GameManager.Instance.GetSelectedContentsAddressLines();
        room1ContentSequence = GameManager.Instance.GetSelectedRoom1ContentsAddressLines();

        if (contentSequence == null || i < 0 || i >= contentSequence.Count)
            return;

        coroutine = StartCoroutine(PlaySoloContentRoutine(i));
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

        if (room1Coroutine != null)
        {
            StopCoroutine(room1Coroutine);
            room1Coroutine = null;
        }

        SendSensorOSC("/Contents/Stop", 1);
        SendSensorOSC("/MediaArt/ParticleStop");
    }

    public void GotoIdle()
    {
        OSCManager.Instance.SendOSC(OscLineType.Video, "/composition/columns/1/connect", 1);
        _lastColumnNum = 1;
    }

    private IEnumerator PlayContentRoutine(int num)
    {
        if (contentSequence == null) yield break;
        for (int i = num; i < contentSequence.Count; i++)
        {
            yield return WaitWhilePaused();

            OSCManager.Instance.SendOSC(OscLineType.Video, contentSequence[i].VideoAddress, 1);
            if (contentSequence[i].VideoAddress.Contains("columns/"))
                _lastColumnNum = int.TryParse(contentSequence[i].VideoAddress.Substring(21, 1), out _lastColumnNum) ? _lastColumnNum : _lastColumnNum;

            SendSensorOSC("/Contents/Stop", 1);
            yield return WaitForTimeOut(contentSequence[i].ContentsTime);

            _hasLastSelect = true;

            SendSensorOSC(contentSequence[i].SensorAddress, contentSequence[i].Num);
            yield return WaitForInteractionOrTimeout(contentSequence[i].InteractiveTime);

            _hasLastSelect = false;
        }

        StopSequence();
        GotoIdle();
    }

    private IEnumerator PlaySoloContentRoutine(int num)
    {
        if (contentSequence == null || num < 0 || num >= contentSequence.Count)
            yield break;

        _hasLastSelect = false;

        yield return WaitWhilePaused();

        OSCManager.Instance.SendOSC(OscLineType.Video, contentSequence[num].VideoAddress, 1);
        if (contentSequence[num].VideoAddress.Contains("columns/"))
            _lastColumnNum = int.TryParse(contentSequence[num].VideoAddress.Substring(21, 1), out _lastColumnNum) ? _lastColumnNum : _lastColumnNum;

        SendSensorOSC("/Contents/Stop", 1);
        yield return WaitForTimeOut(contentSequence[num].ContentsTime);

        _hasLastSelect = true;

        SendSensorOSC(contentSequence[num].SensorAddress, contentSequence[num].Num);
        if(contentSequence[num].InteractiveTime != 0)
            yield return WaitForInteractionOrTimeoutToNext(contentSequence[num].InteractiveTime, num);

        _hasLastSelect = false;

        StopSequence();
        GotoIdle();
    }

    private IEnumerator MediaArtPlayRoutine(int num)
    {
        for(int i = num; i < MediaArtSequence.Count; i++)
        {
            yield return WaitWhilePaused();

            OSCManager.Instance.SendOSC(OscLineType.Video, MediaArtSequence[i].OscAddress, 1);
            if (MediaArtSequence[i].OscAddress.Contains("columns/"))
                _lastColumnNum = int.TryParse(MediaArtSequence[i].OscAddress.Substring(21, 1), out _lastColumnNum) ? _lastColumnNum : _lastColumnNum;

            for (int j = 0; MediaArtSequence[i].Particles.Count > j; j++)
            {
                yield return WaitWhilePaused();

                // Select 占쏙옙占쏙옙占쏙옙 "占쏙옙占쏙옙占쌜울옙"占쏙옙占쏙옙 占쏙옙占쏙옙
                _hasLastSelect = true;

                SendSensorOSC("/MediaArt/ParticleSelect", MediaArtSequence[i].Particles[j].Num);
                yield return WaitForTimeOut(MediaArtSequence[i].Particles[j].Time);
            }

            _hasLastSelect = false;
            SendSensorOSC("/MediaArt/ParticleStop");
        }

        StopSequence();
        GotoIdle();
    }


    public void PlayRoom1Sequence(int i)
    {
        StopSequence();
        room1ContentSequence = GameManager.Instance.GetSelectedRoom1ContentsAddressLines();

        if (room1ContentSequence == null || i < 0 || i >= room1ContentSequence.Count)
            return;

        room1Coroutine = StartCoroutine(Room1PlayContentRoutine(i));
    }

    public void PlayRoom1SoloSequence(int i)
    {
        StopSequence();
        room1ContentSequence = GameManager.Instance.GetSelectedRoom1ContentsAddressLines();

        if (room1ContentSequence == null || i < 0 || i >= room1ContentSequence.Count)
            return;

        room1Coroutine = StartCoroutine(Room1PlaySoloContentRoutine(i));
    }

    private IEnumerator Room1PlayContentRoutine(int num)
    {
        if (room1ContentSequence == null) yield break;

        for (int i = num; i < room1ContentSequence.Count; i++)
        {
            yield return WaitWhilePaused();

            SendRoom1VideoOSC(room1ContentSequence[i].VideoAddress);
            yield return WaitForTimeOut(room1ContentSequence[i].ContentsTime);
        }

        StopSequence();
        GotoRoom1Idle();
    }

    private IEnumerator Room1PlaySoloContentRoutine(int num)
    {
        if (room1ContentSequence == null || num < 0 || num >= room1ContentSequence.Count)
            yield break;

        yield return WaitWhilePaused();

        SendRoom1VideoOSC(room1ContentSequence[num].VideoAddress);
        yield return WaitForTimeOut(room1ContentSequence[num].ContentsTime);

        StopSequence();
        GotoRoom1Idle();
    }

    private void SendRoom1VideoOSC(string address)
    {
        OSCManager.Instance.SendOSC(OscLineType.Room1Video, address, 1);
        if (address.Contains("columns/"))
            _lastRoom1ColumnNum = int.TryParse(address.Substring(21, 1), out _lastRoom1ColumnNum) ? _lastRoom1ColumnNum : _lastRoom1ColumnNum;
    }

    private void GotoRoom1Idle()
    {
        OSCManager.Instance.SendOSC(OscLineType.Room1Video, "/composition/columns/1/connect", 1);
        _lastRoom1ColumnNum = 1;
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
            yield return null; // 占쏙옙占쏙옙占쏙옙 占쏙옙占?
    }

    private IEnumerator WaitForInteractionOrTimeout(float timeout)
    {
        float elapsed = 0f;

        while (elapsed < timeout && !GameManager.Instance.is_ContentsPlayed)
        {
            // paused占쏙옙 占시곤옙 占쏙옙占쏙옙 占쏙옙占쏙옙
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

    private IEnumerator WaitForInteractionOrTimeoutToNext(float timeout, int num)
    {
        float elapsed = 0f;

        while (elapsed < timeout && !GameManager.Instance.is_ContentsPlayed)
        {
            // paused占쏙옙 占시곤옙 占쏙옙占쏙옙 占쏙옙占쏙옙
            if (!_paused)
                elapsed += Time.deltaTime;

            yield return null;
        }

        for (int i = 0; i < GameManager.Instance.is_ContentsCheck.Length; i++)
        {
            GameManager.Instance.is_ContentsCheck[i] = false;
        }

        GameManager.Instance.is_ContentsPlayed = false;

        if(num + 1 < contentSequence.Count)
            PlaySoloSequence(num + 1);
    }

    private IEnumerator WaitForTimeOut(float timeout)
    {
        float elapsed = 0f;

        while (elapsed < timeout)
        {
            // paused占쏙옙 占시곤옙 占쏙옙占쏙옙 占쏙옙占쏙옙
            if (!_paused)
                elapsed += Time.deltaTime;

            yield return null;
        }
    }
}





