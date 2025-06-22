using NUnit.Framework;
using UnityEngine.TestTools;
using System.Collections;

public class SFXSliderManualTest
{
    [UnityTest]
    public IEnumerator ManualTest_SFXVolumeSlider()
    {
        // Manual Test - SFX Volume Slider
        // Steps:
        // 1. Go to Settings -> Sound tab.
        // 2. Set SFX slider to 0%.
        // 3. Trigger an action that plays a sound (e.g. button click).
        // 4. Set SFX slider to 100% and repeat.
        //
        // Expected:
        // - No sound at 0%.
        // - Sound plays at 100%.

        yield return null;

        Assert.Pass("This is a manual test. Follow instructions in code comments.");
    }
}
