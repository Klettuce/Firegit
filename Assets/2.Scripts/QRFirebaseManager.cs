using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using ZXing;
using System.Collections;
using Firebase.Extensions;

public class QRFirebaseManager : MonoBehaviour
{
    public RawImage cameraFeed;
    public Text displayText;
    public InputField idInput, stringValue1Input, stringValue2Input, intValueInput;
    public Button scanButton, saveButton, loadButton;

    private DatabaseReference refData;
    private WebCamTexture camTexture;
    private bool isScanning = false;

    void Start()
    {
        refData = FirebaseDatabase.DefaultInstance.RootReference;

        scanButton.onClick.AddListener(StartScan);
        saveButton.onClick.AddListener(SaveData);
        loadButton.onClick.AddListener(() => LoadData(idInput.text));

        InitializeCamera();
    }

    void InitializeCamera()
    {
        if (camTexture == null)
        {
            camTexture = new WebCamTexture();
        }
        cameraFeed.texture = camTexture;
        cameraFeed.material.mainTexture = camTexture;
    }

    void StartScan()
    {
        if (!isScanning)
        {
            isScanning = true;
            camTexture.Play();
            StartCoroutine(ScanQRCode());
        }
    }

    IEnumerator ScanQRCode()
    {
        BarcodeReader barcodeReader = new BarcodeReader();

        while (isScanning)
        {
            // 카메라가 준비될 때까지 대기
            yield return new WaitForSeconds(0.2f);

            try
            {
                // 현재 프레임에서 QR 코드 검색
                var result = barcodeReader.Decode(camTexture.GetPixels32(), 300, 300);
                if (result != null)
                {
                    idInput.text = result.Text;
                    LoadData(result.Text);
                    StopScanning();
                    yield break;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("QR 스캔 중 오류 발생: " + ex.Message);
            }

            yield return null;
        }
    }

    void StopScanning()
    {
        isScanning = false;
        camTexture.Stop();
    }

    public void SaveData()
    {
        if (int.TryParse(idInput.text, out int id) && int.TryParse(intValueInput.text, out int intValue))
        {
            DataModel data = new DataModel(
                id,
                stringValue1Input.text,
                stringValue2Input.text,
                intValue
            );
            string jsonData = JsonUtility.ToJson(data);
            refData.Child("Data").Child(data.Id.ToString()).SetRawJsonValueAsync(jsonData);
            displayText.text = "Data saved successfully";
        }
        else
        {
            displayText.text = "Invalid input data";
        }
    }

    public void LoadData(string id)
    {
        refData.Child("Data").Child(id).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && task.Result.Exists)
            {
                DataModel loadedData = JsonUtility.FromJson<DataModel>(task.Result.GetRawJsonValue());
                idInput.text = loadedData.Id.ToString();
                stringValue1Input.text = loadedData.StringValue1;
                stringValue2Input.text = loadedData.StringValue2;
                intValueInput.text = loadedData.IntValue.ToString();
                displayText.text = "Data loaded successfully";
            }
            else
            {
                displayText.text = "Data not found or error occurred";
            }
        });
    }

    void OnDisable()
    {
        if (camTexture != null && camTexture.isPlaying)
        {
            camTexture.Stop();
        }
    }
}