using UnityEngine;

/// <summary>
/// このベースクラスを継承してシングルトンを作成する
/// e.g. public class MyClassName : Singleton<MyClassName> {}
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // 破壊されそうになっているかどうかをチェックする
    private static bool m_ShuttingDown = false;
    private static object m_Lock = new object();
    private static T m_Instance;

    /// <summary>
    /// このプロパティを使ってシングルトンのインスタンスにアクセスする
    /// </summary>
    public static T Instance
    {
        get
        {
            if (m_ShuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed. Returning null.");
                return null;
            }

            lock (m_Lock)
            {
                if (m_Instance == null)
                {
                    // 既存のインスタンスを検索する
                    m_Instance = (T)FindObjectOfType(typeof(T));

                    // まだ存在していない場合は、新しいインスタンスを作成する
                    if (m_Instance == null)
                    {
                        // シングルトンをアタッチするためには、新しいGameObjectを作成する必要がある
                        var singletonObject = new GameObject();
                        m_Instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";

                        // インスタンスの持続性を高める
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return m_Instance;
            }
        }
    }


    private void OnApplicationQuit()
    {
        m_ShuttingDown = true;
    }


    private void OnDestroy()
    {
        m_ShuttingDown = true;
    }
}
