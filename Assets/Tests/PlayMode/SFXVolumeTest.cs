using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using System.Collections;

public class SFXVolumeTest
{
    [UnityTest]
    public IEnumerator SFXSlider_AdjustsVolume()
    {
        var audioManager = GameObject.FindObjectOfType<AudioManager>();
        Assert.IsNotNull(audioManager);

        var sfxSlider = GameObject.Find("SFXSlider")?.GetComponent<Slider>();
        Assert.IsNotNull(sfxSlider);

        float originalVolume = audioManager.GetSFXVolume();

        sfxSlider.value = 0.3f;
        yield return null;

        Assert.AreEqual(0.3f, audioManager.GetSFXVolume(), 0.01f);

        sfxSlider.value = originalVolume; // 복원
        yield return null;
    }
}
