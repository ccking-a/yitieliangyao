using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCreator : MonoBehaviour
{
    [SerializeField]
    public ButtonData[] emotionbuttons;
    public GameObject buttonprefab;
    public Transform buttonparent;
    private void Start()
    {

    }

    public void CreateButton(DIagosisUI diagnosisUI)
    {
        foreach (ButtonData buttonData in emotionbuttons)
        {
            GameObject newbutton = Instantiate(buttonprefab, buttonparent);
            newbutton.GetComponent<InfoButton>().setup(buttonData, diagnosisUI);
        }
    }

}
