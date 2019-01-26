using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "NewIntAsset", menuName = "Asset Values/Integer")]
public class IntAsset : ScriptableObject
{
     [SerializeField, BoxGroup("Settings")] private int value;
     [SerializeField, BoxGroup("Settings")] private bool constant;
     [SerializeField, BoxGroup("Settings")] private bool allowNegative;

     [SerializeField, BoxGroup("Range")] private bool useMinValue;
     [SerializeField, BoxGroup("Range"), EnableIf("useMinValue")] private int minValue;
     [SerializeField, BoxGroup("Range")] private bool useMaxValue;
     [SerializeField, BoxGroup("Range"), EnableIf("useMaxValue")] private int maxValue;

     public int Value
     {
          get { return value; }

          set
          {
               int val = value;

               if (!allowNegative && val < 0) val = 0;
               if (useMinValue && val < minValue) val = minValue;
               if (useMaxValue && val > maxValue) val = maxValue;

               if (!constant)
                    this.value = val;
          }
     }
}
