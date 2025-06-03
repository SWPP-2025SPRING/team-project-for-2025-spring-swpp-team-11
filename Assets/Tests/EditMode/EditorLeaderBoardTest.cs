using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EditorLeaderBoardTest
{
    // 포맷이 잘 지켜져서 예상된 결과대로 잘 나오는지
    [Test]
    [TestCase(0f, "00:00.000")]
    [TestCase(1.234f, "00:01.234")] // 기존 코드에서 부동소수점 오차 있었음 -> Mathf.Round 를 사용하도록 고침
    [TestCase(59.999f, "00:59.999")]
    [TestCase(60f, "01:00.000")]
    [TestCase(61.001f, "01:01.001")]// 기존 코드에서 부동소수점 오차 있었음 -> Mathf.Round 를 사용하도록 고침
    [TestCase(3599.999f, "59:59.999")]
    [TestCase(3600f, "60:00.000")]
    [TestCase(3723.456f, "62:03.456")]
    [TestCase(125.5f, "02:05.500")]
    [TestCase(0.001f, "00:00.001")]
    public void LeaderBoardEntryFormatTest(float timeInSeconds, string expected)
    {
        LeaderBoardEntry entry = new LeaderBoardEntry("TMPNAME", timeInSeconds);
        Assert.AreEqual(expected, entry.GetTime());
    }

    
    // 추가했을 때 정렬된 순서가 잘 유지되는지
    [Test]
    public void LeaderBoardContentTest()
    {
        var content = new LeaderBoardContent(new List<LeaderBoardEntry>
        {
            new LeaderBoardEntry("A", 10f),
            new LeaderBoardEntry("B", 20f),
            new LeaderBoardEntry("C", 30f),
        });

        content.AddRecord("New", 15f, 10);

        Assert.AreEqual("A", content.GetSingleName(0));
        Assert.AreEqual("New", content.GetSingleName(1));
        Assert.AreEqual("B", content.GetSingleName(2));
        Assert.AreEqual("C", content.GetSingleName(3));
        
        Assert.AreEqual(10f, content.GetSingleTime(0));
        Assert.AreEqual(15f, content.GetSingleTime(1));
        Assert.AreEqual(20f, content.GetSingleTime(2));
        Assert.AreEqual(30f, content.GetSingleTime(3));
        
        // Out of Bound 일 경우
        Assert.AreEqual("--------", content.GetSingleName(5));
        Assert.AreEqual(float.PositiveInfinity, content.GetSingleTime(5));
    }
}
