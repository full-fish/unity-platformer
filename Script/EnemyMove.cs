using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyMove : MonoBehaviour
{
    public static EnemyMove instance;
    public GameManager gameManager;
    public Bullet bullet;
    public Player_Move player_move;
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsulCollider;
    public int enemyHealth;
    public int nextMove;
    public int bulletDamage;
    int aaa;
    
    void Awake()
    {
        
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsulCollider = GetComponent<CapsuleCollider2D>();
        
        Invoke("Think", 1);  //주어진 시간이 지난 뒤 지정된 함수를 실행하는 함수

    }

    // Update is called once per frame
    void FixedUpdate()
    {   //Move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector2.down, new Color(1, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 1.5f, LayerMask.GetMask("Platform")); //RaycastHit2D : Ray에 닿은 오브젝트
                                                                                                             //Platform에 닿아야지
        if (rayHit.collider == null)//물리엔진이기에 collider
            Turn();

    }

    void Think()
    {
        //Set Next Active
        nextMove = Random.Range(-1, 2);//최저값은 포함되지만 최대값은 포함 안됨

        //Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);

        //Flip Sprite
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;

        float nextThinkTime = Random.Range(3f, 5f);
        Invoke("Think", nextThinkTime); // 재귀함수 : 본인을 호출함  딜레이없이 하면과부하걸려서 위험

    }

    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 2);
    }
    public void OnDamaged()
    {
        //Sprite Alpha 투명화
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        capsulCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //Destroy
        //Invoke("DeActive", 4);
        Invoke("DeActive", 1);
       
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            //Bullet bullet = collision.gameObject.GetComponent<Bullet>();

            EnemyHealthDown(GameObject.Find("Player").GetComponent<Player_Move>().bulletDamage);
            Debug.Log("총알과 적 부딪힘");
        }
        else if (collision.gameObject.tag=="Player" && GameObject.Find("Player").GetComponent<Player_Move>().rigid.velocity.y < 0 && GameObject.FindWithTag("Player").GetComponent<Player_Move>().transform.position.y > transform.position.y)
        EnemyHealthDown(1);
    }
    void Update()
    {
        
    }

    public void EnemyHealthDown(int damage)
    {
        
        
        Debug.Log("업데이트불렛데미지" + GameObject.Find("Player").GetComponent<Player_Move>().bulletDamage);
        Debug.Log("플레이어불렛레벨"+ GameObject.FindWithTag("Player").GetComponent<Player_Move>().BulletLevel);

        Debug.Log("적에게 헬스다운");
        if (enemyHealth > 1)
        {
            enemyHealth -= damage;
        }
        else
        {
            GameObject.Find("Game Manager").GetComponent<GameManager>().stagePoint+=10;
            // gameManager.stagePoint += 10;
            Invoke("OnDamaged", 2);
            GameObject.Find("Game Manager").GetComponent<GameManager>().ex += 2;
            Debug.Log("ex: " + GameObject.Find("Game Manager").GetComponent<GameManager>().ex);


        }
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}

  
