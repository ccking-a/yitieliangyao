using UnityEngine;

[CreateAssetMenu(fileName = "DefaultPrescription", menuName = "TCM/DefaultPrescription")]
public class DefaultPrescription : ScriptableObject
{
    public EmotionType emotion;
    public string junHerbId;
    public string defaultChenId;
    public string defaultZuoId;
    public string defaultShiId;
}
