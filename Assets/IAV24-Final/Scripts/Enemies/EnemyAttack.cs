using IAV24.Final;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IAV24.Final
{
    public class EnemyAttack : MonoBehaviour
    {
        [SerializeField]
        private float damage = 1.0f;

        private Animator anim;

        private void Start()
        {
            anim = transform.parent.gameObject.GetComponent<Animator>();
        }

        void OnTriggerStay(Collider collision)
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Reproduce la animacion de ataque y aplica el dano una vez ha
                // terminado (en este caso, a la mitad de la animacion, ya que
                // coincide con el momento en el que el enemigo ataca)
                anim.Play("Attack");
                StartCoroutine(makeDamage(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length / 2, playerHealth, damage));
            }
        }

        private IEnumerator makeDamage(float delay, PlayerHealth playerHealth, float damage)
        {
            yield return new WaitForSecondsRealtime(delay);
            playerHealth.makeDamage(damage);
        }
    }
}