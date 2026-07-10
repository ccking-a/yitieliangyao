using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BookUI : MonoBehaviour
{
    [SerializeField] private Transform gridContainer;
    [SerializeField] private GameObject herbItemPrefab;
    [SerializeField] private CanvasGroup herbPanel;
    [SerializeField] private TextMeshProUGUI detailText;
    [SerializeField] private Image detailImage;
    private List<HerbData> herbDatas;
    private List<HerbData> currentHerbDatas;
    void Start()
    {
        this.herbDatas = HerbDatabase.Instance.allHerbs;
        this.currentHerbDatas = herbDatas;
        RefreshUI();
        HidePanel();
    }

    public void FiliterByCategory(HerbCategory category)
    {
        if(category == HerbCategory.全部)
        {
            this.currentHerbDatas = herbDatas;
        }
        else
        {
            this.currentHerbDatas = herbDatas.Where(h => h.category == category).ToList();
        }
        RefreshUI();
    }


    public void ToMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void RefreshUI()
    {

        foreach(Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (HerbData herbData in this.currentHerbDatas)
        {
            GameObject herbitem = Instantiate(herbItemPrefab, gridContainer);
            herbitem.GetComponent<BookItem>().Init(herbData, OnHerbClicked);
        }
    }
    public void OnHerbClicked(HerbData herb)
    {
        ShowPanel(herbPanel);
        detailImage.sprite = herb.herbIcon;
        detailImage.preserveAspect = true;
        detailText.text = $"名称: {herb.herbName}\n" +
                $"种类: {herb.category}\n" +
                $"药性: {herb.nature}\n" +
                $"五味: {string.Join(", ", herb.flavor)}\n" +
                $"作用: {herb.efficacy}\n" +
                $"归经: {string.Join(", ", herb.channelTropism)}\n";
    }

    public void ShowPanel(CanvasGroup panel)
    {
        panel.alpha = 1;
        panel.interactable = true;
        panel.blocksRaycasts = true;
    }
    public void HidePanel()
    {
        herbPanel.alpha = 0;
        herbPanel.interactable = false;
        herbPanel.blocksRaycasts = false;
    }
}
