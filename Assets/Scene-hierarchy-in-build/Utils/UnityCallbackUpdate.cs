using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AR_RPG.Scripts.Utils
{
 
    public class UnityCallbackUpdate : MonoBehaviour
    {
        public static event Action OnUpdate;

        private static HashSet<Action<UnityCallbackUpdate>> listListeners;
        private static UnityCallbackUpdate instance;
        public static bool AddListener(Action<UnityCallbackUpdate> OnUpdate)
        {
            CreateInstanceIfNeed();
            return listListeners.Add(OnUpdate);
        }

        public static void CreateInstanceIfNeed()
        {
            if (instance == null)
            {
                GameObject updater = new GameObject("UnityCallbackUpdate");
                DontDestroyOnLoad(updater);
                RemoveAllListeners();

                instance = updater.AddComponent<UnityCallbackUpdate>();
            }
        }

        public static async Task OnUpdateFromMainThread(Action onUpdate)
        {
            bool waitThread = true;
            void MainThreadAction()
            {
                onUpdate?.Invoke();
                waitThread = false;
            }

            OnUpdate += MainThreadAction;

            while (waitThread)
            {
                await Task.Delay(100);
            }
            
            OnUpdate -= MainThreadAction;
        }

        public static bool RemoveListener(Action<UnityCallbackUpdate> OnUpdate)
        {
            return listListeners.Remove(OnUpdate);
        }

        public static void RemoveAllListeners()
        {
            listListeners = new HashSet<Action<UnityCallbackUpdate>>();
        }

        public static void RemoveNulls()
        {
            if (listListeners != null && listListeners.Count > 0)
            {
                listListeners.Remove(null);
            }
        }

        void Update () 
        {
            if(listListeners == null)
                return;

            if (listListeners.Count > 0)
            {
                foreach (var listListener in listListeners)
                {
                    listListener.Invoke(this);
                }
            }

            if (OnUpdate != null)
            {
                OnUpdate();
            }
        }

        public static void StartCorontineIntance(IEnumerator startCorontine)
        {
            CreateInstanceIfNeed();
            instance.StartCoroutine(startCorontine);
        }

        public static void StopCorontineIntance(IEnumerator startCorontine)
        {
            CreateInstanceIfNeed();
            instance.StopCoroutine(startCorontine);
        }
    }
}
