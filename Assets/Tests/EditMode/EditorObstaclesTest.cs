using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EditorObstaclesTest
{
    private static IEnumerable BouncingRockTestCases
    {
        get
        {
            // (playerPos, rockPos, expectedResult)
            yield return new TestCaseData(new Vector3(0, 0, 0), new Vector3(0, 2, 0), true);         // 완전 아래
            yield return new TestCaseData(new Vector3(0, 0, 0), new Vector3(0, 1, 0), false);         // detectionHeightOffset 에 걸칠 때는 false
            yield return new TestCaseData(new Vector3(3, 1, 4), new Vector3(0, 5, 0), true);         // 아래 + 거리 5보다 작음
            yield return new TestCaseData(new Vector3(3, 1, 4), new Vector3(0, 6, 0), true);        // 거리 = 5, true
            yield return new TestCaseData(new Vector3(6, 1, 0), new Vector3(0, 5, 0), false);        // 거리 > 5
            yield return new TestCaseData(new Vector3(1, 10, 1), new Vector3(0, 5, 0), false);       // 위에 있음
            yield return new TestCaseData(new Vector3(1, 2, 1), new Vector3(0, 5, 0), true);         // 아래 + 거리 sqrt(2)
            yield return new TestCaseData(new Vector3(4, 5, 0), new Vector3(0, 5, 0), false);        // y 같음
            yield return new TestCaseData(new Vector3(4.99f, 0, 0), new Vector3(0, 10, 0), true);    // 아래 + 거리 4.99
            yield return new TestCaseData(new Vector3(5.01f, 0, 0), new Vector3(0, 10, 0), false);   // 아래 + 거리 5.01
            yield return new TestCaseData(new Vector3(0, 0, 0), new Vector3(0, 0, 0), false);        // 같은 위치
        }
    }
    
    private static IEnumerable IsPlayerDirectlyAboveTestCases
    {
        get
        {
            // detectionRadius = 5, detectionHeightOffset = 1 기준

            // (playerPos, rockPos, expectedResult)
            yield return new TestCaseData(new Vector3(0, 2, 0), new Vector3(0, 0, 0), true);         // 완전 위
            yield return new TestCaseData(new Vector3(3, 6, 4), new Vector3(0, 0, 0), true);         // 위 + 거리 < 5
            yield return new TestCaseData(new Vector3(3, 6, 4), new Vector3(0, 2, 0), true);        // 거리 = 5 (경계 → true)
            yield return new TestCaseData(new Vector3(6, 7, 0), new Vector3(0, 0, 0), false);        // 거리 > 5
            yield return new TestCaseData(new Vector3(1, 0, 1), new Vector3(0, 5, 0), false);        // 아래에 있음
            yield return new TestCaseData(new Vector3(1, 7, 1), new Vector3(0, 5, 0), true);         // 위 + 거리 sqrt(2)
            yield return new TestCaseData(new Vector3(4, 5, 0), new Vector3(0, 5, 0), false);        // y 같음
            yield return new TestCaseData(new Vector3(4.99f, 10, 0), new Vector3(0, 0, 0), true);    // 위 + 거리 < 5
            yield return new TestCaseData(new Vector3(5.01f, 10, 0), new Vector3(0, 0, 0), false);   // 거리 > 5
            yield return new TestCaseData(new Vector3(0, 0, 0), new Vector3(0, 0, 0), false);        // 같은 위치
        }
    }
    
    
    [Test, TestCaseSource(nameof(BouncingRockTestCases))]
    public void BouncingRockDetectTest(Vector3 playerPos, Vector3 rockPos, bool expectedResult)
    {
        GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        BouncingRock bouncingRock = rock.AddComponent<BouncingRock>();
        
        // detection Radius 는 5
        // detection height offset 은 1 이라고 가정 했습니다.
        
        bouncingRock.detectionRadius = 5;
        bouncingRock.detectionHeightOffset = 1;
        
        bool result = bouncingRock.IsPlayerDirectlyBelow(playerPos, rockPos);
        
        Assert.AreEqual(expectedResult, result);
    }
    
    [Test, TestCaseSource(nameof(IsPlayerDirectlyAboveTestCases))]
    public void UpwardObstacleDetectTest(Vector3 playerPos, Vector3 rockPos, bool expectedResult)
    {
        GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        UpwardObstacle bouncingRock = rock.AddComponent<UpwardObstacle>();
        
        // detection Radius 는 5
        // detection height offset 은 1 이라고 가정 했습니다.
        
        bouncingRock.detectionRadius = 5;
        bouncingRock.detectionHeightOffset = 1;
        
        bool result = bouncingRock.IsPlayerDirectlyAbove(playerPos, rockPos);
        
        Assert.AreEqual(expectedResult, result);
    }
}
