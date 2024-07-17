using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class QRCodeScanner : MonoBehaviour
{
    public RawImage cameraView;
    private WebCamTexture camTexture;
    private bool isCameraInitialized = false;
    public GameObject menu;
    public List<InputField> inputFields = new List<InputField>();
    private CheckData chkData;
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
            Debug.LogError("No camera detected");
            return;
        }

        // �ĸ� ī�޶� ã�� (��κ��� �ȵ���̵� ��⿡�� �ĸ� ī�޶� �� ����)
        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                camTexture = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                break;
            }
        }

        if (camTexture == null)
        {
            // �ĸ� ī�޶� ã�� ���ߴٸ� ù ��° ī�޶� ���
            camTexture = new WebCamTexture(devices[0].name, Screen.width, Screen.height);
        }

        isScanning = true;
        cameraView.texture = camTexture;
        camTexture.Play();
        isCameraInitialized = true;
    }

    async void Update()
    {
        if (!isCameraInitialized) return;

        if (isScanning && camTexture.isPlaying && camTexture.didUpdateThisFrame)
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            // GetPixels32�� CPU ��뷮�� ���� �� �����Ƿ� ����ȭ�� �ʿ��� �� �ֽ��ϴ�
            Result result = barcodeReader.Decode(camTexture.GetPixels32(),
                                                 camTexture.width, camTexture.height);
            if (result != null)
            {
                // ���⿡�� QR �ڵ� �����͸� ó���ϴ� ������ �߰��ϼ���
                chkData = await DBManager.instance.LoadData(result.Text);
                SetUI();
                isScanning = false;
            }
        }
    }
    private void SetUI()
    {
        menu.SetActive(true);
        inputFields[0].text = chkData.serial.ToString();
        inputFields[1].text = chkData.MFD.ToString();
        inputFields[2].text = chkData.EXP.ToString();
        inputFields[3].text = chkData.CheakD.ToString();
        inputFields[4].text = chkData.PRESS.ToString();
    }

    public void SaveData()
    {
        chkData.serial = int.Parse(inputFields[0].text);
        chkData.MFD = inputFields[1].text;
        chkData.EXP = inputFields[2].text;
        chkData.CheakD = inputFields[3].text;
        chkData.PRESS = float.Parse(inputFields[4].text);
        DBManager.instance.SaveData(chkData);
    }
    void OnDestroy()
    {
        if (camTexture != null)
        {
            camTexture.Stop();
        }
    }
}