using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem.Interactions;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

public class DBManager : MonoBehaviour
{
    DatabaseReference refData;
    public static DBManager instance;
    public CheckData cheakData;

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
    private void Start()
    {
    }

    void Update()
    {
    }

    public void SaveData(CheckData data) //파라미터 CheakData 객체로 수정 필요
    {
        string jsondata = JsonUtility.ToJson(data);
        //refData.Child("Information").Child("data1").SetRawJsonValueAsync(jsondata); 

        //시리얼을 먼저 키로 저장하고 아래에 json 정보 입력
        refData.Child("Information").Child(data.serial.ToString()).SetRawJsonValueAsync(jsondata);
    }

    public async Task<CheckData> LoadData(string _serial)
    {
        TaskCompletionSource<CheckData> tcs = new TaskCompletionSource<CheckData>();

        await refData.Child("Information").Child(_serial).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("로딩 실패");
                tcs.SetException(task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snap = task.Result;
                Dictionary<string, string> loadData = new Dictionary<string, string>();
                foreach (DataSnapshot data in snap.Children)
                {
                    loadData.Add(data.Key, data.Value.ToString());
                }
                CheckData result = new CheckData(loadData["serial"], loadData["MFD"], loadData["EXP"], loadData["CheakD"], loadData["PRESS"]);
                cheakData = result;
                tcs.SetResult(result);
            }
        });
        return await tcs.Task;
    }
}

public class CheckData
{
    public int serial; //시리얼 넘버(식별용)
    public string MFD; //제조일자
    public string EXP; //유효일자
    public string CheakD; //최근 점검일자
    public float PRESS; //압력

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
