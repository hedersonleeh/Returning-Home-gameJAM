using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu]
public class DeletePlayerPrefs : ScriptableObject
{
     [Button]
     public void Delete()
     {
          PlayerPrefs.DeleteAll();
     }
}
