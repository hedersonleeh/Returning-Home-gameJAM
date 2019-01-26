using UnityEngine;

[CreateAssetMenu(fileName = "NewStringAsset", menuName = "Asset Values/String")]
public class StringAsset : ScriptableObject
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     [SerializeField] private string value;
     [SerializeField] private bool constant;

     /*************************************************************************************************
     *** OnEnable
     *************************************************************************************************/
     private void OnEnable()
     {
          hideFlags = HideFlags.DontUnloadUnusedAsset;
     }

     /*************************************************************************************************
     *** Properties
     *************************************************************************************************/
     public string Value
     {
          get { return value; }

          set
          {
               if (!constant)
                    this.value = value;
          }
     }
}
