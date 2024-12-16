using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObject : CellObject
{

    public int damage;

    private Animator m_Anim;

    public static event System.Action<EnemyObject> OnMorir;

    public static event System.Action OnHerir;


    private void Start()
    {
        m_Anim = GetComponent<Animator>();
    }

    public override void PlayerEntered()
    {
        m_Anim.SetTrigger("Attack");
        OnHerir?.Invoke();
        GameManager.Instance.RestarComida(damage);
        Destroy(this.gameObject);
        OnMorir?.Invoke(this);
    }



}
