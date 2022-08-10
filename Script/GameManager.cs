using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public int ex;
    public int level;
    public int enemyHealth;
   // public int enemyhealth;
    public Player_Move player;
    public EnemyMove enemyMove;
    public Bullet bullet;
    public GameObject[] Stages;
    public GameObject enemyObject;
    public GameObject PlayerObject;
    public Transform[] spawnPoints;
    public Transform PlayerSpawn;
    
   
    public float maxSpawnDelay;
    public float curSpawnDelay;

    public AudioClip audioDamagedout;
    public AudioSource audioSource;

    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject UIRestartBtn;

     void Awake()
    {
        //Instantiate(PlayerObject, PlayerSpawn.position, PlayerSpawn.rotation);
        audioSource = GetComponent<AudioSource>();
        SpawnEnemy();

    }

    void Update()
    {
        //
        
        /*curSpawnDelay += Time.deltaTime;
        if(curSpawnDelay> maxSpawnDelay)
        {
            //SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f, 3.5f);
            curSpawnDelay = 0;
        }*/

        //점수
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        //Change Stage
        if (stageIndex < Stages.Length-1)
        {
            Stages[stageIndex].SetActive(false); //현재 스테이지 비활성화
            stageIndex++;
            Stages[stageIndex].SetActive(true); //다음 스테이지 활성화
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        //Game Clear
        else
        {
            //Player Contol Lock
            Time.timeScale = 0;
            //Result UI
            Debug.Log("끝!!!");

            //Restart Button UI   
            Text btnText = UIRestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Game Clear!";
            UIRestartBtn.SetActive(true);

        }
        

        //Calculate Point
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.2f);

            
        }
        //죽음
        else
        {
            //Aill Health UI Off
            UIhealth[0].color = new Color(1, 0, 0, 0.2f);

            //Player Die Effect
            player.OnDie();

            //Resilt UI
            Debug.Log("죽음");

            //Retry Button UI
            UIRestartBtn.SetActive(true);
        }
    }

    

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {//Sound
            audioSource.clip = audioDamagedout;
            audioSource.Play();
            //Player Reposition
            if (health > 1)
            {
                PlayerReposition();
            }

            //health DOwn
            HealthDown();
        }
    }
    
    void SpawnEnemy()
    {
        //int ranEney = Random.Range()   //몬스터 현재 1마리라서 생략
        int ranPoint = Random.Range(0, 5);
        Instantiate(enemyObject, spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation);
        Invoke("SpawnEnemy", 5);
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 2, -1);
        player.VelocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
        

    }
}
