using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PlayerUnitTest
{
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        yield return SceneManager.LoadSceneAsync("TestScene");
    }

    [UnityTest]
    public IEnumerator Player_ShouldNotFallThroughGround()
    {
        var player = GameObject.FindWithTag("Player");
        Assert.IsNotNull(player);

        float startY = player.transform.position.y;

        yield return new WaitForSeconds(1f);

        Assert.Less(player.transform.position.y, startY);
    }
}
