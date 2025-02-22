using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace IAV24.Final
{
    public class MagicBullet : MonoBehaviour
    {
        private MagicPower magicPower;
        private float lifeTime;
        private float elapsedTime = 0.0f;

        private void OnCollisionEnter(Collision collision)
        {
            GameObject colObject = collision.gameObject;
            if (colObject.tag == "Enemy")
            {
                magicPower.removeDetectedEnemy(colObject);

                Animator anim = colObject.GetComponent<Animator>();
                // Reproduce la animacion de muerte y destruye tanto
                // al enemigo como la bala una vez termina la animacion
                anim.Play("Death");
                StartCoroutine(destroyEnemy(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length, colObject));
            }
            
        }
        private IEnumerator destroyEnemy(float delay, GameObject colObject)
        {
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            this.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
            colObject.GetComponentInChildren<SphereCollider>().enabled = false;
            colObject.GetComponent<NavMeshAgent>().velocity = new Vector3(0, 0, 0);
            yield return new WaitForSecondsRealtime(delay);
            Destroy(colObject);
            Destroy(this.gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime > lifeTime)
            {
                elapsedTime = 0.0f;
                Destroy(this.gameObject);
            }
        }

        public void setParams(MagicPower magicPower, float lifeTime)
        {
            this.magicPower = magicPower;
            this.lifeTime = lifeTime;
        }
    }
}