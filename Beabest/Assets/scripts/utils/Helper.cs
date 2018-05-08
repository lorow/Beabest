using UnityEngine;

namespace CharacterController
{
    public class Helper : MonoBehaviour
    {
        [Range(-1, 1)] public float vertical;
        [Range(-1, 1)] public float horizontal;

        public bool playAnim;
        public string[] oh_attacs;
        public string[] th_attacs;

        public bool twoHanded;
        public bool enableRootMotion;
        public bool useItem;
        public bool interacting;
        public bool lockon;
        
        
        private Animator anim;
        
        private void Start()
        {
            anim = GetComponent<Animator>();
        }
       
        private void Update()
        {
            enableRootMotion = !anim.GetBool("canMove");
            anim.applyRootMotion = enableRootMotion;

            interacting = anim.GetBool("interacting");

            if (!lockon)
            {
                horizontal = 0;
                vertical = Mathf.Clamp01(vertical);
            }
            
            anim.SetBool("lockon", lockon);
            
            if(enableRootMotion) return;


            if (useItem)
            {
                anim.Play("use_item");
                useItem = false;
            }

            if (interacting)
            {
                playAnim = false;
                vertical = Mathf.Clamp(vertical, 0, 0.5f);
            }
            
            
            anim.SetBool("two_handed", twoHanded);
            
            if (playAnim)
            {
                string targetAnim;

                if (twoHanded)
                    targetAnim = th_attacs[Random.Range(0, th_attacs.Length)];
                else
                    targetAnim = oh_attacs[Random.Range(0, oh_attacs.Length)];
                
                
                if (vertical>.5f)
                    targetAnim = "oh_attack_3";
                
                vertical = 0;
                anim.CrossFade(targetAnim, 0.2f);
                playAnim = false;
            }
            anim.SetFloat("vertical", vertical);
            anim.SetFloat("horizontal",horizontal);
        }
    }
}

