using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Config : ScriptableObject
{
    // Start is called before the first frame update
   public enum StartType
   {
        Client,
        Server,
        Host
    };

   public StartType startType;
}
