//GameObject.Find("오브젝트이름").GetComponent<스크립트이름>().함수및변수;
//GameObject.FindWithTag("Player").GetComponent<Player_Move>().bulletDamage
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Scripting;

public class Player_Move : MonoBehaviour
{
    public GameManager gameManager;
    public EnemyMove enemyMove;
    public Bullet bullet;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioCoin;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public int bulletDamage;
    public int StepupHeadDamage;
    public float MaxSpeed;
    public float JumpPower;
    public Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsulCollider;
    AudioSource audioSource;
    int Playerdirection;
    public int BulletLevel;
    
    
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsulCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }



    void Update()
    {
        if (Input.GetButtonDown("Fire3")) 
            Fire();

        JumpUpdate();

        WalkingUpdate();
        if (GameObject.Find("Game Manager").GetComponent<GameManager>().ex == 10)
        {
            GameObject.Find("Game Manager").GetComponent<GameManager>().level = 2;
            BulletLevel = 2;
        }
        if (BulletLevel == 1)
        {
            bulletDamage = 1;
            Debug.Log("플레이어에서의 불렛데미지" + bulletDamage);
            Debug.Log("플레이어에서의 불렛레벨" + BulletLevel);
        }
           
        else if (BulletLevel == 2)
        {
            bulletDamage = 4;
            Debug.Log("플레이어에서의 불렛데미지" + bulletDamage);
            Debug.Log("플레이어에서의 불렛레벨" + BulletLevel);
        }
            





    }

    void FixedUpdate()
    {
        WalkingFixedUpdate();

        JumpFixedUpdate();


        
    }





    void WalkingUpdate()
    {
        //멈출때 속도
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.45f, rigid.velocity.y); //normalized : 벡터 크기를 1로 만든 상태 (방향 구할떄 좌우니까) 
        }

        //방향 전환
        
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;//false가 오른쪽
        if (spriteRenderer.flipX == true)
        {
            
              
                
        }
        //wakling 애니메이션
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }
    
    void WalkingFixedUpdate()
    {

        /*float h = Input.GetAxisRaw("Horizontal");
        rigid.velocity = new Vector2(MaxSpeed * h, rigid.velocity.y);*/
         //좌우 움직임
         float h = Input.GetAxisRaw("Horizontal");

         rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

         if (rigid.velocity.x > MaxSpeed)//오른쪽 최대 속도 velocity=리지드바디의 현재 속도
             rigid.velocity = new Vector2(MaxSpeed, rigid.velocity.y);
         else if (rigid.velocity.x < MaxSpeed * (-1))//왼쪽 최고 속도
             rigid.velocity = new Vector2(MaxSpeed * (-1), rigid.velocity.y);
    }

    void JumpUpdate()
    {
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
            rigid.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);

            //PlaySound
            PlaySound("JUMP");
        }
    }

    void JumpFixedUpdate()
    {
        //Landing Platform
        //바닥과 닿는거   물체와 닿는 이벤트 함수로도 할 수 있지만 raycast함수로도 가능


        //점프모션관련 레이케스트
        Debug.DrawRay(rigid.position, Vector2.down, new Color(1, 1, 0)); //RayCast : 오브젝트 검색을 위해 Ray를 쏘는 방식
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector2.down, 1, LayerMask.GetMask("Platform")); //RaycastHit2D : Ray에 닿은 오브젝트
                                                                                                                 //Platform에 닿아야지
        if (rayHit.collider != null && rigid.velocity.y < 0)//물리엔진이기에 collider, 없는게 아니라면   rayHIT은 관통안함 부딪히면 끝
        {
            if (rayHit.distance < 0.65f) //값이 작으면 공중에 있는거로 인식, 값이 크면 점프 모션안나와서 2단 점프
            {
                //크기 1이고 중앙에서 시작하니까
                anim.SetBool("isJumping", false);
            }
            else
            {
                anim.SetBool("isJumping", true);

                Debug.Log("점프모션");
            }
        }
        //플렛폼 충돌 감지
        if (rayHit.collider != null)
        {
            Debug.Log("플레이어가 닿아있는 바닥 :"+rayHit.collider.name);
        }
    }

   

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("플레이어 물리충돌 :" + collision.gameObject.tag);
        if (collision.gameObject.tag == "Spike")
        {  
            OnDamaged(collision.transform.position);
        }

        if (collision.gameObject.tag == "Enemy")
        {
            //Attack
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
                SteponHead();
            //Damagged
            else
                OnDamaged(collision.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("플레이어 트리거 충돌 :"+collision.gameObject.tag);
        if (collision.gameObject.tag == "Coin")
        {
            

            //Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");// 이름에 Bronze있으면 true
           bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
                gameManager.stagePoint += 10;
            else if (isGold)
                gameManager.stagePoint += 50;
            //Deactive Coin
            collision.gameObject.SetActive(false);

            //Sound
            PlaySound("COIN");
        }

        else if (collision.gameObject.tag == "Finish")
        {
            //Next Stage    
            gameManager.NextStage();

            //Sound
            PlaySound("FINISH");
        }
    }

    public void SteponHead()
    {

        Debug.Log("머리밟음");
        //Reaction Force
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);


       // GameObject.Find("Enemy(Clone)").GetComponent<EnemyMove>().EnemyHealthDown(1);

        //Sound
        PlaySound("ATTACK");
    }

    void Fire()
    {
        if (spriteRenderer.flipX == false)//오른쪽 최대 속도 velocity=리지드바디의 현재 속도
            Playerdirection = 1;
        else
            Playerdirection = -1;
        if (BulletLevel == 1)
        {
            GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            rigid.AddForce(Vector2.right * 40 * Playerdirection, ForceMode2D.Impulse);
            Bullet bulletlook = bullet.GetComponent<Bullet>();
            if (Playerdirection == 1)
                bulletlook.Rotation();
            else
                bulletlook.RotationOpposite();
        }
        else if(BulletLevel == 2)
        {
            
            GameObject bullet = Instantiate(bulletObjB, transform.position, transform.rotation);
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            rigid.AddForce(Vector2.right * 40 * Playerdirection, ForceMode2D.Impulse);
            Bullet bulletlook = bullet.GetComponent<Bullet>();
            if (Playerdirection == 1)
                bulletlook.Rotation();
            else
                bulletlook.RotationOpposite();
        }
         






       

    }

    void OnDamaged(Vector2 targetPosition)
    {
        //healt Down
        gameManager.HealthDown();
        
        // Change Layer
        gameObject.layer = 11;

        //깜빡임
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //튕겨나감
        
        int dirc = transform.position.x - targetPosition.x > 0 ? 1 : -1;  //0보다 크면 1  아니면 -1
        rigid.AddForce(new Vector2(dirc , 1) * 7, ForceMode2D.Impulse);
        
        //Anmation
        anim.SetTrigger("doDamaged");
        anim.SetTrigger("offDamaged");
        Invoke("OffDamaged", 2);

        //Sound
        PlaySound("DAMAGED");

    }
    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        //Sprite Alpha 투명화
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        capsulCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //Sound
        PlaySound("DIE");
    }
 
    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "COIN":
                audioSource.clip = audioCoin;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        audioSource.Play();
    }

}
