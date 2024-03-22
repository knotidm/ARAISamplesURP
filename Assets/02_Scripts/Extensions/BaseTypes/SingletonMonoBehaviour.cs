using UnityEngine;

public class SingletonMonoBehaviour<T> : ExtendedMonoBehaviour where T : Component
{
    private static T instance;

    public static T Instance
    {
        get { return instance; }
        protected set { instance = value; }
    }

    protected virtual void Awake()
    {
        Instance = this as T;
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
