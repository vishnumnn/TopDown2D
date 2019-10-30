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

        public override bool Equals(object obj)
        {
            PoolingObject p = (obj as PoolingObject);
            if (p.objectToPool.Equals(objectToPool) && p.poolSize == poolSize && p.expandable == expandable)
            {
                return true;
            }
            return false;
        }
    }
}
