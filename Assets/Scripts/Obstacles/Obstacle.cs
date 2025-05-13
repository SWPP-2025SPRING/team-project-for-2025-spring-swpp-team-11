using System;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float knockbackConst;
    public float stunTime;
    public float lifetime = 5f;


    private void Start()
    {
        Destroy(gameObject, lifetime); // 일정 시간 뒤 자동 삭제
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerBehavior p = other.gameObject.GetComponent<PlayerBehavior>();
            
            
            p.GetHit((other.transform.position - transform.position).normalized * knockbackConst + Vector3.up * knockbackConst, stunTime);
        }
    }
}
