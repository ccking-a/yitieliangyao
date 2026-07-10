using System;
using UnityEngine;

/// <summary>
/// 配置在「被治愈的病人」的 PatientData 上：该病人治好后，访客会在若干天后回医馆道谢。
/// </summary>
[Serializable]
public class ThankYouFollowUpRule
{
    public PatientData visitorPatient;
    [Min(0)]
    public int daysAfterTreatment = 2;
    public Line[] thankYouLines;
}

/// <summary>
/// 已排程的道谢事件，存放在 GameManager 中便于日后做存档（整表可序列化）。
/// lines 为排程时的快照，避免以后改表导致存档对不上。
/// </summary>
[Serializable]
public class PendingThankYouVisit
{
    public int visitorPatientId;
    public int scheduledClinicDay;
    public Line[] lines;
}
