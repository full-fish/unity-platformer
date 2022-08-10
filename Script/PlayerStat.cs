using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat instance;
    public int playerLevel;
    public int[] needExp;
    public int heath;
    public int currentHeath;
    public int mp;
    public int currentMp;
    public int atk;
    public int dff;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   public void atk2()
    {
        GameObject.Find("Game Manager").GetComponent<GameManager>().stagePoint += 100;
    }
}
