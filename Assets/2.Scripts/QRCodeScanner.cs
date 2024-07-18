using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using ZXing;
using UnityEngine.SceneManagement;

public class QRCodeScanner : MonoBehaviour
{
    public RawImage cameraView;
    private WebCamTexture camTexture;
    private bool isCameraInitialized = false;
    public GameObject menu;
    public List<InputField> inputFields = new List<InputField>();
    public List<VideoClip> videos = new List<VideoClip>();
    public VideoPlayer videoPlayer1;
    public VideoPlayer videoPlayer2;
    private CheckData chkData;
    public Text text;

    private bool isDataLoaded = false;
    private bool isScanning = false;

    void Start()
    {
        InitializeCamera();
    }

    void InitializeCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            text.text = "No camera detected";
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                camTexture = new WebCamTexture(devices[i].name, Screen.width * 5, Screen.height * 5);
                break;
            }
        }

        if (camTexture == null)
        {
            camTexture = new WebCamTexture(devices[0].name, Screen.width * 5, Screen.height * 5);
        }

        text.text = "Camera initialized";
        isScanning = true;
        cameraView.texture = camTexture;
        camTexture.Play();
        isCameraInitialized = true;
    }


    void Update()
    {
        if (!isCameraInitialized) return;
        if (!isScanning)
        {
            if (isDataLoaded == false)
                for (int i = 0; i < 1000; i++)
                    SetUI();
            isDataLoaded = true;
            return;
        }
        if (camTexture.isPlaying && camTexture.didUpdateThisFrame)
        {
            ScanQRCode();
        }
    }

    async void ScanQRCode()
    {
        IBarcodeReader barcodeReader = new BarcodeReader();
        text.text = "Scanning...";
        Result result = barcodeReader.Decode(camTexture.GetPixels32(),
            camTexture.width, camTexture.height);

        if (result != null)
        {
            text.text = "QR Code detected";
            menu.SetActive(true);
            chkData = await DBManager.instance.LoadData(result.Text);
            if (chkData != null)
            {
                SetUI();
                isScanning = false;
                camTexture.Stop();
            }
            else
            {
                text.text = "No data found for this QR code";
            }
        }
    }

    private void SetUI()
    {
        if (chkData != null && inputFields.Count >= 5)
        {
            inputFields[0].text = chkData.serial.ToString(); //시리얼
            inputFields[1].text = chkData.MFD; // 제조일자
            inputFields[2].text = chkData.EXP; // 유효일자
            inputFields[3].text = chkData.CheakD; // 최근점검일
            inputFields[4].text = chkData.PRESS.ToString(); // 압력
            if (SceneManager.GetActiveScene().name == "NormalScene")
            {
                if (chkData.serial == 1)
                {
                    videoPlayer1.clip = videos[0];
                    videoPlayer2.clip = videos[2];
                }
                else
                {
                    videoPlayer1.clip = videos[1];
                    videoPlayer2.clip = videos[3];
                }
            }
        }
        else
        {
            text.text = "Error: Invalid data or UI setup";
        }
    }

    public void SaveData()
    {
        if (chkData != null && inputFields.Count >= 5)
        {
            chkData.serial = int.Parse(inputFields[0].text);
            chkData.MFD = inputFields[1].text;
            chkData.EXP = inputFields[2].text;
            chkData.CheakD = inputFields[3].text;
            chkData.PRESS = float.Parse(inputFields[4].text);
            DBManager.instance.SaveData(chkData);
            text.text = "Data saved";
        }
        else
        {
            text.text = "Error: Cannot save data";
        }
    }

    void OnDestroy()
    {
        if (camTexture != null)
        {
            camTexture.Stop();
        }
    }
}