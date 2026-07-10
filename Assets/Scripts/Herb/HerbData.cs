using System.Collections.Generic;
using UnityEngine;

// 药材种类枚举
public enum HerbCategory
{
    全部,
    解表药,   // 柴胡、薄荷
    清热药,   // 丹参
    补虚药,   // 白芍、白术、甘草、熟地、百合、麦冬
    安神药,   // 远志、茯神、五味子
    理气药,   // 陈皮
    利水渗湿药, // 茯苓
    收涩药,   // 山茱萸
    温里药,    // 肉桂
    活血化瘀药
}

// 药性枚举
public enum HerbNature
{
    寒, 凉, 平, 温, 热
}

// 五味枚举
public enum HerbFlavor
{
    酸, 苦, 甘, 辛, 咸
}

public enum ChannelTropism {
    心, 肝, 脾, 肺, 肾, 胃
}



// 药材数据资产
[CreateAssetMenu(fileName = "NewHerb", menuName = "TCM/HerbData")]
public class HerbData : ScriptableObject
{
    [Header("基础信息")]
    public string herbId;           // "bai_shao"
    public string herbName;         // "白芍"
    public HerbCategory category;
    public Sprite herbIcon;

    [Header("药性理论")]
    public HerbNature nature;
    public List<HerbFlavor> flavor;
    [TextArea(2, 4)] public string efficacy;  // "养血调经，敛阴止汗"

    [Header("归经")]
    public List<ChannelTropism> channelTropism;

    [Header("配伍关系")]
    public List<HerbInteraction> interactions;  // 相须/相使/相恶/相反

    [Header("游戏数据")]
    public int baseScore;         // 基础得分
    public int unlockChapter;     // 解锁章节
    public int price;             // 购买价格（若有商店）
}

// 配伍关系
[System.Serializable]
public class HerbInteraction
{
    public string targetHerbId;   // 配伍药材ID
    public InteractionType type;  // 相须/相使/相恶/相反
    public int bonusScore;        // 额外分数
}

public enum InteractionType
{
    相须,   // 同类协同，增强药效 (+15分)
    相使,   // 主辅协同 (+10分)
    相恶,   // 拮抗减效 (-10分)
    相反   // 毒性反应 (-20分)
}