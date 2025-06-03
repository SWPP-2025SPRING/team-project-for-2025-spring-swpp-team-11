using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EditorLeaderBoardTest
{
    [Test]
    [TestCase(0f, "00:00.000")]
    [TestCase(1.234f, "00:01.234")]
    [TestCase(59.999f, "00:59.999")]
    [TestCase(60f, "01:00.000")]
    [TestCase(61.001f, "01:01.001")]
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
}
