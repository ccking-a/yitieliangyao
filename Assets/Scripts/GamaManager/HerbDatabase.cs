// 药材数据库（单例）
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HerbDatabase : MonoBehaviour
{
    public static HerbDatabase Instance { get; private set; }

    [SerializeField] public List<HerbData> allHerbs;
    public DefaultPrescription[] defaultPrescriptions;//默认方剂列表

    // 快速查询字典
    private Dictionary<string, HerbData> herbDict;
    private Dictionary<HerbCategory, List<HerbData>> herbsByCategory;

    void Awake()
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
        BuildIndex();
    }

    void BuildIndex()
    {
        herbDict = new Dictionary<string, HerbData>();
        herbsByCategory = new Dictionary<HerbCategory, List<HerbData>>();

        foreach (var herb in allHerbs)
        {
            herbDict[herb.herbId] = herb;

            // 按类别分组
            if (!herbsByCategory.ContainsKey(herb.category))
                herbsByCategory[herb.category] = new List<HerbData>();
            herbsByCategory[herb.category].Add(herb);

        }
    }

    // 按ID获取
    public HerbData GetHerb(string herbId) => herbDict.GetValueOrDefault(herbId);

}