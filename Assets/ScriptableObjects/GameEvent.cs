using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "NewGameEvent")]
public class GameEvent : ScriptableObject
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     [SerializeField] private bool verboseRaise;
     private List<GameEventListener> listeners = new List<GameEventListener>();

     /*************************************************************************************************
     *** Methods
     *************************************************************************************************/
     [Button]
     public void Raise()
     {
          for (int i = listeners.Count - 1; i >= 0; i--)
               listeners[i].OnEventRaised();

          if (verboseRaise)
               Debug.Log(string.Concat(name, ": Raised."));
     }

     public void RegisterListener(GameEventListener eventListener)
     {
          if (!listeners.Contains(eventListener))
               listeners.Add(eventListener);
     }

     public void UnregisterListener(GameEventListener eventListener)
     {
          if (listeners.Contains(eventListener))
               listeners.Remove(eventListener);
     }
}
