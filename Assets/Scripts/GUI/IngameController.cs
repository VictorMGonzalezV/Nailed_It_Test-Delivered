using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LuckyKat;

public class IngameController : MonoBehaviour {
    [SerializeField] private Text CurrentLevel;
    [SerializeField] private Text NextLevel;
    [SerializeField] private Image ProgressBar;
    [SerializeField] private Text ComboAmount;
    [SerializeField] private RectTransform ComboAmountTransform;
    [SerializeField] private RectTransform ComboTransform;
    [SerializeField] private CanvasGroup ComboCanvasGroup;
    [SerializeField] private RectTransform HeadshotTransform;
    [SerializeField] private CanvasRenderer HeadshotCanvas;

    [SerializeField] private AnimationCurve elasticCurve;
    [SerializeField] private RectTransform coinCounter;

    private int currentCombo = 0;
    private float comboTime = 0;

    private Tween headshotTween;
    private Tween comboTween;
    private bool canCancelHeadshot = false;

    // Start is called before the first frame update
    void Start() {
        CurrentLevel.text = (GameData.level + 1).ToString();
        NextLevel.text = (GameData.level + 2).ToString();
        ProgressBar.fillAmount = 0;
    }

    public void ResetUI() {
        CurrentLevel.text = (GameData.level + 1).ToString();
        NextLevel.text = (GameData.level + 2).ToString();
        ProgressBar.fillAmount = 0;
        HeadshotTransform.gameObject.SetActive(false);
        ComboTransform.gameObject.SetActive(false);
        currentCombo = 0;
        comboTime = 0;
    }

    // Update is called once per frame
    void Update() {
        // count progress on path through player controller
        //Disabling the progress bar visualization -VMG
        //ProgressBar.fillAmount = Mathf.Clamp(PlayerController.instance.PathProgress / PlayerController.instance.PathSize, 0f, 1f);
        // Debug.Log("HIER" + PlayerController.instance.PathProgress + "/" + PlayerController.instance.PathSize);
        comboTime -= Time.deltaTime;
        if (currentCombo > 1) {
            if (comboTime <= 0f) {
                if (comboTween == null) {
                    comboTween = new Tween().SetEase(Tween.Ease.InQuad).SetDelay(0.3f).SetTime(0.5f).SetStart(1).SetEnd(0).SetOnUpdate((float v, float t) => {
                        ComboCanvasGroup.alpha = v;
                    }).SetOnComplete(() => {
                        currentCombo = 0;
                        comboTween = null;
                    });
                }
            }
        } else if (currentCombo == 1) {
            if (comboTime <= 0f) {
                currentCombo = 0;
            }
        }
    }

    public void AddCombo() {
        currentCombo++;
        comboTime = 0.5f;

        if (currentCombo > 1) {
            ComboAmount.text = currentCombo.ToString() + "X";
            ComboTransform.gameObject.SetActive(true);
            ComboCanvasGroup.alpha = 1f;
            ComboAmountTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            if (comboTween != null) {
                comboTween.Stop();
            }
            comboTween = new Tween().SetCustomEase(elasticCurve).SetEase(Tween.Ease.Custom).SetTime(0.56f).SetStart(0).SetEnd(1).SetOnUpdate((float v, float t) => {
                ComboAmountTransform.localScale = new Vector3(1.2f - 0.2f * v, 1.2f - 0.2f * v, 1.2f - 0.2f * v);
            }).SetOnComplete(() => {
                comboTween = null;
            });
        }
    }

    public void DoHeadShot() {
        if (HeadshotTransform.gameObject.activeSelf) {
            if (!canCancelHeadshot) {
                return;
            }
            headshotTween.Stop();
        }
        HeadshotTransform.rotation = Quaternion.Euler(0, 0, Random.Range(-3f, 3f));
        makeHeadShotTween();
    }

    void makeHeadShotTween() {
        canCancelHeadshot = false;
        HeadshotTransform.gameObject.SetActive(true);
        HeadshotCanvas.SetAlpha(1);
        HeadshotTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        headshotTween = new Tween().SetCustomEase(elasticCurve).SetEase(Tween.Ease.Custom).SetTime(0.28f).SetStart(0).SetEnd(1).SetOnUpdate((float v, float t) => {
            HeadshotTransform.localScale = new Vector3(1.2f - 0.2f * v, 1.2f - 0.2f * v, 1.2f - 0.2f * v);
        }).SetOnComplete(() => {
            canCancelHeadshot = true;
            HeadshotTransform.localScale = new Vector3(1f, 1f, 1f);
            headshotTween = new Tween().SetEase(Tween.Ease.InQuad).SetDelay(0.3f).SetTime(0.5f).SetStart(1).SetEnd(0).SetOnUpdate((float v, float t) => {
                HeadshotCanvas.SetAlpha(v);
            }).SetOnComplete(() => {
                HeadshotTransform.gameObject.SetActive(false);
            });
        });
    }
}
