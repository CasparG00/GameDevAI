using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T myInstance;
    public static T instance
    {
        get
        {
            if (myInstance != null) return myInstance;
            
            var result = FindObjectOfType<T>();
            if(result != null)
            {
                myInstance = result;
            }
            return myInstance;
        }
        set => myInstance = value;
    }
}