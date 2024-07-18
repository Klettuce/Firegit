using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class popup : MonoBehaviour
{
    public ARTrackedImageManager manager;
    public List<GameObject> list1 = new List<GameObject>();
    private Dictionary<string, GameObject> dict1 = new Dictionary<string, GameObject>();
    public Button[] popupButton;
    public GameObject[] popupPrefab;
    public AudioSource popupAudioSource;
    private int popedIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        popupAudioSource = GetComponent<AudioSource>();
        foreach (GameObject o in list1)
        {
            dict1.Add(o.name, o);
        }
        for (int i = 0; i < popupButton.Length; i++)
        {
            int index = i; // Capture the current value of i
            popupButton[i].onClick.AddListener(() => OnButtonClick(index));
        }
        foreach (GameObject popup in popupPrefab)
        {
            popup.SetActive(false);
        }

    }

    //void OnEnable()
    //{
    //    manager.trackedImagesChanged += OnChanged;
    //}
    //void OnDisable()
    //{
    //    manager.trackedImagesChanged -= OnChanged;
    //}



    //void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    //{
    //    foreach (ARTrackedImage t in eventArgs.added)
    //    {
    //        UpdateImage(t);
    //    }
    //    foreach (ARTrackedImage t in eventArgs.updated)
    //    {
    //        UpdateImage(t);
    //    }
    //}

    //void UpdateImage(ARTrackedImage t)
    //{
    //    string name = t.referenceImage.name;

    //    GameObject o = dict1[name];
    //    o.transform.position = t.transform.position;
    //    o.transform.rotation = t.transform.rotation;
    //    o.SetActive(true);


    //    //if (dict1.TryGetValue(name, out GameObject o))
    //    //{
    //    //    o.transform.position = t.transform.position;
    //    //    o.transform.rotation = t.transform.rotation;
    //    //    o.SetActive(true);
    //    //}

    //}
    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        OnButtonClick(0);
    //    }
    //}

    public void OnButtonClick(int index)
    {

        if (index >= 0 && index < popupPrefab.Length)
        {
            if (popedIndex != -1)
                popupPrefab[popedIndex].SetActive(false);
            popedIndex = index;
            popupPrefab[index].SetActive(true);
            popupAudioSource.Play();
        }

    }
}
