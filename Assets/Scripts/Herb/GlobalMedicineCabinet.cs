using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalMedicineCabinet : MonoBehaviour
{
    public static GlobalMedicineCabinet Instance { get; private set; }

    //药匣panel
    public CanvasGroup HerbPanel;
    public Image PatientImage;
    public TextMeshProUGUI PatientName;
    public TextMeshProUGUI PatientDescription;
    public Button HideButton;

    //所有的herb
    public Image[] HerbImages;
    public Button[] HerbButtons;
    public TextMeshProUGUI[] HerbNames;

    //病人类型
    public EmotionType emotionType;
    public PatientData currentPatient;
    public DefaultPrescription currentPrescription;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Initialize();
        HidePanel(HerbPanel);
    }


    void Update()
    {

    }
    private void Initialize()
    {
        HideButton.onClick.RemoveAllListeners();
        HideButton.onClick.AddListener(() =>
        {
            HidePanel(HerbPanel);
        });
    }

    public void show()
    {
        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
        ShowPanel(HerbPanel);
        RefreshUI();
    }

    public void Hide()
    {
        HidePanel(HerbPanel);
    }

    private void RefreshUI()
    {
        
        this.currentPatient = CurrentPatient.data;
        this.PatientImage.sprite = currentPatient.Sprite;
        this.PatientName.text = currentPatient.PatientName;
        this.PatientDescription.text = currentPatient.Description;
        this.emotionType = currentPatient.Emotion;
        foreach (DefaultPrescription prescription in HerbDatabase.Instance.defaultPrescriptions)
        {
            if (prescription.emotion == currentPatient.Emotion)
            {
                currentPrescription = prescription;
                break;
            }
        }
        HerbData jun = HerbDatabase.Instance.GetHerb(currentPrescription.junHerbId);
        HerbData chen = HerbDatabase.Instance.GetHerb(currentPrescription.defaultChenId);
        HerbData zuo = HerbDatabase.Instance.GetHerb(currentPrescription.defaultZuoId);
        HerbData shi = HerbDatabase.Instance.GetHerb(currentPrescription.defaultShiId);
        SetDefaultHerb(0, jun);
        SetDefaultHerb(1, chen);
        SetDefaultHerb(2, zuo);
        SetDefaultHerb(3, shi);
    }

    private void SetDefaultHerb(int i, HerbData herb)
    {
        HerbImages[i].sprite = herb.herbIcon;
        HerbNames[i].text = herb.herbName;
    }

    public void HidePanel(CanvasGroup panel)
    {
        panel.alpha = 0;
        panel.interactable = false;
        panel.blocksRaycasts = false;
    }

    public void ShowPanel(CanvasGroup panel)
    {
        panel.alpha = 1;
        panel.interactable = true;
        panel.blocksRaycasts = true;
    }

    public void JumotoRhythmScene()
    {
        HidePanel(HerbPanel);
        SceneManager.LoadScene("MakeHerb");
    }

}
