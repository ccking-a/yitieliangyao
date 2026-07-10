using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager
{
    /// <summary>
    /// 当前播放协程句柄。注意：Unity 在协程结束后不会把此引用自动变成 null，
    /// 必须在协程收尾时手动清空，否则无法用 playRoutine != null 判断“是否还在播”。
    /// </summary>
    public Coroutine playRoutine;

    public bool IsPlaying => playRoutine != null;

    public void PlayDialogue(
        MonoBehaviour host,
        Line[] lines,
        TextMeshProUGUI name,
        TextMeshProUGUI dialogueText,
        Image expressionImage,
        AudioSource voiceAudioSource,
        Func<bool> consumeAdvanceRequest)
    {
        StopDialogue(host, voiceAudioSource);

        if (host == null)
        {
            Debug.LogWarning("Host is null, cannot play  dialogue.");
            return;
        }

        playRoutine = host.StartCoroutine(PlayDialogueWithCleanup(
            lines,
            name,
            dialogueText,
            expressionImage,
            voiceAudioSource,
            consumeAdvanceRequest));
    }

    private IEnumerator PlayDialogueWithCleanup(
        Line[] lines,
        TextMeshProUGUI name,
        TextMeshProUGUI dialogueText,
        Image expressionImage,
        AudioSource voiceAudioSource,
        Func<bool> consumeAdvanceRequest)
    {
        try
        {
            yield return PlayDialogueRoutine(
                lines,
                name,
                dialogueText,
                expressionImage,
                voiceAudioSource,
                consumeAdvanceRequest);
        }
        finally
        {
            playRoutine = null;
        }
    }

    public void StopDialogue(MonoBehaviour host, AudioSource voiceAudioSource)
    {
        if (host != null && playRoutine != null)
        {
            host.StopCoroutine(playRoutine);
            playRoutine = null;
        }

        if (voiceAudioSource != null && voiceAudioSource.isPlaying)
        {
            voiceAudioSource.Stop();
        }
    }

    private IEnumerator PlayDialogueRoutine(
        Line[] lines,
        TextMeshProUGUI name,
        TextMeshProUGUI dialogueText,
        Image expressionImage,
        AudioSource voiceAudioSource,
        Func<bool> consumeAdvanceRequest)
    {
        if (lines == null || lines.Length == 0)
        {
            if (dialogueText != null)
            {
                dialogueText.text = string.Empty;
            }

            Debug.LogWarning("Dialogue lines are empty.");
            yield break;
        }

        for (int i = 0; i < lines.Length; i++)
        {
            Line line = lines[i];
            if (line == null)
            {
                continue;
            }

            if (name != null)
            {
                name.text = line.name ?? string.Empty;
            }


            if (expressionImage != null)
            {
                expressionImage.sprite = line.expressionSprite ?? null;
                if(line.expressionSprite == null)
                {
                    expressionImage.color = new Color(1, 1, 1, 0);
                }
                else
                {
                    expressionImage.color = new Color(1, 1, 1, 1);
                }
            }

            if (voiceAudioSource != null && line.voiceClip != null)
            {
                voiceAudioSource.clip = line.voiceClip;
                voiceAudioSource.volume = AudioManager.Instance.sfxVolume;
                voiceAudioSource.Play();
            }

            yield return TypeLine(line.text, line.typingCharsPerSecond, dialogueText, consumeAdvanceRequest);

            if (consumeAdvanceRequest != null && consumeAdvanceRequest())
            {
                if (voiceAudioSource != null && voiceAudioSource.isPlaying)
                {
                    voiceAudioSource.Stop();
                }

                continue;
            }

            if (voiceAudioSource != null && line.voiceClip != null)
            {
                while (voiceAudioSource.isPlaying)
                {
                    if (consumeAdvanceRequest != null && consumeAdvanceRequest())
                    {
                        voiceAudioSource.Stop();
                        break;
                    }

                    yield return null;
                }
            }

            if (line.autoNextDelay > 0f)
            {
                float wait = 0f;
                while (wait < line.autoNextDelay)
                {
                    if (consumeAdvanceRequest != null && consumeAdvanceRequest())
                    {
                        break;
                    }

                    wait += Time.deltaTime;
                    yield return null;
                }
            }
        }
    }

    private IEnumerator TypeLine(
        string text,
        float charsPerSecond,
        TextMeshProUGUI dialogueText,
        Func<bool> consumeAdvanceRequest)
    {
        string safeText = text ?? string.Empty;
        if (dialogueText != null)
        {
            dialogueText.text = string.Empty;
        }

        if (safeText.Length == 0)
        {
            yield break;
        }

        int shownChars = 0;
        float interval = 1f / Mathf.Max(1f, charsPerSecond);
        while (shownChars < safeText.Length)
        {
            if (consumeAdvanceRequest != null && consumeAdvanceRequest())
            {
                if (dialogueText != null)
                {
                    dialogueText.text = safeText;
                }

                yield break;
            }

            shownChars++;
            if (dialogueText != null)
            {
                dialogueText.text = safeText.Substring(0, shownChars);
            }

            yield return new WaitForSeconds(interval);
        }
    }
}
