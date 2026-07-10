using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{

    public Button Mainbutton;
    public Button LoadGamebutton;
    public Button Settingbutton;

    public GameObject SettingPanel;
    public GameObject popSettingPanel;

    public TextMeshProUGUI[] Texts;
    void Start()
    {

        Initialize();


    }

    private void Initialize()
    {
        foreach(TextMeshProUGUI text in Texts)
        {
            text.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.2f);
            text.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, Color.black);
        }

        Mainbutton.onClick.AddListener(() =>
        {
            SaveManager.Instance.StartNewGame();
        });
        LoadGamebutton.onClick.AddListener(() =>
        {
            SaveManager.Instance.LoadGame();
        });
        Settingbutton.onClick.AddListener(() =>
        {
            popSettingPanel = Instantiate(SettingPanel, FindObjectOfType<Canvas>().transform);
        });



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
