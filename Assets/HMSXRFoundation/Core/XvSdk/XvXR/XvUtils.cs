using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HMS.openxr.Input
{
    public class XvUtils
    {
        public static void log(string tag, string info)
        {
            Debug.Log("HMS#" + tag + "====> " + info);
        }
    }
}