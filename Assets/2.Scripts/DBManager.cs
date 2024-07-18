using UnityEngine;
using Firebase.Database;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;

public class DBManager : MonoBehaviour
{
    DatabaseReference refData;
    public static DBManager instance;
    public CheckData cheakData;

    [Header("Debug")]
    public Text debugText;
    private CheckData testData;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        refData = FirebaseDatabase.DefaultInstance.RootReference;
    }
    async private void Start()
    {
        testData = await LoadData("1");
        Test();
    }
    public void SaveData(CheckData data)
    {
        string jsondata = JsonUtility.ToJson(data);
        refData.Child("Information").Child(data.serial.ToString()).SetRawJsonValueAsync(jsondata);
    }

    public async Task<CheckData> LoadData(string _serial)
    {
        DataSnapshot snap = await refData.Child("Information").Child(_serial).GetValueAsync();

        if (!snap.Exists)
        {
            return null;
        }

        Dictionary<string, object> loadData = snap.Value as Dictionary<string, object>;
        if (loadData == null)
        {
            return null;
        }

        CheckData result = new CheckData(
            loadData["serial"].ToString(),
            loadData["MFD"].ToString(),
            loadData["EXP"].ToString(),
            loadData["CheakD"].ToString(),
            loadData["PRESS"].ToString()
        );

        cheakData = result;
        return result;
    }

    private void Test()
    {
        testData.PRESS = float.Parse("40");
        DBManager.instance.SaveData(testData);
    }
}

public class CheckData
{
    public int serial;
    public string MFD;
    public string EXP;
    public string CheakD;
    public float PRESS;

    public CheckData(int _serial)
    {
        serial = _serial;
    }

    public CheckData(string _serial, string _MFD, string _EXP, string _Cheak, string _press)
    {
        serial = Convert.ToInt32(_serial);
        MFD = _MFD;
        EXP = _EXP;
        CheakD = _Cheak;
        PRESS = (float)Convert.ToDouble(_press);
    }

    public int GetSerial() { return serial; }
}