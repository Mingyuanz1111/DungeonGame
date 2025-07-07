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
        { "Day", 5f },
        { "Evening", 2f },
        { "Night", 5f }
    };
    public float timeUntilNextPhase;

    public Base baseObject;
    private Base baseScript;
    private DungeonGenerator dungeonGen;
    public Fog fogScript;
    public TextMeshProUGUI timerText;

    void Start()
    {
        baseScript = baseObject.GetComponent<Base>();
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
            fogScript.Move(baseObject.transform.position, new Vector3(dungeonGen.roomSize, dungeonGen.roomSize, 1f), 120f);
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
