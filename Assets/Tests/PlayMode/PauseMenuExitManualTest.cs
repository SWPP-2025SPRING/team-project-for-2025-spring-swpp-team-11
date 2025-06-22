using NUnit.Framework;
using UnityEngine.TestTools;
using System.Collections;

public class PauseMenuExitManualTest
{
    [UnityTest]
    public IEnumerator ManualTest_PauseMenuExit()
    {
        // Manual Test - Pause Menu Exit
        // Steps:
        // 1. Enter gameplay.
        // 2. Press 'ESC' to open pause menu.
        // 3. Click 'Exit' button.
        //
        // Expected:
        // - Scene changes to Map Select (1_StageSelectSceneAppliedOne).
        // - Game is properly paused before exit.

        yield return null;

        Assert.Pass("This is a manual test. Follow instructions in code comments.");
    }
}
