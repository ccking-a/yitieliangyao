using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    public TextMeshProUGUI Day;
    public CanvasGroup FailPanel;
    public TextMeshProUGUI FailMessage;


    public CanvasGroup DayPanel;
    public CanvasGroup NightPanel;
    private void Start()
    {

        Initialize();

    }

    private void Initialize()
    {
        HidePanel(FailPanel);
        Day.text = "Day" + GameManager.Instance.Day;

        if (GameManager.Instance.DayPatient - GameManager.Instance.DayTreatedPatient > 0)
        {
            SetSunScene();
        }
        else
        {
            SetNightScene();
        }
    }

    public void Back()
    {
        SceneManager.LoadScene("GameStart");
    }

    public void SaveGame()
    {
        SaveManager.Instance.SaveGame();
    }

    private void SetSunScene()
    {
        ShowPanel(DayPanel);
        HidePanel(NightPanel);
    }

    private void SetNightScene()
    {
        HidePanel(DayPanel);
        ShowPanel(NightPanel);
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

    public void NextDay()
    {

        if (GameManager.Instance == null)
        {
            Debug.LogError("gamamanager instance is null");
            return;
        }

        if (GameManager.Instance.DayPatient - GameManager.Instance.DayTreatedPatient > 0)
        {
            StartCoroutine(ShowFailPanel("今天还有病人未完成治疗，先完成治疗吧", 1));
            return;
        }

        GameManager.Instance.Day++;
        GameManager.Instance.DayTreatedPatient = 0;
        GameManager.Instance.PruneExpiredThankYouVisits();
        Day.text = "Day " + GameManager.Instance.Day;
        SetSunScene();

        if(GameManager.Instance.Day == 3)
        {
            GameManager.Instance.DayPatient = 2;
        }

    }

    public void JumpToClinic()
    {
        StartCoroutine(LoadClinicScene());
    }
    public void JumpToBook()
    {
        StartCoroutine(LoadBookScene());
    }

    IEnumerator ShowFailPanel(string message, int duration)
    {
        ShowPanel(FailPanel);
        FailMessage.text = message;
        yield return new WaitForSeconds(duration);
        HidePanel(FailPanel);
    }

    IEnumerator LoadClinicScene()
    {
        SceneManager.LoadScene("Clinic");
        yield return null;
    }

    IEnumerator LoadBookScene()
    {
        SceneManager.LoadScene("Book");
        yield return null;

    }
}
