using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using System.Collections;

public class KeyRebindTest
{
    [UnityTest]
    public IEnumerator KeyRebinding_UpdatesControlKey()
    {
        var rebindUI = GameObject.FindObjectOfType<KeyRebindingUI>();
        Assert.IsNotNull(rebindUI);

        var controlManager = GameObject.FindObjectOfType<ControlManager>();
        Assert.IsNotNull(controlManager);

        // 기존 키 저장
        KeyCode originalJumpKey = controlManager.GetKey("Jump");

        // 키 재바인딩 UI에서 점프 키를 'K'로 변경
        yield return rebindUI.StartKeyRebind("Jump", KeyCode.K);
        yield return null;

        KeyCode updatedKey = controlManager.GetKey("Jump");
        Assert.AreEqual(KeyCode.K, updatedKey);

        // 테스트 후 원래대로 복원
        controlManager.SetKey("Jump", originalJumpKey);
    }
}
