using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BagManager : MonoBehaviour
{

    public Transform gridContainer;
    public GameObject giftItemPrefab;

    public CanvasGroup Panel;
    public CanvasGroup GiftPanel;
    public Image GiftIcon;
    public TextMeshProUGUI GiftName;
    public TextMeshProUGUI GiftDescription;

    void Start()
    {
        HidePanel(Panel);
        HidePanel(GiftPanel);
    }

    public void RefreshUI()
    {
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Gift gift in GameManager.Instance.Bag)
        {
            GameObject newgift = Instantiate(giftItemPrefab, gridContainer);
            newgift.GetComponent<GIftItem>().Initialize(gift, OnGiftOnclick);
        }
    }

    public void OnGiftOnclick(Gift gift)//打开礼物详情
    {
        ShowPanel(GiftPanel);
        GiftIcon.sprite = gift.sprite;
        GiftIcon.preserveAspect = true;
        GiftName.text = gift.name;
        GiftDescription.text = gift.description;
    }

    public void OnOpenBagPanel()//打开背包
    {
        ShowPanel(Panel);
        RefreshUI();
    }
    public void OnCloseBagPanle()//关闭背包
    {
        HidePanel(Panel);
        HidePanel(GiftPanel);
    }
    public void OnCloseGiftPanel()//关闭礼物panel
    {
        HidePanel(GiftPanel);
    }
    public void ShowPanel(CanvasGroup panel)
    {
        panel.alpha = 1;
        panel.interactable = true;
        panel.blocksRaycasts = true;
    }
    public void HidePanel(CanvasGroup panel)
    {
        panel.alpha = 0;
        panel.interactable = false;
        panel.blocksRaycasts = false;
    }
}
[System.Serializable]
public class Gift
{
    public PatientData patient;
    public string name;
    public string description;
    public Sprite sprite;
}
