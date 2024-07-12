using UnityEngine;
using Unity.Collections;
using UnityEngine.UI;
using ZXing;
using System.Collections;

public class AndroidQRScanner : MonoBehaviour
{
    [SerializeField] private Text resultText;
    public RawImage cameraFeed;
    private bool isCameraRunning = false;
    private WebCamTexture camTexture;

    IEnumerator Start()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            if (devices.Length == 0)
            {
                Debug.Log("No camera detected");
                yield break;
            }

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
                Debug.Log("Unable to find back camera");
                yield break;
            }

            cameraFeed.texture = camTexture;
            camTexture.Play();
            isCameraRunning = true;
        }
        else
        {
            Debug.Log("Camera Permission Denied");
        }
    }

    void Update()
    {
        if (isCameraRunning && camTexture.isPlaying)
        {
            try
            {
                IBarcodeReader barcodeReader = new BarcodeReader();
                Result result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height);
                if (result != null)
                {
                    resultText.text = result.ToString();
                    Debug.Log("SCANNED: " + result.Text);
                    // ���⿡�� QR �ڵ� �����͸� ó���ϴ� ������ �߰��ϼ���
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }
    }

    void OnDisable()
    {
        if (camTexture != null)
        {
            camTexture.Stop();
            isCameraRunning = false;
        }
    }
}