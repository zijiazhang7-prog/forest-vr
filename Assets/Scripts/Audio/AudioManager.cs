using UnityEngine;

/// <summary>
/// 音频管理器。BGM + SFX 占位，AudioClip 可后填。
/// 没有音频时保持空引用不报错。
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("BGM")]
    [SerializeField] private AudioClip forestAmbientBgm;

    [Header("SFX")]
    [SerializeField] private AudioClip orbCollectSfx;
    [SerializeField] private AudioClip pedestalActivateSfx;
    [SerializeField] private AudioClip mushroomCorrectSfx;
    [SerializeField] private AudioClip mushroomWrongSfx;
    [SerializeField] private AudioClip chargeSnatchSfx;
    [SerializeField] private AudioClip qteSuccessSfx;
    [SerializeField] private AudioClip qteFailSfx;
    [SerializeField] private AudioClip unicornRewardSfx;
    [SerializeField] private AudioClip endingSfx;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PlayBgm(forestAmbientBgm);
    }

    public void PlayBgm(AudioClip clip)
    {
        if (bgmSource == null || clip == null) return;
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    // 便利方法——联调时各模块可调用
    public void PlayOrbCollect() => PlaySfx(orbCollectSfx);
    public void PlayPedestalActivate() => PlaySfx(pedestalActivateSfx);
    public void PlayMushroomCorrect() => PlaySfx(mushroomCorrectSfx);
    public void PlayMushroomWrong() => PlaySfx(mushroomWrongSfx);
    public void PlayChargeSnatch() => PlaySfx(chargeSnatchSfx);
    public void PlayQteSuccess() => PlaySfx(qteSuccessSfx);
    public void PlayQteFail() => PlaySfx(qteFailSfx);
    public void PlayUnicornReward() => PlaySfx(unicornRewardSfx);
    public void PlayEnding() => PlaySfx(endingSfx);
}
