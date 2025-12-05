using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class EpsonProjectorPjLinkController : Singleton<EpsonProjectorPjLinkController>
{
    [Header("관리할 Epson 프로젝터들")]
    public  List<ProjectorDeviceLine> projectors;

    [Header("PJLink 포트 (기본 4352)")]
    public int port = 4352;

    [Header("PJLink 비밀번호 (없으면 비워두기)")]
    public string pjlinkPassword = "";   // 비밀번호 없으면 빈 문자열

    [Tooltip("비밀번호를 사용할 경우 true (프로젝터에서 PJLink 암호가 설정되어 있을 때)")]
    public bool usePassword = false;

    private void Start()
    {
        StartCoroutine(StartRoutine());
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitUntil(() =>
            GameManager.Instance.is_JsonLoad);

        projectors = GameManager.Instance.data.Projector_DeviceLines;
        //GameManager.Instance.DeviceOnAction += PowerOnAll;
        //GameManager.Instance.DeviceOffAction += PowerOffAll;
    }

    private void OnDestroy()
    {
        //GameManager.Instance.DeviceOnAction -= PowerOnAll;
        //GameManager.Instance.DeviceOffAction -= PowerOffAll;
    }

    // ─────────────────────────────────────────────────────────
    // 인덱스로 타겟 가져오기
    // ─────────────────────────────────────────────────────────
    private ProjectorDeviceLine GetTarget(int index)
    {
        if (projectors == null || projectors.Count == 0)
        {
            Debug.LogError("[EpsonPJLink] projectors 배열이 비어 있습니다.");
            return null;
        }

        if (index < 0 || index >= projectors.Count)
        {
            Debug.LogError($"[EpsonPJLink] 잘못된 인덱스: {index}");
            return null;
        }

        return projectors[index];
    }

    // ─────────────────────────────────────────────────────────
    // 간단 테스트용 ContextMenu (0번 프로젝터)
    // ─────────────────────────────────────────────────────────
    [ContextMenu("0번 프로젝터 - 전원 켜기")]
    public void PowerOn_0() => PowerOnSingle(0);

    [ContextMenu("0번 프로젝터 - 전원 끄기")]
    public void PowerOff_0() => PowerOffSingle(0);

    [ContextMenu("모든 프로젝터 - 전원 켜기")]
    public void PowerOnAll()
    {
        if (projectors == null) return;
        for (int i = 0; i < projectors.Count; i++)
            PowerOnSingle(i);
    }

    [ContextMenu("모든 프로젝터 - 전원 끄기")]
    public void PowerOffAll()
    {
        if (projectors == null) return;
        for (int i = 0; i < projectors.Count; i++)
            PowerOffSingle(i);
    }

    // ─────────────────────────────────────────────────────────
    //  단일 프로젝터 On / Off
    // ─────────────────────────────────────────────────────────
    public void PowerOnSingle(int index)
    {
        var t = GetTarget(index);
        if (t == null) return;

        SendPjLinkPowerCommand(t, true);
    }

    public void PowerOffSingle(int index)
    {
        var t = GetTarget(index);
        if (t == null) return;

        SendPjLinkPowerCommand(t, false);
    }

    // ─────────────────────────────────────────────────────────
    // PJLink 전원 제어 핵심 함수
    //  - on == true  -> 켜기 (%1POWR 1\r)
    //  - on == false -> 끄기 (%1POWR 0\r)
    // ─────────────────────────────────────────────────────────
    private void SendPjLinkPowerCommand(ProjectorDeviceLine target, bool on)
    {
        if (target == null)
        {
            Debug.LogError("[EpsonPJLink] Target null");
            return;
        }

        string cmd = on ? "%1POWR 1" : "%1POWR 0"; // PJLink 명령 (CR은 뒤에서 붙임)
        string mode = on ? "ON" : "OFF";

        try
        {
            using (var client = new TcpClient())
            {
                Debug.Log($"[EpsonPJLink] {target.Name} ({target.IpAddress}:{port}) 연결 시도 ({mode})");
                client.ReceiveTimeout = 2000;
                client.SendTimeout = 2000;

                client.Connect(target.IpAddress, port);

                using (var stream = client.GetStream())
                {
                    // 1) 첫 응답 (PJLINK 0\r 또는 PJLINK 1 xxxxxxxx\r) 읽기
                    string greeting = ReadLineCR(stream);
                    Debug.Log($"[EpsonPJLink] 응답: \"{greeting}\"");

                    if (string.IsNullOrEmpty(greeting) || !greeting.StartsWith("PJLINK"))
                    {
                        Debug.Log("[EpsonPJLink] PJLink 응답이 아닙니다.");
                        return;
                    }

                    // 2) 인증 필요 여부 확인
                    bool requiresAuth = greeting.StartsWith("PJLINK 1");
                    string packetToSend;

                    if (!requiresAuth)
                    {
                        // 보안 OFF (PJLINK 0) → 바로 명령 전송
                        packetToSend = cmd + "\r";
                    }
                    else
                    {
                        // 보안 ON (PJLINK 1 <random>) → 비밀번호 + 랜덤값으로 MD5 계산
                        if (!usePassword || string.IsNullOrEmpty(pjlinkPassword))
                        {
                            Debug.LogError("[EpsonPJLink] 프로젝터가 암호를 요구하지만 usePassword가 false이거나 비밀번호가 비어 있습니다.");
                            return;
                        }

                        // greeting 예: "PJLINK 1 498e4a67"
                        string[] parts = greeting.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 3)
                        {
                            Debug.LogError("[EpsonPJLink] PJLINK 1 응답 파싱 실패");
                            return;
                        }

                        string random = parts[2].Trim(); // "498e4a67\r" 이런 식 -> CR 제거
                        random = random.Replace("\r", "").Replace("\n", "");

                        string authSource = random + pjlinkPassword;
                        string hashHex = ComputeMd5Hex(authSource);

                        // MD5 해시(32바이트) + 명령 + CR
                        packetToSend = hashHex + cmd + "\r";
                    }

                    // 3) 명령 전송
                    byte[] sendBytes = Encoding.ASCII.GetBytes(packetToSend);
                    stream.Write(sendBytes, 0, sendBytes.Length);
                    stream.Flush();

                    // 4) 응답 읽기 (예: %1POWR=OK\r 또는 에러)
                    string resp = ReadLineCR(stream);
                    if (!string.IsNullOrEmpty(resp))
                        Debug.Log($"[EpsonPJLink] {target.Name} 응답: \"{resp}\"");
                    else
                        Debug.Log($"[EpsonPJLink] {target.Name} 응답 없음 (하지만 명령은 전송됨)");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"[EpsonPJLink] {target.Name} 제어 중 예외: {ex.Message}");
        }
    }

    // CR(\r) 기준으로 한 줄 읽기
    private string ReadLineCR(NetworkStream stream, int timeoutMs = 2000)
    {
        stream.ReadTimeout = timeoutMs;

        var sb = new StringBuilder();
        byte[] buffer = new byte[1];

        try
        {
            while (true)
            {
                int read = stream.Read(buffer, 0, 1);
                if (read <= 0)
                    break;

                char c = (char)buffer[0];
                if (c == '\r')
                    break;

                // 일부 장비는 \n도 넣을 수 있으니 그냥 추가만 하고 나중에 Trim
                sb.Append(c);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[EpsonPJLink] ReadLineCR 예외: {e.Message}");
        }

        return sb.ToString().Trim('\r', '\n');
    }

    // MD5(string) -> 32자리 소문자 hex
    private string ComputeMd5Hex(string text)
    {
        using (var md5 = MD5.Create())
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            byte[] hash = md5.ComputeHash(bytes);
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}