using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GIftItem : MonoBehaviour
{
    public TextMeshProUGUI name;
    public Image icon;
    public Gift gift;
    public System.Action<Gift> onclick;

    public void Initialize(Gift gift ,System.Action<Gift> action)
    {
        this.gift = gift;
        this.name.text = gift.name;
        this.icon.sprite = gift.sprite;
        this.onclick = action;
        GetComponent<Button>().onClick.AddListener(() => { onclick?.Invoke(gift); });
    }

}
