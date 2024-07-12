using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public class DBManager : MonoBehaviour
{ 
    DatabaseReference refData;
    public Text Text;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        refData = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveData() //파라미터 CheakData 객체로 수정 필요
    {
        CheakData data1 = new CheakData(255, "2024-05-10", "2027-06-10", 8f);

        string jsondata = JsonUtility.ToJson(data1);
        //refData.Child("Information").Child("data1").SetRawJsonValueAsync(jsondata); 
        
        //시리얼을 먼저 키로 저장하고 아래에 json 정보 입력
        refData.Child("Information").Child(data1.serial.ToString()).SetRawJsonValueAsync(jsondata);
    }

    public void LoadData(string _serial) //파라미터 CheakData 객체로 수정 필요
    {
        refData.Child("Information").Child(_serial).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            Dictionary<string, string> LoadData = new Dictionary<string, string>();
            if(task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("로딩 실패");
            }
            else if(task.IsCompleted)
            {
                DataSnapshot snap = task.Result;
                //: DB 조회 결과 형식

                foreach(DataSnapshot data in snap.Children)
                {
                    //Debug.Log($"{data.Key} : {data.Value}");
                    LoadData.Add(data.Key, data.Value.ToString());
                    Debug.Log($"{data.Key} : {LoadData[data.Key]}");
                    Text.text = data.Value.ToString();
                }
            }
        });
    }

    public void testDataUpload(string testNum)
    {
        testKeyValue testJson = new testKeyValue(testNum, "testSuccess");
        string jsondata = JsonUtility.ToJson(testJson);
        refData.Child("Information").Child(jsondata);
    }
}

public class testKeyValue
{
    string Key;
    string Value;

    public testKeyValue(string _Key, string _Value)
    {
        Key = _Key;
        Value = _Value;
    }
}

public class CheakData
{
    public int serial; //시리얼 넘버(식별용)
    public string MFD; //제조일자
    public string EXP; //유효일자
    public string Cheak; //최근 점검일자
    public float PRESS; //압력

    public CheakData(int _serial, string _MFD, string _EXP, float _press)
    {
        serial = _serial;
        MFD = _MFD;
        EXP = _EXP;
        PRESS = _press;
    }
    
    public int GetSerial() { return serial; }
}
