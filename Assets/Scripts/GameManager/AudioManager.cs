using System.Collections.Generic;
using UnityEngine;

public enum BGM
{
    TITLE,
    STAGE_SELECT,
    STAGE1,
    STAGE2,
    STAGE3
}
[System.Serializable]
public class BGMPair
{
    public BGM bgm;
    public AudioClip clip;

    public BGMPair(BGM bgm, AudioClip clip)
    {
        this.bgm = bgm;
        this.clip = clip;
    }
}

public enum SFX
{
    HOVER_BUTTON,
    CLICK_BUTTON,
    STAGE_START,
    WIRE_RELEASE,
    HIT,
    STONE,
    TIME_INCREASE,
    GRADE_EXPLODE,
    RESULT,
    JUMP
}

[System.Serializable]
public class SFXPair
{
    public SFX sfx;
    public AudioClip clip;

    public SFXPair(SFX sfx,AudioClip clip)
    {
        this.sfx = sfx;
        this.clip = clip;
    }
}

public class AudioManager : MonoBehaviour
{
    public AudioSource bgmSource;
    public float bgmCoef = 0.2f;
    public List<BGMPair> bgmClipList;
    public Dictionary<BGM, AudioClip> _bgmClips = new Dictionary<BGM, AudioClip>();

    public AudioSource sfxSource;
    public List<SFXPair> sfxClipList;
    private Dictionary<SFX, AudioClip> _sfxClips = new Dictionary<SFX, AudioClip>();

    void Start()
    {
        foreach (SFXPair p in sfxClipList)
        {
            _sfxClips.Add(p.sfx, p.clip);
        }
        foreach (BGMPair p in bgmClipList)
        {
            _bgmClips.Add(p.bgm, p.clip);
        }
    }

    public float GetBGMVolume()
    {
        return bgmSource.volume / bgmCoef;
    }

    public float GetSFXVolume()
    {
        return sfxSource.volume;
    }

    public void SetBGM(BGM bgm)
    {
        if (bgmSource.clip != _bgmClips.GetValueOrDefault(bgm))
            bgmSource.clip = _bgmClips.GetValueOrDefault(bgm);
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume * bgmCoef;
    }

    public void StartBGM()
    {
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public void PlayOneShot(SFX sfx)
    {
        AudioClip clip = _sfxClips.GetValueOrDefault(sfx);
        sfxSource.PlayOneShot(clip);
    }
}
