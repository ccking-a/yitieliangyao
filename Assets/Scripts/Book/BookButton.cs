using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookButton : MonoBehaviour
{
    public Button buttonprefabs;
    void Start()
    {
        foreach(HerbCategory category in Enum.GetValues(typeof(HerbCategory)))
        {
            var btn = Instantiate(buttonprefabs, transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = category.ToString();
            btn.onClick.AddListener(() => FindObjectOfType<BookUI>().FiliterByCategory(category));

        }
    }

}
