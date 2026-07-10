using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRhythmNote", menuName = "Game/RhythmNote")]
public class RhythmNote : ScriptableObject
{
    public List<float> timelist;
}
