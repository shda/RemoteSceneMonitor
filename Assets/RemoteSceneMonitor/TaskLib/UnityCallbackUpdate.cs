using System;
using System.Collections.Generic;
using UnityEngine;

namespace TaskLib
{
    public class UnityCallbackUpdate : MonoBehaviour
    {
        private static Stack<Action> listListeners = new Stack<Action>();
        private static UnityCallbackUpdate instance;
    
        public static void AddListener(Action OnUpdate)
        {
            CreateInstanceIfNeed();
            lock (listListeners)
            {
                listListeners.Push(OnUpdate); 
            }
        }

        public static void CreateInstanceIfNeed()
        {
            if (instance == null)
            {
                GameObject updater = new GameObject("UnityCallbackUpdate");
                DontDestroyOnLoad(updater);
                instance = updater.AddComponent<UnityCallbackUpdate>();
            }
        }
        void Update()
        {
            if (listListeners == null)
                return;

            try
            {
                Action[] arrayActions = null;
                lock (listListeners)
                {
                    arrayActions = listListeners.ToArray();
                    listListeners.Clear();
                }
            
                foreach (var listListener in arrayActions)
                {
                    try
                    {
                        listListener?.Invoke();
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogException(e);
                    }
                }
            }
            catch (Exception e)
            {
         
            }
        }
    }
}