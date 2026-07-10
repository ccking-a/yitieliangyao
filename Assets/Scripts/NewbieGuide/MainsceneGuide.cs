using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainsceneGuide : MonoBehaviour
{

    public bool advanceRequested;

    public int id;
    public CanvasGroup tishipanel;


    public CanvasGroup NewbiePanel;
    public DiagnosisDialogueAsset newbietext1;
    public DiagnosisDialogueAsset newbietextSleep;
    public TextMeshProUGUI name1;
    public TextMeshProUGUI text1;
    public Image image1;
    public Button requestedButton1;
    public AudioSource voice1;
    public DialogueManager dialogueManager1 = new DialogueManager();

    public Button FirstToSleep;

    public GameObject neihuan;

    void Start()
    {
        HidePanel(NewbiePanel);
        HidePanel(tishipanel);

        requestedButton1.onClick.RemoveAllListeners();
        requestedButton1.onClick.AddListener(() => advanceRequested = true);
        if (SceneManager.GetActiveScene().name != "Diagnosis")
        {
            requestedButton1.gameObject.SetActive(false);
        }

        if (!GameManager.Instance.FirstTimeInMainScene && SceneManager.GetActiveScene().name == "MainScene")
        {
            id = 1;
            ShowPanel(NewbiePanel);
            StartCoroutine(Newbie("点击木门进入诊所"));
        }
        if (!GameManager.Instance.FirstTimeToSleep && SceneManager.GetActiveScene().name == "MainScene" && GameManager.Instance.FirstTimeInMainScene)
        {
            id = 4;
            ShowPanel(NewbiePanel);
            if (FirstToSleep != null)
            {
                FirstToSleep.gameObject.SetActive(false);
            }
            StartCoroutine(Newbie("点击床铺开始睡觉"));
        }
        if (!GameManager.Instance.FirstTimeInClinic && SceneManager.GetActiveScene().name == "Clinic")
        {
            id = 2;
            ShowPanel(NewbiePanel);
            StartCoroutine(Newbie("点击张屠户头像为其诊断"));
        }
        if (!GameManager.Instance.FirstTimeInMakeHerb && SceneManager.GetActiveScene().name == "MakeHerb")
        {
            if (neihuan != null)
            {
                neihuan.SetActive(false);
            }
            id = 3;
            ShowPanel(NewbiePanel);
            StartCoroutine(Newbie("点击节奏按钮开始节奏游戏"));
        }

    }

    void Update()
    {

    }

    public void FirstDiagnosis(Action complete)
    {
        if (GameManager.Instance == null)
        {
            return;
        }
        StartCoroutine(NewbieToDiagnosis(complete));
    }

    private IEnumerator Newbie(string tishi)
    {
        float duration = 1f;
        for (float i = 0; i < duration; i += Time.deltaTime)
        {
            image1.color = new Color(1f, 1f, 1f, i / duration);
            yield return null;
        }

        requestedButton1.gameObject.SetActive(true);

        if (id == 4)
        {
            dialogueManager1.PlayDialogue(
          this, newbietextSleep.lines, name1, text1, image1, voice1, ConsumeAdvanceRequest);
        }
        else
        {
            dialogueManager1.PlayDialogue(
                       this, newbietext1.lines, name1, text1, image1, voice1, ConsumeAdvanceRequest);
        }

        while (dialogueManager1.IsPlaying)
        {
            yield return null;
        }
        image1.gameObject.SetActive(false);
        requestedButton1.gameObject.SetActive(false);
        ShowPanel(tishipanel);
        tishipanel.GetComponentInChildren<TextMeshProUGUI>().text = tishi;
        if (id == 1)
        {
            GameManager.Instance.FirstTimeInMainScene = true;
        }
        if (id == 2)
        {
            NewbiePanel.blocksRaycasts = false;
            GameManager.Instance.FirstTimeInClinic = true;
        }
        if (id == 3)
        {
            HidePanel(NewbiePanel);
            HidePanel(tishipanel);
            neihuan.SetActive(true);
            GameManager.Instance.FirstTimeInMakeHerb = true;
        }
        if (id == 4)
        {
            HidePanel(NewbiePanel);
            HidePanel(tishipanel);
            GameManager.Instance.FirstTimeToSleep = true;
        }

    }
    public IEnumerator NewbieToDiagnosis(Action complete)
    {
        //float duration = 1f;
        //for (float i = 0; i < duration; i += Time.deltaTime)
        //{
        //    image1.color = new Color(1f, 1f, 1f, i / duration);
        //    yield return null;
        //}
        if (GameManager.Instance.FirstTimeInDiagnosis)
        {
            complete?.Invoke();
            yield break;
        }
        yield return new WaitForSeconds(0.5f);
        requestedButton1.gameObject.SetActive(true);
        GameManager.Instance.FirstTimeInDiagnosis = true;
        dialogueManager1.PlayDialogue(
           this, newbietext1.lines, name1, text1, image1, voice1, ConsumeAdvanceRequest);
        while (dialogueManager1.IsPlaying)
        {
            yield return null;
        }
        complete?.Invoke();
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
