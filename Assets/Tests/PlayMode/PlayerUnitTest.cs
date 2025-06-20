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
        
        yield return new WaitForSeconds(1f);
        
        // player 점프 확인
        playerBehav.Jump();
        yield return new WaitForSeconds(0.1f);
        var jumpVelocity1 = rigid.linearVelocity;
        yield return new WaitForSeconds(0.3f);
        var jumpVelocity2 = rigid.linearVelocity;
        
        var jumpPos1 = player.transform.position;
        
        Assert.Greater(jumpVelocity1.magnitude, jumpVelocity2.magnitude);
        Assert.AreEqual(playerBehav._isGrounded, false);
        
        // player 더블 점프 확인
        playerBehav.Jump();
        yield return new WaitForSeconds(0.2f);
        var jumpPos2 = player.transform.position;
        
        Assert.Greater(jumpPos2.y, jumpPos1.y);
        

        // player 트리플 점프 확인 (불가능해야함)
        playerBehav.Jump();
        yield return new WaitForSeconds(0.7f);
        var jumpPos3 = player.transform.position;
        
        Assert.Greater(jumpPos2.y, jumpPos3.y);
        yield return new WaitForSeconds(1f);
        
        // player 와이어 연결 확인 및 스윙
        // 와이어 연결이 잘 되는가?
        // 스윙할 때 키를 받아 반영하는가?
        playerBehav.Jump();
        yield return new WaitForSeconds(0.3f);
        playerBehav.ToggleWireMode();
        
        playerBehav.testInput = Vector2.up + Vector2.left;
        
        yield return new WaitForSeconds(0.1f);
        Assert.AreEqual(playerBehav._isWiring, true);
        
        var swingStartPos = player.transform.position;
        
        yield return new WaitForSeconds(0.2f);
        
        playerBehav.ToggleWireMode();
        playerBehav.testInput = Vector2.zero;
        
        var swingEndPos = player.transform.position;
        
        yield return new WaitForSeconds(0.1f);
        Assert.Greater(swingStartPos.x, swingEndPos.x);
        
        Assert.AreEqual(playerBehav._isWiring, false);

        yield return new WaitForSeconds(0.5f);
        
        // player 카메라 뒤에 있는 와이어 연결 금지 확인
        playerBehav.testInput = Vector2.up;
        yield return new WaitForSeconds(1.3f);
        
        playerBehav.testInput = Vector2.zero;
        yield return new WaitForSeconds(1f);
        
        playerBehav.Jump();
        yield return new WaitForSeconds(0.3f);
        playerBehav.ToggleWireMode();
        yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(playerBehav._isWiring, false);
        
        // player 피격 및 기절 확인
        playerBehav.GetHit(Vector3.up + Vector3.back, 5);
        
        yield return new WaitForSeconds(2f);
        playerBehav.testInput = Vector2.up;
        playerBehav.Jump();
        var hitPos0 = player.transform.position;
        var hitVel0 = rigid.linearVelocity;
        yield return new WaitForSeconds(1f);
        var hitPos1 = player.transform.position;
        playerBehav.testInput = Vector2.zero;
        var hitVel1 = rigid.linearVelocity;
        
        Assert.AreEqual(hitPos0.y, hitPos1.y);
        Assert.Greater(hitVel0.magnitude, hitVel1.magnitude);


        // 플레이어가 땅에 충돌했을 때 _isGrounded 가 true로 바뀌는지
        playerBehav.Jump();
        yield return new WaitForSeconds(1f);
        Assert.AreEqual(playerBehav._isGrounded, true);
        
        // 와이어 중 점프 불가 확인
        playerBehav.Jump();
        yield return new WaitForSeconds(0.3f);
        playerBehav.ToggleWireMode();  // 와이어 시작
        playerBehav.Jump();  // 와이어 중 점프 시도
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(playerBehav._isWiring, true);  // 여전히 와이어 상태여야 함
    }
    
}
