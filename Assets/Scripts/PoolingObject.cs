using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class PoolingObject
    {
        public int poolSize;
        public GameObject objectToPool;
        public bool expandable;
    }
}
