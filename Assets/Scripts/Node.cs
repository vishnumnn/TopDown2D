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
        public float x;
        public float y;
        public int Id;
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
            return Mathf.Sqrt(Mathf.Pow(this.x - other.x, 2) + Mathf.Pow(this.y - other.y, 2));
        }
    }
}
