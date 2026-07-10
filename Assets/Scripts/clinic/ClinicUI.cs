using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClinicUI : MonoBehaviour
{

    public CanvasGroup DayImage;
    public CanvasGroup NightImage;

    public Button[] PatientsButton;
    public TextMeshProUGUI DayText;
    public Image[] PatientImage;


    [SerializeField] private ClinicThankYouFollowUpUI thankYouFollowUp;

    //前五天为五脏教程，后续病人按照队列顺序出现。
    public PatientData[] patientDatas;//病人数据
    public List<PatientData> currentClinicPatient;
    public List<int> treatedPatient;//已经治疗的病人id

    private void Start()
    {
        StartCoroutine(ClinicEntryRoutine());
    }

    private IEnumerator ClinicEntryRoutine()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("ClinicUI: GameManager.Instance 为空，无法初始化医馆。");
            yield break;
        }

        Initialize();
        yield return SetPatient();
        if (thankYouFollowUp != null)
        {
            yield return thankYouFollowUp.RunPendingVisits(this);
        }
    }

    private void Initialize()
    {
        if (GameManager.Instance == null)
        {
            return;
        }


        if (GameManager.Instance.DayPatient - GameManager.Instance.DayTreatedPatient > 0)
        {
            SetSunScene();
        }
        else
        {
            SetNightScene();
        }

        this.patientDatas = GameManager.Instance.patientDatas;
        this.treatedPatient = GameManager.Instance.treatedPatient;
        this.currentClinicPatient = new List<PatientData>();

        this.DayText.text = "第" + GameManager.Instance.Day + "天" + ",今日已治愈" + GameManager.Instance.DayTreatedPatient + "/" + GameManager.Instance.DayPatient + "人";

    }

    public void RefreshUI()
    {

        for (int j = 0; j < 2; j++)
        {
            PatientImage[j].sprite = null;
            PatientImage[j].color = new Color(1, 1, 1, 0);
            PatientImage[j].preserveAspect = true;
        }

        int i = 0;
        foreach (PatientData patientData in currentClinicPatient)
        {
            if (patientData == null || PatientsButton == null || i >= PatientsButton.Length - 1)
            {
                continue;
            }

            if (patientData.Sprite != null) PatientsButton[i].GetComponent<Image>().sprite = patientData.Sprite;
            PatientsButton[i].GetComponentInChildren<TextMeshProUGUI>().text = patientData.PatientName;
            PatientsButton[i].onClick.RemoveAllListeners();
            PatientsButton[i].onClick.AddListener(() =>
            {
                CurrentPatient.SetCurrentPatient(patientData);
                //唤起确认面板然后开始诊治
                JumpToDiagnosis();
            });
            PatientImage[i].sprite = patientData.lihui;
            PatientImage[i].color = new Color(1,1,1,1);

            i++;
        }
        for (; i <= 2; i++)
        {
            PatientsButton[i].gameObject.SetActive(false);
        }
    }

    IEnumerator SetPatient()
    {
        yield return new WaitForSeconds(0.1f);

        if (patientDatas == null)
        {
            Debug.LogWarning("ClinicUI: patientDatas 为空，跳过病人列表刷新。");
            yield break;
        }

        int daypatient = 0;

        foreach (PatientData patient in patientDatas)
        {
            if (patient == null)
            {
                continue;
            }
            if (daypatient >= GameManager.Instance.DayPatient - GameManager.Instance.DayTreatedPatient)
            {
                break;
            }
            if (!treatedPatient.Contains(patient.PatientId))
            {
                currentClinicPatient.Add(patient);
                daypatient++;
            }
        }
        RefreshUI();
    }
    public void JumpToDiagnosis()
    {
        StartCoroutine(LoadDiagnosisScene());
    }
    public void JumpToMainScene()
    {
        StartCoroutine(LoadMainScene());
    }

    IEnumerator LoadDiagnosisScene()
    {
        SceneManager.LoadScene("Diagnosis");
        yield return null;
    }
    IEnumerator LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
        yield return null;

    }

    private void SetSunScene()
    {
        ShowPanel(DayImage);
        HidePanel(NightImage);
    }

    private void SetNightScene()
    {
        HidePanel(DayImage);
        ShowPanel(NightImage);
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

}
