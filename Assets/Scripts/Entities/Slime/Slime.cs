using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Health))]
public class Slime : MonoBehaviour
{
    Animator anim;
    Health health;
    void Awake()
    {
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        health.onDeath += PlayDeath;
    }

    void OnDisable()
    {
        health.onDeath -= PlayDeath;
    }

    public void PlayDeath() { anim.SetTrigger("Die"); }
    public void OnDeathFinished() { Destroy(gameObject); }
}