using Assets.Scripts.Entity;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class GameOverManager : MonoBehaviour
    {
        public EntityHealth playerHealth;


        Animator anim;


        void Awake()
        {
            anim = GetComponent<Animator>();
        }


        void Update()
        {
            if (playerHealth.currentHealth <= 0)
            {
                anim.SetTrigger("GameOver");
            }
        }
    }
}