using System.Collections;
using System.Collections.Generic;
using System.Diagnostics; // Process
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation; // Ping
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MultiPcRemoteController : Singleton<MultiPcRemoteController>
{
    [Header("관리할 PC 리스트")]
    public List<PCDeviceLine> targets;

    [Header("원격 종료에 사용할 계정 정보 (해당 PC의 관리자 계정)")]
    public string username = "user";
    public string password = "1234";

    [Header("WOL 정보")]
    public string broadcastAddress = "255.255.255.255";
    public int wolPort = 9;

    [Header("PC 상태 모니터링 (인덱스별로 targets와 1:1 매칭)")]
    public List<bool> isOnlineList = new List<bool>();  // true = 켜짐, false = 꺼짐(또는 알 수 없음)

    private Coroutine monitorCoroutine;

    private void Start()
    {
        StartCoroutine(StartRoutine());
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitUntil(() =>
            GameManager.Instance.is_JsonLoad);

        targets = GameManager.Instance.data.PC_DeviceLines;
        GameManager.Instance.DeviceOnAction += WakeAll;
        GameManager.Instance.DeviceOffAction += ShutdownAll;

        // targets가 셋팅된 후에 상태 모니터링 시작
        monitorCoroutine = StartCoroutine(MonitorPcStateRoutine());
    }

    private void OnDestroy()
    {
        GameManager.Instance.DeviceOnAction -= WakeAll;
        GameManager.Instance.DeviceOffAction -= ShutdownAll;
        //DisconnectAll();

        if (monitorCoroutine != null)
        {
            StopCoroutine(monitorCoroutine);
            monitorCoroutine = null;
        }
    }

    // =========================================================
    //  공통: 타겟 가져오기
    // =========================================================
    private PCDeviceLine GetTarget(int index)
    {
        if (targets == null || targets.Count == 0)
        {
            Debug.Log("[MultiPcRemoteController] targets 배열이 비어 있습니다.");
            return null;
        }
        if (index < 0 || index >= targets.Count)
        {
            Debug.Log($"[MultiPcRemoteController] 잘못된 인덱스: {index}");
            return null;
        }
        return targets[index];
    }

    // 현재 인덱스의 PC가 켜져 있는지 여부 반환 (기본값: false)
    private bool IsPcOnline(int index)
    {
        if (isOnlineList == null || index < 0 || index >= isOnlineList.Count)
            return false;

        return isOnlineList[index];
    }

    // isOnlineList 크기를 targets에 맞게 맞추기
    private void EnsureStateList()
    {
        if (targets == null)
        {
            isOnlineList.Clear();
            return;
        }

        if (isOnlineList == null)
            isOnlineList = new List<bool>();

        if (isOnlineList.Count != targets.Count)
        {
            isOnlineList.Clear();
            for (int i = 0; i < targets.Count; i++)
                isOnlineList.Add(false); // 초기값은 false (꺼짐/미확인)
        }
    }

    // =========================================================
    //  PC 상태 모니터링 코루틴 (10초마다)
    // =========================================================
    private IEnumerator MonitorPcStateRoutine()
    {
        var ping = new System.Net.NetworkInformation.Ping();
        const int timeoutMs = 500; // 타임아웃(0.5초 정도로 짧게)

        while (true)
        {
            if (targets == null || targets.Count == 0)
            {
                yield return new WaitForSeconds(10f);
                continue;
            }

            EnsureStateList();

            for (int i = 0; i < targets.Count; i++)
            {
                var t = targets[i];
                bool prevState = isOnlineList[i];
                bool newState = false;

                try
                {
                    if (!string.IsNullOrWhiteSpace(t.IpAddress))
                    {
                        var reply = ping.Send(t.IpAddress, timeoutMs);
                        newState = (reply != null && reply.Status == IPStatus.Success);
                    }
                }
                catch
                {
                    newState = false;
                }

                isOnlineList[i] = newState;

                if (prevState != newState)
                {
                    Debug.Log($"[PING] {t.Name} ({t.IpAddress}) 상태 변경 : {(newState ? "온라인" : "오프라인")}");
                }
            }

            yield return new WaitForSeconds(10f);
        }
    }

    // =========================================================
    //  WOL (Wake-on-LAN)
    // =========================================================

    [ContextMenu("0번 PC - WOL (켜기)")]
    public void WakePc0() => WakeSingle(0);

    public void WakeSingle(int index)
    {
        var t = GetTarget(index);
        if (t == null) return;

        // 이미 켜져 있으면 WOL 명령 안 보냄
        if (IsPcOnline(index))
        {
            Debug.Log($"[WOL] {t.Name} ({t.IpAddress}) : 이미 켜져 있어서 WOL을 보내지 않습니다.");
            return;
        }

        if (string.IsNullOrWhiteSpace(t.MacAddress))
        {
            Debug.Log($"[WOL] {t.Name} : MAC 주소가 비어 있습니다.");
            return;
        }

        try
        {
            Debug.Log($"[WOL] {t.Name} ({t.IpAddress}) - MAC: {t.MacAddress}, BCast: {broadcastAddress}:{wolPort}");
            SendWolPacket(t.MacAddress, broadcastAddress, wolPort);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[WOL] {t.Name} : 예외 발생 - {ex.Message}");
        }
    }

    [ContextMenu("모든 PC - WOL (켜기)")]
    public void WakeAll()
    {
        if (targets == null || targets.Count == 0)
        {
            Debug.Log("[WOL] targets 배열이 비어 있어서 WakeAll 불가");
            return;
        }

        for (int i = 0; i < targets.Count; i++)
        {
            WakeSingle(i);
        }
    }

    private void SendWolPacket(string mac, string broadcastIP, int port)
    {
        // MAC 문자열 정리 (":" 또는 "-" 제거)
        string macClean = mac.Replace(":", "").Replace("-", "").Trim();

        if (macClean.Length != 12)
            throw new System.Exception("MAC 주소 형식이 잘못되었습니다. 예: AA-BB-CC-11-22-33");

        byte[] macBytes = new byte[6];
        for (int i = 0; i < 6; i++)
        {
            macBytes[i] = byte.Parse(macClean.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
        }

        // WOL 매직 패킷: 6바이트 0xFF + 16회 MAC 반복
        byte[] packet = new byte[6 + (16 * 6)];
        for (int i = 0; i < 6; i++)
            packet[i] = 0xFF;
        for (int i = 0; i < 16; i++)
        {
            System.Buffer.BlockCopy(macBytes, 0, packet, 6 + i * 6, 6);
        }

        IPAddress ip = IPAddress.Parse(string.IsNullOrWhiteSpace(broadcastIP) ? "255.255.255.255" : broadcastIP);

        using (var client = new UdpClient())
        {
            client.EnableBroadcast = true;
            client.Send(packet, packet.Length, new IPEndPoint(ip, port));
        }
    }

    // =========================================================
    //  NET USE 연결 / 삭제
    //  (원격 종료를 위해 인증 공유 연결)
    // =========================================================

    [ContextMenu("0번 PC - NET USE 연결")]
    public void ConnectPc0() => ConnectSingle(0);

    [ContextMenu("0번 PC - NET USE 연결 삭제")]
    public void DisconnectPc0() => DisconnectSingle(0);

    public void ConnectSingle(int index)
    {
        var t = GetTarget(index);
        if (t == null) return;

        string path = $"\\\\{t.IpAddress}\\C$";
        string arguments = $"use {path} /user:{username} {password}";

        Debug.Log($"[NET USE] {t.Name} ({t.IpAddress}) C$ 연결 시도");
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        RunProcess("net", arguments);
#else
        Debug.LogWarning("[NET USE] Windows에서만 지원됩니다.");
#endif
    }

    public void DisconnectSingle(int index)
    {
        var t = GetTarget(index);
        if (t == null) return;

        string path = $"\\\\{t.IpAddress}\\C$";
        string arguments = $"use {path} /delete /y";

        Debug.Log($"[NET USE] {t.Name} ({t.IpAddress}) C$ 연결 삭제 시도");
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        RunProcess("net", arguments);
#else
        Debug.LogWarning("[NET USE] Windows에서만 지원됩니다.");
#endif
    }

    [ContextMenu("모든 PC - NET USE 연결")]
    public void ConnectAll()
    {
        if (targets == null || targets.Count == 0)
        {
            Debug.Log("[NET USE] targets 배열이 비어 있어서 ConnectAll 불가");
            return;
        }

        for (int i = 0; i < targets.Count; i++)
        {
            ConnectSingle(i);
        }
    }

    [ContextMenu("모든 PC - NET USE 연결 삭제")]
    public void DisconnectAll()
    {
        if (targets == null || targets.Count == 0)
        {
            Debug.Log("[NET USE] targets 배열이 비어 있어서 DisconnectAll 불가");
            return;
        }

        for (int i = 0; i < targets.Count; i++)
        {
            DisconnectSingle(i);
        }
    }

    // =========================================================
    //  원격 종료 (shutdown.exe)
    // =========================================================

    [ContextMenu("0번 PC - 원격 종료")]
    public void ShutdownPc0() => ShutdownSingle(0);

    public void ShutdownSingle(int index)
    {
        var t = GetTarget(index);
        if (t == null) return;

        // 이미 꺼져 있으면 shutdown 명령 안 보냄
        if (!IsPcOnline(index))
        {
            Debug.Log($"[SHUTDOWN] {t.Name} ({t.IpAddress}) : 이미 꺼져 있어서 종료 명령을 보내지 않습니다.");
            return;
        }

        ConnectSingle(index);

        string arguments = $"/s /t 0 /m \\\\{t.IpAddress}";

        Debug.Log($"[SHUTDOWN] {t.Name} ({t.IpAddress}) 종료 시도");
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        RunProcess("shutdown", arguments);
#else
        Debug.LogWarning("[SHUTDOWN] Windows에서만 지원됩니다.");
#endif
        DisconnectSingle(index);
    }

    [ContextMenu("모든 PC - 원격 종료")]
    public void ShutdownAll()
    {
        if (targets == null || targets.Count == 0)
        {
            Debug.Log("[SHUTDOWN] targets 배열이 비어 있어서 ShutdownAll 불가");
            return;
        }

        for (int i = 0; i < targets.Count; i++)
        {
            ShutdownSingle(i);
        }
    }

    // =========================================================
    //  공통 프로세스 실행 함수 (net, shutdown)
    // =========================================================

    private void RunProcess(string fileName, string arguments)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            };

            using (var proc = new Process { StartInfo = psi })
            {
                proc.Start();

                string output = proc.StandardOutput.ReadToEnd();
                string error = proc.StandardError.ReadToEnd();

                proc.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                    Debug.Log($"[{fileName}] 출력:\n{output}");
                if (!string.IsNullOrEmpty(error))
                    Debug.Log($"[{fileName}] 에러:\n{error}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[{fileName}] 실행 중 예외 : {ex.Message}");
        }
    }
}