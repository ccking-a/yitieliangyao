using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //存储的数据
    //病人数据列表
    public PatientData[] patientDatas;//所有的病人数据
    public List<int> treatedPatient;//已经治疗的病人id
    public List<Gift> Bag;//礼物列表

    //排程中的道谢访问列表（可能包含多个未来访问，约定日相同的访问按添加顺序依次访问）
    public List<PendingThankYouVisit> pendingThankYouVisits = new List<PendingThankYouVisit>();

    public int TotalScore;

    public int Day=1;
    public int DayPatient = 1;
    public int DayTreatedPatient;
    public int currentChapter;

    //新手引导
    public bool FirstTimeInMainScene = false;
    public bool FirstTimeToSleep = false;
    public bool FirstTimeInClinic = false;
    public bool FirstTimeInDiagnosis = false;
    public bool FirstTimeInMakeHerb = false;

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

    public PatientData GetPatientDataById(int patientId)
    {
        if (patientDatas == null)
        {
            return null;
        }

        foreach (PatientData p in patientDatas)
        {
            if (p != null && p.PatientId == patientId)
            {
                return p;
            }
        }

        return null;
    }

    /// <summary>
    /// 进入医馆等时机调用：当前游戏日已超过约定日则丢弃（过期不再出现）。
    /// </summary>
    public void PruneExpiredThankYouVisits()
    {
        if (pendingThankYouVisits == null)
        {
            pendingThankYouVisits = new List<PendingThankYouVisit>();
            return;
        }

        pendingThankYouVisits.RemoveAll(v => v != null && v.scheduledClinicDay < Day);
    }

    /// <summary>
    /// 某病人治愈成功时调用：根据该病人 PatientData 上的规则写入排程。
    /// 约定日 = 治愈当日的 GameManager.Day + daysAfterTreatment。
    /// </summary>
    public void ScheduleThankYouFollowUpsAfterCure(PatientData curedPatient)
    {
        if (curedPatient == null
            || curedPatient.thankYouFollowUpsWhenCured == null
            || curedPatient.thankYouFollowUpsWhenCured.Length == 0)
        {
            return;
        }

        if (pendingThankYouVisits == null)
        {
            pendingThankYouVisits = new List<PendingThankYouVisit>();
        }

        foreach (ThankYouFollowUpRule rule in curedPatient.thankYouFollowUpsWhenCured)
        {
            if (rule == null || rule.visitorPatient == null)
            {
                continue;
            }

            if (rule.thankYouLines == null || rule.thankYouLines.Length == 0)
            {
                continue;
            }

            if (!LinesHaveThankYouContent(rule.thankYouLines))
            {
                continue;
            }

            Line[] snapshot = SnapshotLines(rule.thankYouLines);
            if (!LinesHaveThankYouContent(snapshot))
            {
                continue;
            }

            pendingThankYouVisits.Add(new PendingThankYouVisit
            {
                visitorPatientId = rule.visitorPatient.PatientId,
                scheduledClinicDay = Day + rule.daysAfterTreatment,
                lines = snapshot,
            });
        }
    }

    /// <summary>
    /// 至少存在一句可播放内容（非空文本 / 语音 / 表情其一），用于避免未配置道谢的病人或空表导致报错。
    /// </summary>
    public static bool LinesHaveThankYouContent(Line[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            return false;
        }

        foreach (Line line in lines)
        {
            if (line == null)
            {
                continue;
            }

            if (!string.IsNullOrEmpty(line.text))
            {
                return true;
            }

            if (line.voiceClip != null)
            {
                return true;
            }

            if (line.expressionSprite != null)
            {
                return true;
            }
        }

        return false;
    }

    public PendingThankYouVisit FindNextThankYouForToday()
    {
        if (pendingThankYouVisits == null)
        {
            return null;
        }

        foreach (PendingThankYouVisit v in pendingThankYouVisits)
        {
            if (v != null && v.scheduledClinicDay == Day)
            {
                return v;
            }
        }

        return null;
    }

    public void RemoveThankYouVisit(PendingThankYouVisit visit)
    {
        if (visit == null || pendingThankYouVisits == null)
        {
            return;
        }

        pendingThankYouVisits.Remove(visit);
    }

    private static Line[] SnapshotLines(Line[] src)
    {
        if (src == null)
        {
            return null;
        }

        Line[] dst = new Line[src.Length];
        for (int i = 0; i < src.Length; i++)
        {
            Line s = src[i];
            if (s == null)
            {
                dst[i] = null;
                continue;
            }

            dst[i] = new Line
            {
                name = s.name,
                text = s.text,
                voiceClip = s.voiceClip,
                expressionSprite = s.expressionSprite,
                typingCharsPerSecond = s.typingCharsPerSecond,
                autoNextDelay = s.autoNextDelay,
            };
        }

        return dst;
    }
}
