using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClinicThankYouFollowUpUI : MonoBehaviour
{
    [Header("Panel 1：有道谢时显示（访客 + 开始对话）")]
    [SerializeField] private CanvasGroup visitAnnouncementPanel;
    [SerializeField] private TextMeshProUGUI announcementVisitorNameText;
    [SerializeField] private Image announcementVisitorPortrait;
    [SerializeField] private Button startDialogueButton;

    [Header("Panel 2：对话演出")]
    [SerializeField] private CanvasGroup dialoguePanel;
    [SerializeField] private TextMeshProUGUI NameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image expressionImage;
    [SerializeField] private AudioSource voiceSource;

    private readonly DialogueManager dialogueManager = new DialogueManager();
    private bool advanceRequested;
    private bool startDialogueClicked;

    private void Awake()
    {
        if (visitAnnouncementPanel != null)
        {
            HidePanel(visitAnnouncementPanel);
        }

        if (dialoguePanel != null)
        {
            HidePanel(dialoguePanel);
        }
    }

    private void Update()
    {
        if (dialoguePanel != null && dialoguePanel.alpha > 0.01f && dialoguePanel.blocksRaycasts)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                advanceRequested = true;
            }
        }
    }

    public IEnumerator RunPendingVisits(MonoBehaviour host)
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || host == null)
        {
            yield break;
        }

        if (dialoguePanel == null && dialogueText == null)
        {
            Debug.LogWarning("ClinicThankYouFollowUpUI: 未绑定对话 Panel 或 dialogueText，无法播放道谢。");
            yield break;
        }

        gm.PruneExpiredThankYouVisits();

        while (true)
        {
            PendingThankYouVisit pending = gm.FindNextThankYouForToday();
            if (pending == null)
            {
                break;
            }

            PatientData visitor = gm.GetPatientDataById(pending.visitorPatientId);
            if (visitor == null || !GameManager.LinesHaveThankYouContent(pending.lines))//不包含内容时
            {
                gm.RemoveThankYouVisit(pending);
                continue;
            }

            if (visitAnnouncementPanel != null)
            {
                if (announcementVisitorNameText != null)
                {
                    announcementVisitorNameText.text = visitor.PatientName;
                }

                if (announcementVisitorPortrait != null && visitor.Sprite != null)
                {
                    announcementVisitorPortrait.sprite = visitor.touxiang2;
                    announcementVisitorPortrait.preserveAspect = true;
                }

                startDialogueClicked = false;
                if (startDialogueButton != null)
                {
                    startDialogueButton.onClick.RemoveAllListeners();
                    startDialogueButton.onClick.AddListener(() => startDialogueClicked = true);
                }
                else
                {
                    startDialogueClicked = true;
                }

                ShowPanel(visitAnnouncementPanel);
                while (!startDialogueClicked)
                {
                    yield return null;
                }

                HidePanel(visitAnnouncementPanel);
            }

            if (dialogueText != null)
            {
                dialogueText.text = string.Empty;
            }
            if (NameText != null)
            {
                NameText.text = string.Empty;
            }

            if (voiceSource == null)
            {
                voiceSource = GetComponent<AudioSource>();
            }

            if (dialoguePanel != null)
            {
                ShowPanel(dialoguePanel);
            }

            advanceRequested = false;
            dialogueManager.PlayDialogue(
                host,
                pending.lines,
                NameText,
                dialogueText,
                expressionImage,
                voiceSource,
                ConsumeAdvanceRequest);

            while (dialogueManager.IsPlaying)
            {
                yield return null;
            }

            GameManager.Instance.Bag.Add(visitor.gift);

            gm.RemoveThankYouVisit(pending);

            if (dialoguePanel != null)
            {
                HidePanel(dialoguePanel);
            }
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

    private static void HidePanel(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private static void ShowPanel(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void OnDisable()
    {
        dialogueManager.StopDialogue(this, voiceSource);
    }
}
