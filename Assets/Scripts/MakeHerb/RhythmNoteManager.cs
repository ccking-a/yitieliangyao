using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RhythmNoteManager : MonoBehaviour
{


    public List<float> AllRhythm { get; private set; }//所有点击的时间点
    public List<float> CircleRhythm { get; private set; }//生成圆环的时间点 allrhythm - duration
    private List<bool> Notehit;//每个圆环是否被点击
    public AudioSource audiosource;
    public float countdown = 3f;
    public bool isFirstTime = true;

    public float countdowntime { get; private set; }//开局倒计时
    public float RhythmTime { get; private set; }//节奏开始的时间
    public float Circleduration { get; private set; } //圆圈缩小的时间

    public Button ButtonRhythm;//点击按钮
    public TextMeshProUGUI scoretext;
    public TextMeshProUGUI beattext;
    public RhythmUI rhythmUI;
    public ParticleSystem RhythmParticleSystem;
    public ParticleSystem.EmissionModule RhythmParticleSystemEmision;
    public ParticleSystem.MainModule RhythmParticleSystemMain;

    public int beatindex { get; private set; }
    public int allbeat { get; private set; }
    public int createCircleindex { get; private set; }
    public int score { get; private set; }

    public bool isBegin { get; private set; }
    public bool isEnd;
    public GameObject waihuan;
    public GameObject neihuan;
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        AllRhythm = CurrentPatient.data.rhythmNote.timelist;
        CircleRhythm = new List<float>();
        Notehit = new List<bool>();
        countdowntime = Time.time + countdown;
        RhythmTime = 0f;
        createCircleindex = 0;
        score = 0;
        isBegin = false;
        isEnd = false;
        waihuan.SetActive(true);
        bool showNeihuan = GameManager.Instance == null || GameManager.Instance.FirstTimeInMakeHerb;
        neihuan.SetActive(showNeihuan);
        Circleduration = 0.6f;
        beatindex = 0;
        allbeat = AllRhythm.Count;
        scoretext.text = "";
        beattext.text = " ";
        isFirstTime = GameManager.Instance.FirstTimeInMakeHerb;

        audiosource = GetComponent<AudioSource>();
        audiosource.clip = CurrentPatient.data.rhythmclip;
        audiosource.volume = AudioManager.Instance.bgmVolume;
        AudioManager.Instance.PauseBgm();

        RhythmParticleSystemEmision = RhythmParticleSystem.emission;
        RhythmParticleSystemMain = RhythmParticleSystem.main;

        for (int i = 0; i < AllRhythm.Count; i++)
        {
            Notehit.Add(false);
        }

        ButtonRhythm.onClick.RemoveAllListeners();
        ButtonRhythm.onClick.AddListener(() =>
        {
            if (isBegin)
            {
                Beat();
            }
        });



        if (waihuan == null)
        {
            Debug.LogError("外环引用为空");
        }
        if (neihuan == null)
        {
            Debug.LogError("内环引用为空");
        }

        CircleRhythm.Clear();
        for (int i = 0; i < AllRhythm.Count; i++)
        {
            float rhythm = AllRhythm[i];
            CircleRhythm.Add(rhythm - Circleduration);
        }
    }

    void Update()
    {

        if (!isFirstTime)
        {
            isFirstTime = GameManager.Instance.FirstTimeInMakeHerb;
            countdowntime = Time.time + countdown;
            return;
        }

        if (!isBegin)
        {
            if (Time.time >= countdowntime)
            {
                RhythmGameBegin();
            }
        }
        else
        {
            RhythmTime += Time.deltaTime;
        }



        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isBegin)
            {
                Beat();
            }
        }
    }

    public void ReStartRhythm()
    {
        Initialize();
    }

    private void Beat()
    {
        if (beatindex < allbeat)
        {
            if (Math.Abs(RhythmTime - AllRhythm[beatindex]) < 0.2f)
            {
                score += 50;
                Notehit[beatindex] = true;
                beatindex++;
                scoretext.text = "总分：" + score.ToString();
                beattext.text = "perfect";
                PlayPrefectRhythmParticle();
                Debug.Log("perfect");
            }
            else if (Math.Abs(RhythmTime - AllRhythm[beatindex]) < 0.4f)
            {
                score += 30;
                Notehit[beatindex] = true;
                beatindex++;
                scoretext.text = "总分：" + score.ToString();
                beattext.text = "good";
                PlayGoodRhythmParticle();
                Debug.Log("good");
            }
            else if (Math.Abs(RhythmTime - AllRhythm[beatindex]) < 0.6f)
            {
                score += 10;
                Notehit[beatindex] = true;
                beatindex++;
                scoretext.text = "总分：" + score.ToString();
                beattext.text = "bad";
                PlayGoodRhythmParticle();
                Debug.Log("bad");
            }
        }
    }

    private void RhythmGameBegin()
    {
        isBegin = true;
        if (audiosource.clip != null)
        {
            audiosource.Play();
        }
        StartCoroutine(StartCreateCircle());
        StartCoroutine(WaitForEndOfRhythm());
    }

    private IEnumerator WaitForEndOfRhythm()
    {
        while (!isEnd)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        rhythmUI.StartSuccessTreated(score);

    }

    private IEnumerator StartCreateCircle()
    {
        int j = AllRhythm.Count;
        while (createCircleindex < j)
        {
            if (RhythmTime >= CircleRhythm[createCircleindex])
            {
                int noteIndex = createCircleindex;
                createCircleindex++;
                StartCoroutine(CreateCircle(noteIndex));
            }
            else
            {
                yield return null;
            }
        }
        yield return new WaitForSeconds(3f);
        isEnd = true;
    }
    private IEnumerator CreateCircle(int noteIndex)
    {
        Vector3 startScale = Vector3.one * 2;
        Vector3 TargetScale = Vector3.one;
        GameObject newwaihuan = Instantiate(waihuan, neihuan.transform);
        newwaihuan.transform.localPosition = Vector3.zero;

        StartCoroutine(waitformiss(noteIndex, newwaihuan));

        for (float t = 0; t < Circleduration; t += Time.deltaTime)
        {
            newwaihuan.transform.localScale = Vector3.Lerp(startScale, TargetScale, t / Circleduration);
            yield return null;
        }
        newwaihuan.transform.localScale = TargetScale;


    }
    private IEnumerator waitformiss(int noteIndex, GameObject waihuan)
    {
        float createTime = Time.time;
        while (!Notehit[noteIndex])
        {
            yield return null;
            if (Time.time - createTime > 0.3f + Circleduration) break;

        }
        if (!Notehit[noteIndex])
        {
            beatindex++;
            scoretext.text = "总分：" + score.ToString();
            beattext.text = "miss";
            PlayMissRhythmParticle();
            Debug.Log("miss");
        }
        waihuan.SetActive(false);
    }

    public void PlayPrefectRhythmParticle()
    {
        var color = new ParticleSystem.MinMaxGradient(Color.yellow, new Color(1f, 0.7f, 0f));
        RhythmParticleSystemMain.startColor = color;

        RhythmParticleSystemEmision.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0, 40) });

        RhythmParticleSystemMain.startSize = new ParticleSystem.MinMaxCurve(0.4f, 0.6f);

        RhythmParticleSystem.Play();

    }
    public void PlayGoodRhythmParticle()
    {
        var color = new ParticleSystem.MinMaxGradient(new Color(1f, 1f, 0.6f));
        RhythmParticleSystemMain.startColor = color;

        RhythmParticleSystemEmision.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0, 30) });

        RhythmParticleSystemMain.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.5f);

        RhythmParticleSystem.Play();

    }
    public void PlayMissRhythmParticle()
    {
        var color = new ParticleSystem.MinMaxGradient(new Color(0.5f, 0.2f, 0.2f));
        RhythmParticleSystemMain.startColor = color;

        RhythmParticleSystemEmision.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0, 20) });

        RhythmParticleSystemMain.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.5f);

        RhythmParticleSystem.Play();

    }
}
