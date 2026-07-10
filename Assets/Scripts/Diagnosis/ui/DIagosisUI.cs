using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DIagosisUI : MonoBehaviour
{
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI dialogueText;
    public Image expressionImage;
    public AudioSource voiceAudioSource;

    private bool advanceRequested;
    private DialogueManager dialogueManager;
    public ButtonCreator buttonCreator;

    public MainsceneGuide mainsceneGuide;

    private void Awake()
    {
        dialogueManager = new DialogueManager();
    }

    private void Start()
    {
        if (CurrentPatient.data == null)
        {
            Debug.LogWarning("Current patient is empty, cannot play diagnosis dialogue.");
            return;
        }

        if (CurrentPatient.data.DiagnosisDialogue == null)
        {
            Debug.LogWarning($"Patient {CurrentPatient.data.PatientName} has no DiagnosisDialogue.");
            return;
        }

        if (voiceAudioSource == null)
        {
            voiceAudioSource = GetComponent<AudioSource>();
        }

        dialogueManager.PlayDialogue(
            this,
            CurrentPatient.data.DiagnosisDialogue.lines,
            NameText,
            dialogueText,
            expressionImage,
            voiceAudioSource,
            ConsumeAdvanceRequest);
        StartCoroutine(firstdiagnosis());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            advanceRequested = true;
        }
    }

    public IEnumerator firstdiagnosis()
    {
        while (dialogueManager.IsPlaying)
        {
            yield return null;
        }

        mainsceneGuide.FirstDiagnosis(() =>
        {
            if (buttonCreator != null)
            {
                buttonCreator.CreateButton(this);
            }
        });
    }

    public void ShowDiagnosisFail()
    {
        if (CurrentPatient.data == null)
        {
            return;
        }

        dialogueManager.StopDialogue(this, voiceAudioSource);

        if (dialogueText != null)
        {
            dialogueText.text = CurrentPatient.data.DiagnosisFailText;
        }

        if (NameText != null && CurrentPatient.data != null)
        {
            NameText.text = CurrentPatient.data.PatientName;
        }
    }

    private bool ConsumeAdvanceRequest()
    {
        if (!advanceRequested)
        {
            return false;
        }

        advanceRequested = false;
        return true;
    }

    private void OnDisable()
    {
        dialogueManager.StopDialogue(this, voiceAudioSource);
    }
}
