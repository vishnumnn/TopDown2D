using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Node : MonoBehaviour
    {
        // Properties
        public Vector3 position;
        public int Id;
        public int currentBlips;
        public int Player;
        public List<GameObject> neighbors;
        
        // Blip generation cooldown
        private float cooldown = 1.0f;
        // Ability to generate blips
        private bool canGenerateBlips = true;

        public float getDist(Node other)
        {
            if (other == null)
            {
                return Mathf.Infinity;
            }
            return Mathf.Sqrt(Mathf.Pow(this.position.x - other.position.x, 2) + Mathf.Pow(this.position.y - other.position.y, 2));
        }

        public override bool Equals(System.Object o)
        {
            if (o == null)
                return false;
            Node obj = (Node)o;
            if (this.Id != obj.Id)
                return false;
            if (this.position.x != obj.position.x || this.position.y != obj.position.y)
                return false;

            return true;
        }

        private void SpawnBlip()
        {
            // Make random direction to eject node
            Vector2 x = (new Vector2(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f))).normalized;
            Vector3 ejectDirection = new Vector3(x.x, x.y, 1);

            // Create Blip
            GameObject newBlip = ObjectPoolContainer.sharedInstance.RetrieveObjectByTag("GreenBlip");
            if(newBlip != null)
            {

                newBlip.transform.position = gameObject.transform.position;
                newBlip.AddComponent<Blip>();
                Blip blipScript = newBlip.GetComponent<Blip>();
                blipScript.BlipState = Blip.State.Ejection;
                blipScript.Direction = ejectDirection;
                blipScript.OriginDest = new GameObject[2];
                blipScript.OriginDest[0] = gameObject;
                blipScript.Player = 1;
                currentBlips++;
            }
            else
            {
                canGenerateBlips = false;
            }
        }

        IEnumerator BlipDispatcher()
        {
            while (canGenerateBlips)
            {
                yield return new WaitForSeconds(cooldown);
                SpawnBlip();
            }
        }
        private void Start()
        {
            StartCoroutine(BlipDispatcher());
        }

        private void Update()
        {
        }

        private void OnEnable()
        {
            
        }

    }
}