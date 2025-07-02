using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gamemode : MonoBehaviour
{
    public int day = 0;
    public string phase = "Night";
    public Dictionary<string, float> phaseDuration = new Dictionary<string, float>
    {
        { "Day", 10f },
        { "Evening", 5f },
        { "Night", 10f }
    };
    public float timeUntilNextPhase;

    public Base baseScript;
    private DungeonGenerator dungeonGen;
    public TextMeshProUGUI timerText;

    void Start()
    {
        dungeonGen = GetComponent<DungeonGenerator>();
        RunNextPhase();
    }

    void Update()
    {
        if (timeUntilNextPhase <= 0)
        {
            RunNextPhase();
        }
        timeUntilNextPhase -= Time.deltaTime * ((baseScript.isBoosted) ? baseScript.boostMultiplier : 1);
        timerText.text = timeUntilNextPhase.ToString();
    }

    void RunNextPhase()
    {
        if (phase == "Day")
        {
            phase = "Evening";
        }
        else if(phase == "Evening")
        {
            phase = "Night";
        }
        else if (phase == "Night")
        {
            phase = "Day";
            day++;
            timeUntilNextPhase = phaseDuration[phase];
            dungeonGen.GenerateLevel();
        }
        timeUntilNextPhase = phaseDuration[phase];
    }
}
