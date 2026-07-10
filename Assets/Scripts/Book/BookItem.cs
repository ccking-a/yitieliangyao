using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookItem : MonoBehaviour
{
    [SerializeField] public Image herbicon;
    [SerializeField] public TextMeshProUGUI herbname;
    private HerbData herbdata;
    private System.Action<HerbData> onselected;
    public void Init(HerbData herb,System.Action<HerbData> onselect)
    {
        this.herbdata = herb;
        this.herbicon.sprite = herbdata.herbIcon;
        this.herbname.text = herbdata.herbName;
        this.onselected = onselect;

        GetComponent<Button>().onClick.AddListener(() => { onselected?.Invoke(herbdata); });
    }
}
