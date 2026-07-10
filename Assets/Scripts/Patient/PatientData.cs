using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPatientData", menuName = "Game/PatientData")]
public class PatientData  : ScriptableObject
{
    public string PatientName;
    public int PatientId;
    public EmotionType Emotion;
    public string Description;
    public DiagnosisDialogueAsset DiagnosisDialogue;
    public string DiagnosisFailText;
    public SuccessDialogAsset SuccessDialog;
    public SuccessDialogAsset GoodDialog;
    public SuccessDialogAsset MissDialog;
    public Sprite Sprite;
    public Sprite lihui;
    public Sprite touxiang2;
    public RhythmNote rhythmNote;
    public int TargetRhythmScore;
    public AudioClip rhythmclip;
    public Gift gift;
    public float difficulty;

    [Tooltip("当本病人被治愈后，按规则在若干天后回医馆道谢的访客与台词（在 GameManager 中排程）。")]
    public ThankYouFollowUpRule[] thankYouFollowUpsWhenCured;
}

public enum EmotionType
{
    怒,
    喜,
    思,
    悲,
    恐,
}
