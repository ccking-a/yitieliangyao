using System;
using UnityEngine;

[Serializable]
public class Line
{
    [TextArea(2, 5)]
    public string name;
    public string text;
    public AudioClip voiceClip;
    public Sprite expressionSprite;
    [Min(20f)]
    public float typingCharsPerSecond = 30f;
    [Min(3f)]
    public float autoNextDelay = 3f;
}
