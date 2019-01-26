using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "NewFloatAsset", menuName = "Asset Values/Float")]
public class FloatAsset : ScriptableObject
{
     [SerializeField, BoxGroup("Settings")] private float value;
     [SerializeField, BoxGroup("Settings")] private bool constant;
     [SerializeField, BoxGroup("Settings")] private bool allowNegative;

     [SerializeField, BoxGroup("Range")] private bool useMinValue;
     [SerializeField, BoxGroup("Range"), EnableIf("useMinValue")] private float minValue;
     [SerializeField, BoxGroup("Range")] private bool useMaxValue;
     [SerializeField, BoxGroup("Range"), EnableIf("useMaxValue")] private float maxValue;

     public float Value
     {
          get { return value; }

          set
          {
               float val = value;

               if (!allowNegative && val < 0) val = 0;
               if (useMinValue && val < minValue) val = minValue;
               if (useMaxValue && val > maxValue) val = maxValue;

               if (!constant)
                    this.value = val;
          }
     }
}
