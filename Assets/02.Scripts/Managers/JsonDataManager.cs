using System.IO;
using UnityEngine;

public class JsonDataManager : MonoBehaviour
{
    private JsonDataformat data = new JsonDataformat();
    private string path;

    private void Start()
    {
        path = Path.Combine(Application.dataPath, "database.json");
        JsonLoad();
    }

    public void JsonLoad()
    {
        if (!File.Exists(path))
        {
            SaveFunc();
        }
        else
        {
            string loadJson = File.ReadAllText(path);
            data = JsonUtility.FromJson<JsonDataformat>(loadJson);

            if (data != null)
            {
                GameManager.Instance.data = data;
                GameManager.Instance.SetOscLineDictionary();
                GameManager.Instance.is_JsonLoad = true;
            }
        }
    }

    public void SaveFunc()
    {
        data = GameManager.Instance.data;

        // 데이터 저장
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

}
