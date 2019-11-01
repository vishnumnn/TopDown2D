using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Node
    {
        public Vector3 position;
        public int Id;
        public int BlipCount;
        public int Player;
        public List<Node> neighbors;

        public Node()
        {

        }

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
    }
}