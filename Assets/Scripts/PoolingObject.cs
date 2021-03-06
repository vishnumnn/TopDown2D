﻿using System;
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
        [Range(2, 6000)]
        public int poolSize = 60;
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

        public override string ToString()
        {
            return $"Pooling Object: {objectToPool.tag} Pool Size: {poolSize}";
        }
    }
}
