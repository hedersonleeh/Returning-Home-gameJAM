using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "NewBoolAsset", menuName = "Asset Values/Bool")]
public class BoolAsset : ScriptableObject
{
     [SerializeField, BoxGroup] private bool value;
     [SerializeField, BoxGroup] private bool constant;
     
     public bool Value
     {
          get { return value; }

          set
          {
               if (!constant)
                    this.value = value;
          }
     }
}
