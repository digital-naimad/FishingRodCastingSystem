using System.Collections.Generic;
using UnityEngine;

namespace FishingRodSystem
{
    /// <summary>
    /// Thread-safe implementation of Singleton Pattern for MonoBehaviour.
    /// Note: Based on dictionary instead of using FindObjectsOfType or creating GameObject during the game, which are very inefficient
    /// </summary>
    /// <typeparam name="T">The type we want to make a singleton</typeparam>

    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        #region Static fields
        protected static bool Quitting { get; private set; }

        private static readonly object Lock = new object();
        private static Dictionary<System.Type, MonoSingleton<T>> instancesDictionary;
        #endregion

        #region Static instance getter
        public static T Instance
        {
            get
            {
                lock (Lock)
                {
                    if (instancesDictionary == null)
                    {
                        instancesDictionary = new Dictionary<System.Type, MonoSingleton<T>>();
                    }

                    if (instancesDictionary.ContainsKey(typeof(T)))
                    {
                        return (T)instancesDictionary[typeof(T)];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        #endregion

        #region MonoBehaviour's inherited methods
        private void OnEnable()
        {
            lock (Lock)
            {
                if (instancesDictionary == null)
                {
                    instancesDictionary = new Dictionary<System.Type, MonoSingleton<T>>();
                }

                if (instancesDictionary.ContainsKey(this.GetType()))
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    instancesDictionary.Add(this.GetType(), this);

                    DontDestroyOnLoad(gameObject);
                }
            }
        }
        #endregion
    }
}