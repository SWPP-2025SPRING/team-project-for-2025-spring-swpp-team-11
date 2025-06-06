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
    public IEnumerator Player_UnitTest()
    {
        // player 참조
        var player = GameObject.FindWithTag("Player");
        var playerBehav = player.GetComponent<TestPlayerBehavior>();
        var rigid = player.GetComponent<Rigidbody>();
        
        Assert.IsNotNull(player);

        // player 중력 적용 확인, isGrounded 확인
        float startY = player.transform.position.y;
        yield return new WaitForSeconds(2f);
        Assert.Less(player.transform.position.y, startY);
        Assert.AreEqual(playerBehav._isGrounded, true);
        
        
        // player 가속 확인
        playerBehav.testInput = Vector2.up;
        var startVelocity = rigid.linearVelocity;
        
        yield return new WaitForSeconds(1f);
        var endVelocity = rigid.linearVelocity;
        
        Assert.Greater(endVelocity.magnitude, startVelocity.magnitude);
        playerBehav.testInput = Vector2.zero;
    }
    
}
