using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RhythmUI : MonoBehaviour
{
    public TextMeshProUGUI RhythmPatientName;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI SuccessText;
    public Image expressionImage;
    public AudioSource voiceAudioSource;
    public CanvasGroup RhythmPanel;
    public CanvasGroup SuccessPanel;
    public CanvasGroup FailPanel;
    public GameObject neihuan;
    public Button ReStartRhythmButton;
    public Button ReChooseHerbButton;
    public RhythmNoteManager rhythmNoteManager;

    private bool advanceRequested;
    private DialogueManager dialogueManager;
    void Start()
    {
        Initialize();

    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            advanceRequested = true;
        }
    }

    public void Initialize()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager is missing, cannot play diagnosis dialogue.");
            return;
        }

        if (CurrentPatient.data == null)
        {
            Debug.LogWarning("Current patient is empty, cannot play diagnosis dialogue.");
            return;
        }

        if (voiceAudioSource == null)
        {
            voiceAudioSource = GetComponent<AudioSource>();
        }
        dialogueManager = new DialogueManager();

        if (RhythmPatientName != null)
        {
            RhythmPatientName.text = "正在为" + CurrentPatient.data.PatientName + "捣药";
        }

        ShowPanel(RhythmPanel);
        SetNeihuanVisibleForScene();
        HidePanel(SuccessPanel);
        HidePanel(FailPanel);

        ReStartRhythmButton.onClick.RemoveAllListeners();
        ReStartRhythmButton.onClick.AddListener(() =>
        {
            ShowPanel(RhythmPanel);
            neihuan.SetActive(true);
            HidePanel(FailPanel);
            rhythmNoteManager.ReStartRhythm();

        });

        ReChooseHerbButton.onClick.RemoveAllListeners();
        ReChooseHerbButton.onClick.AddListener(() =>
        {
            neihuan.SetActive(false);
            HidePanel(FailPanel);
            GlobalMedicineCabinet.Instance.show();
        });

    }

    public void StartSuccessTreated(float score)
    {
        HidePanel(RhythmPanel);
        neihuan.SetActive(false);


        SuccessDialogAsset line = new SuccessDialogAsset();

        if (score < CurrentPatient.data.TargetRhythmScore * 0.4f)
        {
            line = CurrentPatient.data.MissDialog;
        }
        else if (score >= CurrentPatient.data.TargetRhythmScore * 0.8)
        {
            line = CurrentPatient.data.SuccessDialog;
        }
        else if (score >= CurrentPatient.data.TargetRhythmScore * 0.4f)
        {
            line = CurrentPatient.data.GoodDialog;
        }

        advanceRequested = false;
        ShowPanel(SuccessPanel);
        AudioManager.Instance.UnPauseBgm();

        StartCoroutine(BeforeRhythm(line,score));

    }

    IEnumerator BeforeRhythm(SuccessDialogAsset line,float score)
    {
        Line[] lines = CurrentPatient.data.SuccessDialog == null ? null : line.lines;
        dialogueManager.PlayDialogue(
            this,
            lines,
            NameText,
            SuccessText,
            expressionImage,
            voiceAudioSource,
            ConsumeAdvanceRequest);
        while (dialogueManager.IsPlaying)
        {
            yield return null;
        }

        if (score < CurrentPatient.data.TargetRhythmScore * 0.4f)
        {
            StartFailTreated();
            yield break;
        }

        CurrentPatient.isTreatment = true;
        if (GameManager.Instance != null && CurrentPatient.data != null)
        {
            GameManager.Instance.treatedPatient.Add(CurrentPatient.data.PatientId);
            GameManager.Instance.DayTreatedPatient++;
            GameManager.Instance.ScheduleThankYouFollowUpsAfterCure(CurrentPatient.data);
        }

        StartCoroutine(LoadClinicScene());
    }

    private IEnumerator LoadClinicScene()
    {
        while (dialogueManager.playRoutine != null)
        {
            yield return null;
        }
        SceneManager.LoadScene("Clinic");
    }

    public void StartFailTreated()
    {
        HidePanel(RhythmPanel);
        HidePanel(SuccessPanel);
        neihuan.SetActive(false);
        ShowPanel(FailPanel);
    }


    private bool ConsumeAdvanceRequest()
    {
        if (!advanceRequested)
        {
            return false;
        }

        advanceRequested = false;
        return true;
    }

    private void OnDisable()
    {
        if (dialogueManager != null)
        {
            dialogueManager.StopDialogue(this, voiceAudioSource);
        }
    }

    private void SetNeihuanVisibleForScene()
    {
        if (neihuan == null)
        {
            return;
        }

        bool show = GameManager.Instance == null || GameManager.Instance.FirstTimeInMakeHerb;
        neihuan.SetActive(show);
    }

    public void HidePanel(CanvasGroup panel)
    {
        panel.alpha = 0;           // 完全透明
        panel.interactable = false; // 不可点击
        panel.blocksRaycasts = false; // 不阻挡射线
    }

    // 显示 Panel
    public void ShowPanel(CanvasGroup panel)
    {
        panel.alpha = 1;
        panel.interactable = true;
        panel.blocksRaycasts = true;
    }
}
