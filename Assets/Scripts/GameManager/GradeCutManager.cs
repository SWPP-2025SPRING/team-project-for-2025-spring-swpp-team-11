using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cutline
{
    public string grade;
    public float maxTime;
}

[System.Serializable]
public class GradeCut
{
    public int stage;
    public List<Cutline> cutlines;//higher->lower order
    public string lowestGrade;
}

public class GradeCutManager : MonoBehaviour
{
    [SerializeField] private List<GradeCut> gradeCuts;

    public string GetGradeByTime(int stage,float record)
    {
        foreach(GradeCut gradeCut in gradeCuts)
        {
            if (gradeCut.stage != stage) continue;
            foreach(Cutline cutline in gradeCut.cutlines)
            {
                if (record < cutline.maxTime) return cutline.grade;
            }
            return gradeCut.lowestGrade;
        }
        return "";
    }
}
