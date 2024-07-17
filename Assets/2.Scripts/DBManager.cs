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

    public void SaveData(CheckData data) //�Ķ���� CheakData ��ü�� ���� �ʿ�
    {
        string jsondata = JsonUtility.ToJson(data);
        //refData.Child("Information").Child("data1").SetRawJsonValueAsync(jsondata); 

        //�ø����� ���� Ű�� �����ϰ� �Ʒ��� json ���� �Է�
        refData.Child("Information").Child(data.serial.ToString()).SetRawJsonValueAsync(jsondata);
    }

    public async Task<CheckData> LoadData(string _serial)
    {
        TaskCompletionSource<CheckData> tcs = new TaskCompletionSource<CheckData>();

        await refData.Child("Information").Child(_serial).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("�ε� ����");
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
    public int serial; //�ø��� �ѹ�(�ĺ���)
    public string MFD; //��������
    public string EXP; //��ȿ����
    public string CheakD; //�ֱ� ��������
    public float PRESS; //�з�

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
