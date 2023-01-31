using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LuckyKat;

public class WinBonusController : MonoBehaviour {
    static private List<int> weaponUnlock;
    public int coinReward = 0;
    [SerializeField] private Image overlay;
    [SerializeField] private AnimationCurve ElasticCurve;
    [SerializeField] private GameObject coinsStage;
    [SerializeField] private GameObject progressStage;
    [SerializeField] private GameObject unlockStage;

    // Coins stage
    [SerializeField] private RectTransform WinTextTransform;
    [SerializeField] private RectTransform BannerTransform;
    [SerializeField] private RectTransform coinsTransform;
    [SerializeField] private Text coinsText;
    [SerializeField] private CanvasGroup rewardGroup;
    [SerializeField] private RectTransform rewardTransform;
    [SerializeField] private RectTransform rewardPointerTransform;
    //[SerializeField] private CoinFountain rewardFountain;
    [SerializeField] private Button rewardButton;
    [SerializeField] private Text rewardCoinsText;
    private float rewardCount = 0f;
    private int rewardCountSign = 1;
    private int multiplierCache = 2;
    private bool rewardCountEnabled = false;
    [SerializeField] private Text bonusNoThanksText;
    [SerializeField] private Button bonusNoThanksButton;
    // Progression stage
    [SerializeField] private Image progressGunFill;
    [SerializeField] private Image progressGunBg;
    [SerializeField] private RectTransform gainProgressTransform;
    [SerializeField] private Text progressText;
    [SerializeField] private Text extraProgressText;
    [SerializeField] private Button extraProgressButton;
    [SerializeField] private CanvasGroup extraProgressButtonGroup;
    [SerializeField] private Text progressNoThanksText;
    [SerializeField] private Button progressNoThanksButton;
    // Unlock stage
    [SerializeField] private RectTransform shineTransform;
    [SerializeField] private Image shineImage;
    [SerializeField] private Button claimWeaponButton;
    [SerializeField] private CanvasGroup claimWeaponButtonGroup;
    [SerializeField] private Text claimNoThanksText;
    [SerializeField] private Button claimNoThanksButton;

    //Extra button for the assignment
    [SerializeField] private GameObject installButton;

    private bool shineActive = false;
    private float unlockStrength = 0;
    private float unlockProgress = 0;
    private int currentUnlock = 0;

    private void OnEnable() {

        //I decided to keep the overlay to make it look a bit more polished -VMG
        // show background
        Color backgroundCol = overlay.color;
        backgroundCol.a = 0f;
        new Tween().SetEase(Tween.Ease.OutQuad).SetTime(0.3f).SetStart(0f).SetEnd(0.4f).SetOnUpdate((float v, float t) => {
            backgroundCol.a = v;
            overlay.color = backgroundCol;
        });

        //Disabling the "YOU WIN" banner -VMG
        // show banner
        /*new Tween().SetCustomEase(ElasticCurve).SetEase(Tween.Ease.Custom).SetDelay(0.1f).SetTime(0.4f).SetStart(0).SetEnd(1).SetOnUpdate((float v, float t) => {
            BannerTransform.rotation = Quaternion.Euler(0, 0, v * -10 + 10);
            BannerTransform.localScale = new Vector3(1, v, 1);
        }).SetOnComplete(() => {
            BannerTransform.rotation = Quaternion.Euler(0, 0, 0);
            BannerTransform.localScale = new Vector3(1, 1, 1);
        });*/
        // show win
        /*new Tween().SetEase(Tween.Ease.OutQuad).SetDelay(0.25f).SetTime(0.05f).SetStart(0).SetEnd(1).SetOnUpdate((float v, float t) => {
            WinTextTransform.localScale = new Vector3(1, v, 1);
        }).SetOnComplete(() => {
            WinTextTransform.localScale = new Vector3(1, 1, 1);
        });

        // #1 weapons to unlock. show progress, after ad (if full unlock weapon, branch to option 2) or no thanks go to bonus coins
        // #2 progress full, ask to unlock. after unlock or no thanks go to bonus coins
        // #3 no weapons to unlock. old pattern
        if (weaponUnlock.Count > 0) {
            new Tween().SetTime(0.3f).SetOnComplete(() => {
                //Disabling Weapon unlock progress -VMG

                //DoProgressStage();
            });
        } else {
            new Tween().SetTime(0.3f).SetOnComplete(() => {
                DoCoinStage();
            });
        }*/

        installButton.SetActive(true);
    }

    void DoProgressStage() {
        //Setting progressStage to false to disable weapon progression-VMG
        progressStage.SetActive(false);
        // show progress reward
        new Tween().SetCustomEase(ElasticCurve).SetEase(Tween.Ease.Custom).SetTime(0.4f).SetStart(0).SetEnd(1).SetOnUpdate((float v, float t) => {
            gainProgressTransform.localScale = new Vector3(v, v, 1);
        }).SetOnComplete(() => {
            gainProgressTransform.localScale = new Vector3(1, 1, 1);
        });
        new Tween().SetEase(Tween.Ease.OutQuad).SetDelay(0.1f).SetTime(0.4f).SetStart(0).SetEnd(1).SetOnUpdate((float v, float t) => {
            progressText.text = Mathf.Floor(unlockProgress + unlockStrength * v * 100).ToString() + "%";
            progressGunFill.fillAmount = unlockProgress + unlockStrength * v;
        }).SetOnComplete(() => {
            unlockProgress += unlockStrength;
            if (unlockProgress > 0.98f) {
                unlockProgress = 1;
                ShowUnlockOptions();
            } else {
                ShowExtraProgress();
            }
            GameData.weaponProgression = unlockProgress % 1;
            GameData.SaveGameData();
            progressGunFill.fillAmount = unlockProgress;
            progressText.text = Mathf.Floor(unlockProgress * 100).ToString() + "%";
        });
    }

    void ShowExtraProgress() {
        extraProgressButton.enabled = true;
        new Tween().SetEase(Tween.Ease.OutQuad).SetTime(0.2f).SetStart(0f).SetEnd(1f).SetOnUpdate((float v, float t) => {
            extraProgressButtonGroup.alpha = v;
        });

        Color progressNoThanksColor = progressNoThanksText.color;
        new Tween().SetEase(Tween.Ease.OutQuad).SetDelay(0.5f).SetTime(0.2f).SetStart(0f).SetEnd(1f).SetOnStart(() => {
            progressNoThanksButton.gameObject.SetActive(true);
        }).SetOnUpdate((float v, float t) => {
            progressNoThanksColor.a = v;
            progressNoThanksText.color = progressNoThanksColor;
        });
    }

    void HideExtraProgress() {
        Color progressNoThanksColor = progressNoThanksText.color;
        new Tween().SetEase(Tween.Ease.InQuad).SetTime(0.2f).SetStart(1f).SetEnd(0f).SetOnUpdate((float v, float t) => {
            extraProgressButtonGroup.alpha = v;
            progressNoThanksColor.a = v;
            progressNoThanksText.color = progressNoThanksColor;
        }).SetOnComplete(() => {
            extraProgressButton.gameObject.SetActive(false);
            DoUnlock();
        });
    }

    void CloseProgress() {
        Color progressNoThanksColor = progressNoThanksText.color;
        new Tween().SetEase(Tween.Ease.InQuad).SetTime(0.2f).SetStart(1f).SetEnd(0f).SetOnUpdate((float v, float t) => {
            gainProgressTransform.localScale = new Vector3(v, v, 1);
            extraProgressButtonGroup.alpha = v;
            progressNoThanksColor.a = v;
            progressNoThanksText.color = progressNoThanksColor;
        }).SetOnComplete(() => {
            extraProgressButton.gameObject.SetActive(false);
            progressNoThanksButton.gameObject.SetActive(false);
            progressStage.SetActive(false);
            DoCoinStage();
        });
    }

    void ShowUnlockOptions() {
        unlockStage.SetActive(true);
        extraProgressButton.gameObject.SetActive(false);

        Color shineColor = shineImage.color;
        shineActive = true;
        new Tween().SetEase(Tween.Ease.OutQuad).SetTime(0.2f).SetStart(0f).SetEnd(0.4f).SetOnStart(() => {
        }).SetOnUpdate((float v, float t) => {
            shineColor.a = v;
            shineImage.color = shineColor;
        });

        claimWeaponButton.enabled = true;
        claimWeaponButtonGroup.enabled = true;
        new Tween().SetEase(Tween.Ease.OutQuad).SetDelay(0.1f).SetTime(0.2f).SetStart(0f).SetEnd(1f).SetOnUpdate((float v, float t) => {
            claimWeaponButtonGroup.alpha = v;
        });

        Color claimNoThanksColor = claimNoThanksText.color;
        new Tween().SetEase(Tween.Ease.OutQuad).SetDelay(0.5f).SetTime(0.2f).SetStart(0f).SetEnd(1f).SetOnStart(() => {
            claimNoThanksButton.gameObject.SetActive(true);
        }).SetOnUpdate((float v, float t) => {
            claimNoThanksColor.a = v;
            claimNoThanksText.color = claimNoThanksColor;
        });
    }

    void DoUnlock() {
        unlockStage.SetActive(true);
        claimWeaponButton.enabled = false;
        claimNoThanksButton.enabled = false;
        Color shineColor = shineImage.color;
        shineActive = true;
        new Tween().SetEase(Tween.Ease.OutQuad).SetTime(0.2f).SetStart(shineColor.a).SetEnd(1f).SetOnStart(() => {
        }).SetOnUpdate((float v, float t) => {
            shineColor.a = v;
            shineImage.color = shineColor;
        });

        new Tween().SetCustomEase(ElasticCurve).SetEase(Tween.Ease.Custom).SetTime(0.4f).SetStart(0.4f).SetEnd(0.2f).SetOnUpdate((float v, float t) => {
            gainProgressTransform.localScale = new Vector3(1 + v, 1 + v, 1);
        });

        progressText.text = "Unlocked!";
        weaponUnlock.Remove(currentUnlock);
        GameData.weaponsUnlocked.Add(currentUnlock);
        WeaponManager.instance.SetWeapon(WeaponManager.instance.data.getId(currentUnlock), true);
        SoundPlayer.instance.play("coin", 1.2f, 0.6f);
        SoundPlayer.instance.play("coin", 1f, 0.6f);
        SoundPlayer.instance.play("coin", 0.8f, 0.6f);

        new Tween().SetTime(1f).SetOnComplete(() => {
            CloseWeaponUnlock();
        });
    }

    void RemoveUnlockButtons() {
        Color claimNoThanksColor = claimNoThanksText.color;
        new Tween().SetEase(Tween.Ease.InQuad).SetTime(0.2f).SetStart(1f).SetEnd(0f).SetOnUpdate((float v, float t) => {
            claimWeaponButtonGroup.alpha = v;
            claimNoThanksColor.a = v;
            claimNoThanksText.color = claimNoThanksColor;
        });
    }

    void CloseUnlock() {
        Color claimNoThanksColor = claimNoThanksText.color;
        Color shineColor = shineImage.color;
        float shineAlpha = shineColor.a;
        shineActive = false;
        new Tween().SetEase(Tween.Ease.InQuad).SetTime(0.2f).SetStart(1f).SetEnd(0f).SetOnUpdate((float v, float t) => {
            gainProgressTransform.localScale = new Vector3(v, v, 1);
            claimWeaponButtonGroup.alpha = v;
            claimNoThanksColor.a = v;
            claimNoThanksText.color = claimNoThanksColor;
            shineColor.a = shineAlpha * v;
            shineImage.color = shineColor;
        }).SetOnComplete(() => {
            progressStage.SetActive(false);
            unlockStage.SetActive(false);
            DoCoinStage();
        });
    }
    void CloseWeaponUnlock() {
        Color shineColor = shineImage.color;
        float shineAlpha = shineColor.a;
        shineActive = false;
        new Tween().SetEase(Tween.Ease.InQuad).SetTime(0.2f).SetStart(1f).SetEnd(0f).SetOnUpdate((float v, float t) => {
            gainProgressTransform.localScale = new Vector3(v, v, 1);
            shineColor.a = shineAlpha * v;
            shineImage.color = shineColor;
        }).SetOnComplete(() => {
            progressStage.SetActive(false);
            unlockStage.SetActive(false);
            DoCoinStage();
        });
    }

    void DoCoinStage() {
        coinsStage.SetActive(true);
        // show coin reward
        new Tween().SetCustomEase(ElasticCurve).SetEase(Tween.Ease.Custom).SetTime(0.4f).SetStart(0).SetEnd(1).SetOnUpdate((float v, float t) => {
            coinsTransform.localScale = new Vector3(v, v, 1);
        }).SetOnComplete(() => {
            coinsTransform.localScale = new Vector3(1, 1, 1);
        });
        int oldCoins = GameData.coins;
        new Tween().SetEase(Tween.Ease.OutQuad).SetDelay(0.1f).SetTime(0.4f).SetStart(0).SetEnd(coinReward).SetOnUpdate((float v, float t) => {
            int c = (int)Mathf.Floor(v);
            coinsText.text = c.ToString();
            CoinCounter.UpdateCoinsDirty(oldCoins + c);
        }).SetOnComplete(() => {
            coinsText.text = coinReward.ToString();
            ShowRewardAndContinue();
        });
    }

    void ShowRewardAndContinue() {
        rewardButton.enabled = true;
        rewardCountEnabled = true;
        new Tween().SetEase(Tween.Ease.OutQuad).SetTime(0.2f).SetStart(0f).SetEnd(1f).SetOnUpdate((float v, float t) => {
            rewardGroup.alpha = v;
        });


        Color bonusNoThanksColor = bonusNoThanksText.color;
        new Tween().SetEase(Tween.Ease.OutQuad).SetDelay(0.5f).SetTime(0.2f).SetStart(0f).SetEnd(1f).SetOnStart(() => {
            bonusNoThanksButton.gameObject.SetActive(true);
        }).SetOnUpdate((float v, float t) => {
            bonusNoThanksColor.a = v;
            bonusNoThanksText.color = bonusNoThanksColor;
        });
    }

    public void GiveCoinReward(int multiplier = 1) {
        //GameData.coins += coinReward * multiplier;
        //This will keep the coins stuck at 200, so players can't accumulate more coins -VMG
        GameData.coins = coinReward * multiplier;
        GameData.SaveGameData();
    }

    int GetMultiplier(float value) {
        if (value <= 0.3666666667f) {
            return 1;
        } else if (value <= 0.66666666667f) {
            return 2;
        } else if (value <= 0.85555555556f) {
            return 3;
        } else return 4;
    }

    // Update is called once per frame
    void Update() {
        if (rewardCountEnabled) {
            rewardCount += Time.deltaTime * 0.8f * rewardCountSign;
            if (0f >= rewardCount || rewardCount >= 1f) {
                rewardCount = Mathf.Clamp(rewardCount, 0f, 1f);
                rewardCountSign = -rewardCountSign;
            }
            rewardPointerTransform.rotation = Quaternion.Euler(0, 0, 90f + rewardCount * -180f);
            int multiplier = GetMultiplier(rewardCount);
            if (multiplierCache != multiplier) {
                rewardCoinsText.text = "+" + (coinReward * multiplier).ToString();
                multiplierCache = multiplier;
            }
        }

        if (shineActive) {
            shineTransform.rotation = shineTransform.rotation * Quaternion.Euler(0, 0, 0.2f * Time.deltaTime * 60f);
        }
    }

    public void ResetIU() {
        // populate unlock list
        if (weaponUnlock == null) {
            weaponUnlock = new List<int>();
            int l = WeaponManager.instance.data.id.Length;
            for (var i = 1; i < l; i++) {
                if (!GameData.weaponsUnlocked.Contains(i)) {
                    weaponUnlock.Add(i);
                }
            }
        }

        Color backgroundCol = overlay.color;
        backgroundCol.a = 0f;
        overlay.color = backgroundCol;

        /*BannerTransform.rotation = Quaternion.Euler(0, 0, -10);
        BannerTransform.localScale = new Vector3(1, 0, 1);
        WinTextTransform.localScale = new Vector3(1, 0, 1);

        coinsTransform.localScale = new Vector3(0, 0, 1);
        coinsText.text = "0";

        rewardGroup.alpha = 0;
        rewardButton.enabled = false;
        rewardPointerTransform.rotation = Quaternion.Euler(0, 0, 90f);
        rewardCoinsText.text = "+" + coinReward.ToString();
        rewardCount = 0f;
        rewardCountSign = 1;
        rewardCountEnabled = false;

        bonusNoThanksButton.gameObject.SetActive(false);
        Color bonusNoThanksColor = bonusNoThanksText.color;
        bonusNoThanksColor.a = 0;
        bonusNoThanksText.color = bonusNoThanksColor;
        bonusNoThanksButton.enabled = true;

        if (weaponUnlock.Count > 0) {
            // progress stage
            unlockProgress = GameData.weaponProgression;
            if (GameData.unlockWeapon >= weaponUnlock.Count) {
                GameData.unlockWeapon = GameData.unlockWeapon % weaponUnlock.Count;
            }
            // if (GameData.unlockWeapon < 0) {
            //     GameData.unlockWeapon = 0;
            // }
            currentUnlock = weaponUnlock[GameData.unlockWeapon];
            progressGunFill.sprite = WeaponManager.instance.data.uiImage[currentUnlock];
            progressGunBg.sprite = WeaponManager.instance.data.uiBackground[currentUnlock];
            progressGunFill.fillAmount = unlockProgress;
            gainProgressTransform.localScale = new Vector3(0, 0, 1);
            progressText.text = Mathf.Floor(unlockProgress * 100).ToString() + "%";
            unlockStrength = GetUnlockStrength(WeaponManager.instance.data.id.Length - weaponUnlock.Count);
            extraProgressText.text = "+" + Mathf.Floor(unlockStrength * 100).ToString() + "%";
            extraProgressButton.gameObject.SetActive(true);
            extraProgressButton.enabled = false;
            extraProgressButtonGroup.alpha = 0;
            progressNoThanksButton.gameObject.SetActive(false);
            Color progressNoThanksColor = progressNoThanksText.color;
            progressNoThanksColor.a = 0;
            progressNoThanksText.color = progressNoThanksColor;
            progressNoThanksButton.enabled = true;
            // Unlock stage

            // shineTransform;
            Color shineColor = shineImage.color;
            shineColor.a = 0;
            shineImage.color = shineColor;

            claimWeaponButton.enabled = false;
            claimWeaponButtonGroup.alpha = 0;
            claimNoThanksButton.gameObject.SetActive(false);
            Color claimNoThanksColor = claimNoThanksText.color;
            claimNoThanksColor.a = 0;
            claimNoThanksText.color = claimNoThanksColor;
            claimNoThanksButton.enabled = true;
        }

        // hide stages
        coinsStage.SetActive(false);
        progressStage.SetActive(false);
        unlockStage.SetActive(false);*/
    }

    float GetUnlockStrength(int value) {
        switch (value) {
            case 1:
                return 0.5f;
            case 2:
                return 0.33333f;
            default:
                return 0.25f;
        }
    }

    public void ContinueGame() {
        // ContinueButton.enabled = false;
        SoundPlayer.instance.play("mousedown");
        rewardButton.enabled = false;
        bonusNoThanksButton.enabled = false;
        rewardCountEnabled = false;
        /*if (!GameController.instance.noAds && GameData.level >= GameController.instance.interstitialStart && !GameController.instance.interstitialBeforeWin) { // since game level is zero based and interstitial start is one based we dont have to equalize them because game level has already incremented at this point
            AdManager.instance.ShowInterstitial(() => {
                GameController.instance.LoadLevel(GameData.level);
                gameObject.SetActive(false);
            });
        } else {
            GameController.instance.LoadLevel(GameData.level);
            gameObject.SetActive(false);
        }*/
    }
    //Disabled this function because the scripts referenced by it aren't needed anymore -VMG
    public void MultiplyReward() {
        /*SoundPlayer.instance.play("mousedown");
#if UNITY_EDITOR
        GiveCoinReward(multiplierCache);
        if (rewardFountain != null) {
            rewardFountain.gameObject.SetActive(true);
        }
        new Tween().SetTime(0.6f).SetOnComplete(() => {
            CoinCounter.UpdateCoins();
        });
        new Tween().SetTime(1.4f).SetOnComplete(() => {
            rewardFountain.gameObject.SetActive(false);
            GameController.instance.LoadLevel(GameData.level);
        });
        return;
#endif
        /*if (AdManager.instance != null) {
            rewardCountEnabled = false;
            rewardButton.enabled = false;
            bonusNoThanksButton.enabled = false;
            AdManager.instance.ShowRewarded("RewardMultiplier", () => {
                GiveCoinReward(multiplierCache);
                //Firebase.Analytics.Parameter[] currencyParameters = {
                    //new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterVirtualCurrencyName, "coins"),
                    //new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterValue, 200 * multiplierCache)
                //};
                //Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventEarnVirtualCurrency, currencyParameters);

                if (rewardFountain != null) {
                    rewardFountain.gameObject.SetActive(true);
                }
                new Tween().SetTime(0.6f).SetOnComplete(() => {
                    CoinCounter.UpdateCoins();
                });
                new Tween().SetTime(1.4f).SetOnComplete(() => {
                    GameController.instance.LoadLevel(GameData.level);
                });
            }, () => {
                rewardCountEnabled = true;
                rewardButton.enabled = true;
                bonusNoThanksButton.enabled = true;
            });
        }*/
    }

    public void GiveExtraProgress() {
        SoundPlayer.instance.play("mousedown");

#if UNITY_EDITOR
        extraProgressButton.enabled = false;
        progressNoThanksButton.enabled = false;
        new Tween().SetEase(Tween.Ease.OutQuad).SetTime(0.4f).SetStart(0).SetEnd(1).SetOnUpdate((float v, float t) => {
            progressText.text = Mathf.Floor(unlockProgress + unlockStrength * v * 100).ToString() + "%";
            progressGunFill.fillAmount = unlockProgress + unlockStrength * v;
        }).SetOnComplete(() => {
            unlockProgress += unlockStrength;
            if (unlockProgress > 0.98f) {
                unlockProgress = 1;
                HideExtraProgress();
            } else {
                CloseProgress();
            }
            GameData.weaponProgression = unlockProgress % 1;
            GameData.SaveGameData();
            progressGunFill.fillAmount = unlockProgress;
            progressText.text = Mathf.Floor(unlockProgress * 100).ToString() + "%";
        });
        return;
#endif
       /* if (AdManager.instance != null) {
            extraProgressButton.enabled = false;
            progressNoThanksButton.enabled = false;
            AdManager.instance.ShowRewarded("ExtraProgress", () => {
                new Tween().SetEase(Tween.Ease.OutQuad).SetTime(0.4f).SetStart(0).SetEnd(1).SetOnUpdate((float v, float t) => {
                    progressText.text = Mathf.Floor(unlockProgress + unlockStrength * v * 100).ToString() + "%";
                    progressGunFill.fillAmount = unlockProgress + unlockStrength * v;
                }).SetOnComplete(() => {
                    unlockProgress += unlockStrength;
                    if (unlockProgress > 0.98f) {
                        HideExtraProgress();
                        unlockProgress = 1;
                    } else {
                        CloseProgress();
                    }
                    GameData.weaponProgression = unlockProgress % 1;
                    GameData.SaveGameData();
                    progressGunFill.fillAmount = unlockProgress;
                    progressText.text = Mathf.Floor(unlockProgress * 100).ToString() + "%";
                });
            }, () => {
                extraProgressButton.enabled = true;
                progressNoThanksButton.enabled = true;
            });
        }*/
    }
    public void NoExtraProgress() {
        extraProgressButton.enabled = false;
        progressNoThanksButton.enabled = false;
        SoundPlayer.instance.play("mousedown");
        CloseProgress();
    }

    public void GiveWeapon() {
        SoundPlayer.instance.play("mousedown");
#if UNITY_EDITOR
        claimWeaponButton.enabled = false;
        claimNoThanksButton.enabled = false;
        RemoveUnlockButtons();
        DoUnlock();
        return;
#endif
        /*if (AdManager.instance != null) {
            claimWeaponButton.enabled = false;
            claimNoThanksButton.enabled = false;
            AdManager.instance.ShowRewarded("ProgressUnlock", () => {
                RemoveUnlockButtons();
                DoUnlock();
            }, () => {
                extraProgressButton.enabled = true;
                progressNoThanksButton.enabled = true;
            });
        }*/
    }
    public void NoUnlock() {
        claimWeaponButton.enabled = false;
        claimNoThanksButton.enabled = false;
        SoundPlayer.instance.play("mousedown");
        GameData.unlockWeapon = (GameData.unlockWeapon + 1) % weaponUnlock.Count;
        GameData.SaveGameData();
        CloseUnlock();
    }
}
