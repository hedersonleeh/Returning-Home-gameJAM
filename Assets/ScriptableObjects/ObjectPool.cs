using UnityEngine;
using System.Collections.Generic;

public interface IObjectPooled { ObjectPool Pool { get; set; } }

[CreateAssetMenu(fileName = "NewObjectPool", menuName = "Object Pool")]
public class ObjectPool : ScriptableObject
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     public GameObject prefab;
     private Queue<GameObject> pool;

     /*************************************************************************************************
     *** OnEnable
     *************************************************************************************************/
     private void OnEnable()
     {
          pool = new Queue<GameObject>();
     }

     /*************************************************************************************************
     *** OnDisable
     *************************************************************************************************/
     private void OnDisable()
     {
          pool.Clear();
     }

     /*************************************************************************************************
     *** Properties
     *************************************************************************************************/

     /*************************************************************************************************
     *** Methods
     *************************************************************************************************/
     public GameObject Spawn(Transform parent, Vector3 worldPosition)
     {
          GameObject spawnedObject = null;

          if (pool.Count > 0)
               spawnedObject = pool.Dequeue();

          if (spawnedObject == null)
          {
               spawnedObject = Instantiate(prefab, null, false);
               spawnedObject.GetComponent<IObjectPooled>().Pool = this;
          }

          spawnedObject.transform.parent = parent;
          spawnedObject.transform.position = worldPosition;
          spawnedObject.SetActive(true);

          return spawnedObject;
     }

     public GameObject Spawn(Transform parent, Vector3 worldPosition, Quaternion worldRotation)
     {
          GameObject spawnedObject = Spawn(parent, worldPosition);
          spawnedObject.transform.rotation = worldRotation;

          return spawnedObject;
     }

     public void Vanish(GameObject gameObj)
     {
          pool.Enqueue(gameObj);
          gameObj.SetActive(false);
     }
}
