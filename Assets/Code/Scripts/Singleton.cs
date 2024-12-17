using global::UnityEngine;

public abstract class Singleton<T> : UnityEngine.MonoBehaviour where T : global::Singleton<T>
{
    private static T instance;

    [UnityEngine.SerializeField] private bool dontDestroyOnLoad = true;

    public static T Instance
    {
        get
        {
            if (Singleton<T>.instance == null)
            {
                Singleton<T>.instance = UnityEngine.Object.FindFirstObjectByType<T>();

                if (Singleton<T>.instance == null)
                {
                    UnityEngine.GameObject go = new UnityEngine.GameObject($"{typeof(T).Name}");
                    Singleton<T>.instance = go.AddComponent<T>();
                }
            }
            return Singleton<T>.instance;
        }
    }

    protected virtual void Awake()
    {
        if (Singleton<T>.instance != null && Singleton<T>.instance != this)
        {
            UnityEngine.Object.Destroy(gameObject);
            return;
        }

        Singleton<T>.instance = (T)this;

        if (dontDestroyOnLoad)
        {
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
        }

        Init();
    }

    protected virtual void Init() { }
}
