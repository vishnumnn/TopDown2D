using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class PartitionUtility
    {
        public static int pCount;
        public static List<GameObject>[,] partMat;

        public PartitionUtility(int numPartitions)
        {
            pCount = numPartitions;
            partMat = new List<GameObject>[pCount, pCount];
            // initialize 2d array with lists.
            for(int i = 0; i < pCount; i++)
            {
                for(int j = 0; j < pCount; j++)
                {
                    partMat[i, j] = new List<GameObject>();
                }
            }
        }

        public static Tuple<int, int> getIndex(GameObject g)
        {
            float val = 1f/ pCount;
            Vector3 pos = Camera.main.WorldToViewportPoint(g.transform.position);// This camera reference might cause problems
            int indx = (int)(pos.x / val);
            int indy = (int)(pos.y / val);
            return new Tuple<int, int>( 
                (indx) == pCount ? pCount - 1 : indx, 
                (indy) == pCount ? pCount - 1 : indy);
        }
    }
}
