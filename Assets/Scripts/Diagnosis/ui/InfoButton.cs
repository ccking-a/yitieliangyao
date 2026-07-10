using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InfoButton : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI text;
    public EmotionType emotion;
    public Button button;
    private void Awake()
    {
        this.button = GetComponent<Button>();
        this.text = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void setup(ButtonData buttonData, DIagosisUI diagnosisUI)
    {
        this.emotion = buttonData.emotion;
        this.text.text = buttonData.emotion.ToString();
        this.button.image.sprite = buttonData.icon;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            Debug.Log("点击了" + this.text.text);
            if (CurrentPatient.data.Emotion == this.emotion)
            {
                CurrentPatient.isDiagnosis = true;
                GlobalMedicineCabinet.Instance.show();
            }
            else
            {
                diagnosisUI?.ShowDiagnosisFail();
            }
        });
    }
}
[System.Serializable]
public class ButtonData
{
    public EmotionType emotion;
    public Sprite icon;
}
