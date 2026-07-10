using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    public GameObject tishipanel;
    public GameObject Poptishipanel;
    public Transform poptishipanelTransform;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        
    }

    public void StartNewGame()
    {
        GameManager.Instance.treatedPatient = new List<int>();
        GameManager.Instance.pendingThankYouVisits = new List<PendingThankYouVisit>();
        GameManager.Instance.Bag = new List<Gift>();
        GameManager.Instance.TotalScore = 0;
        GameManager.Instance.Day = 1;
        GameManager.Instance.DayPatient = 1;
        GameManager.Instance.DayTreatedPatient = 0;
        GameManager.Instance.currentChapter = 1;
        GameManager.Instance.FirstTimeInMainScene = false;
        GameManager.Instance.FirstTimeToSleep = false;
        GameManager.Instance.FirstTimeInClinic = false;
        GameManager.Instance.FirstTimeInDiagnosis = false;
        GameManager.Instance.FirstTimeInMakeHerb = false;
        SceneManager.LoadScene("MainScene");
    }

    public void SaveGame()
    {
        SavaData data = new SavaData
        {
            treatedPatient = GameManager.Instance.treatedPatient,
            pendingThankYouVisits = GameManager.Instance.pendingThankYouVisits,
            Bag = GameManager.Instance.Bag,
            TotalScore = GameManager.Instance.TotalScore,
            Day = GameManager.Instance.Day,
            DayPatient = GameManager.Instance.DayPatient,
            DayTreatedPatient = GameManager.Instance.DayTreatedPatient,
            currentChapter = GameManager.Instance.currentChapter,
            FirstTimeInMainScene = GameManager.Instance.FirstTimeInMainScene,
            FirstTimeToSleep = GameManager.Instance.FirstTimeToSleep,
            FirstTimeInClinic = GameManager.Instance.FirstTimeInClinic,
            FirstTimeInDiagnosis = GameManager.Instance.FirstTimeInDiagnosis,
            FirstTimeInMakeHerb = GameManager.Instance.FirstTimeInMakeHerb
        };
        string json = JsonUtility.ToJson(data, true);

        string filePath = Application.persistentDataPath + "saveData.json";
        File.WriteAllText(filePath, json);

        poptishipanelTransform = FindObjectOfType<Canvas>().transform;
        Poptishipanel = Instantiate(tishipanel,poptishipanelTransform);
        Poptishipanel.GetComponentInChildren<TextMeshProUGUI>().text = "保存成功！";
        StartCoroutine(DestroyPopTishiPanel());

        Debug.Log("Game saved to: " + filePath);
    }

    IEnumerator DestroyPopTishiPanel()
    {
        yield return new WaitForSeconds(1.5f);
        if (Poptishipanel != null)
        {
            Destroy(Poptishipanel);
        }
    }

    public void LoadGame()
    {
        string filePath = Application.persistentDataPath + "saveData.json";
        if (!File.Exists(filePath))
        {
            Debug.LogError("Save file not found: " + filePath);
            poptishipanelTransform = FindObjectOfType<Canvas>().transform;
            Poptishipanel = Instantiate(tishipanel, poptishipanelTransform);
            Poptishipanel.GetComponentInChildren<TextMeshProUGUI>().text = "未找到存档！";
            StartCoroutine(DestroyPopTishiPanel());
            return;
        }
        string json = File.ReadAllText(filePath);
        SavaData data = JsonUtility.FromJson<SavaData>(json);

        GameManager.Instance.treatedPatient = data.treatedPatient;
        GameManager.Instance.pendingThankYouVisits = data.pendingThankYouVisits;
        GameManager.Instance.Bag = data.Bag; 
        GameManager.Instance.TotalScore = data.TotalScore;
        GameManager.Instance.Day = data.Day;
        GameManager.Instance.DayPatient = data.DayPatient;
        GameManager.Instance.DayTreatedPatient = data.DayTreatedPatient;
        GameManager.Instance.currentChapter = data.currentChapter;
        GameManager.Instance.FirstTimeInMainScene = data.FirstTimeInMainScene;
        GameManager.Instance.FirstTimeToSleep = data.FirstTimeToSleep;
        GameManager.Instance.FirstTimeInClinic = data.FirstTimeInClinic;
        GameManager.Instance.FirstTimeInDiagnosis = data.FirstTimeInDiagnosis;
        GameManager.Instance.FirstTimeInMakeHerb = data.FirstTimeInMakeHerb;

        SceneManager.LoadScene("MainScene");
    }
}

[System.Serializable]
public class SavaData 
{
    public PatientData[] patientDatas;//所有的病人数据
    public List<int> treatedPatient;//已经治疗的病人id
    public List<Gift> Bag;//礼物列表

    //排程中的道谢访问列表（可能包含多个未来访问，约定日相同的访问按添加顺序依次访问）
    public List<PendingThankYouVisit> pendingThankYouVisits = new List<PendingThankYouVisit>();

    public int TotalScore;

    public int Day = 1;
    public int DayPatient = 1;
    public int DayTreatedPatient;
    public int currentChapter;

    //新手引导
    public bool FirstTimeInMainScene;
    public bool FirstTimeToSleep;
    public bool FirstTimeInClinic;
    public bool FirstTimeInDiagnosis ;
    public bool FirstTimeInMakeHerb;
}
