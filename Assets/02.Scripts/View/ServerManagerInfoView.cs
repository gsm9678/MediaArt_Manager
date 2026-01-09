using System.Collections;
using TMPro;
using UnityEngine;

public class ServerManagerInfoView : MonoBehaviour
{
    [SerializeField] TMP_Text NameTxt;
    [SerializeField] TMP_Text TelTxt;
    [SerializeField] TMP_Text MobileTxt;

    private ServerManagerData _serverManagerData;

    private void Start()
    {
        StartCoroutine(StartRoutine());
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitUntil(() => GameManager.Instance.is_JsonLoad);

        _serverManagerData = GameManager.Instance.data.ServerManagerData;
        NameTxt.text = "¿Ã∏ß: " + _serverManagerData.Name;
        TelTxt.text = "T: " + _serverManagerData.Tel;
        MobileTxt.text = "M: " + _serverManagerData.Modile;
    }
}
