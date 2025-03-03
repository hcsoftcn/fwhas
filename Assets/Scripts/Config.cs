using UnityEngine;

[CreateAssetMenu()]
public class Config : ScriptableObject
{
    // Start is called before the first frame update
   public enum StartType
   {
        Client,
        Server
    };

   public StartType startType;
   public int curLocale;
}
