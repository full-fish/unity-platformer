using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    


    Rigidbody2D rigid;
    // Start is called before the first frame update

 
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
       
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
        
    }





    public void Rotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
    }

    public void RotationOpposite()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
    }
}
