using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

//version 1.2

[CustomEditor(typeof(Transform)), CanEditMultipleObjects]
public class TransformReset : DecoratorEditor
{
     private static Vector3 resetPosition = Vector3.zero;
     private static Vector3 resetRotation = Vector3.zero;
     private static Vector3 resetScale = Vector3.one;

     private SerializedProperty p;
     private SerializedProperty r;
     private SerializedProperty s;

     public TransformReset() : base("TransformInspector")
     {
     }

     private void OnEnable()
     {
          p = serializedObject.FindProperty("m_LocalPosition");
          r = serializedObject.FindProperty("m_LocalRotation");
          s = serializedObject.FindProperty("m_LocalScale");

          if (EditorPrefs.HasKey("CustomOriginResetPosition"))
               resetPosition = StringToVector3(EditorPrefs.GetString("CustomOriginResetPosition"));
          if (EditorPrefs.HasKey("CustomOriginResetRotation"))
               resetRotation = StringToVector3(EditorPrefs.GetString("CustomOriginResetRotation"));
          if (EditorPrefs.HasKey("CustomOriginResetScale"))
               resetScale = StringToVector3(EditorPrefs.GetString("CustomOriginResetScale"));
     }

     public override void OnInspectorGUI()
     {
          /*draw default inspector*/
          base.OnInspectorGUI();

          /*add the reset position, rotation and scale buttons*/
          EditorGUILayout.BeginHorizontal();
          if (GUILayout.Button("Transform", EditorStyles.miniButton))
          {
               s.vector3Value = resetScale;
               p.vector3Value = resetPosition;
               s.vector3Value = resetScale;
               serializedObject.ApplyModifiedProperties();
               GUI.FocusControl(null);
          }
          if (GUILayout.Button("Position", EditorStyles.miniButtonLeft))
          {
               p.vector3Value = resetPosition;
               serializedObject.ApplyModifiedProperties();
               GUI.FocusControl(null);
          }
          if (GUILayout.Button("Rotation", EditorStyles.miniButtonMid))
          {
               r.quaternionValue = Quaternion.Euler(resetRotation);
               serializedObject.ApplyModifiedProperties();
               GUI.FocusControl(null);
          }
          if (GUILayout.Button("Scale", EditorStyles.miniButtonRight))
          {
               s.vector3Value = resetScale;
               serializedObject.ApplyModifiedProperties();
               GUI.FocusControl(null);
          }
          EditorGUILayout.EndHorizontal();
     }

     private Vector3 StringToVector3(string sVector)
     {
          // Remove the parentheses
          if (sVector.StartsWith("(") && sVector.EndsWith(")"))
          {
               sVector = sVector.Substring(1, sVector.Length - 2);
          }

          // split the items
          string[] sArray = sVector.Split(',');

          // store as a Vector3
          Vector3 result = new Vector3(
              float.Parse(sArray[0]),
              float.Parse(sArray[1]),
              float.Parse(sArray[2]));

          return result;
     }
}

/// <summary>
/// A base class for creating editors that decorate Unity's built-in editor types. Credits for this
/// class goes to its author Mr.Lior Tal. http://www.tallior.com/extending-unity-inspectors/
/// </summary>
public abstract class DecoratorEditor : Editor
{
     // empty array for invoking methods using reflection
     private static readonly object[] EMPTY_ARRAY = new object[0];

     #region Editor Fields

     /// <summary>
     /// Type object for the internally used (decorated) editor.
     /// </summary>
     private System.Type decoratedEditorType;

     /// <summary>
     /// Type object for the object that is edited by this editor.
     /// </summary>
     private System.Type editedObjectType;

     private Editor editorInstance;

     #endregion Editor Fields

     private static Dictionary<string, MethodInfo> decoratedMethods = new Dictionary<string, MethodInfo>();

     private static Assembly editorAssembly = Assembly.GetAssembly(typeof(Editor));

     protected Editor EditorInstance
     {
          get
          {
               if (editorInstance == null && targets != null && targets.Length > 0)
               {
                    editorInstance = Editor.CreateEditor(targets, decoratedEditorType);
               }

               if (editorInstance == null)
               {
                    Debug.LogError("Could not create editor !");
               }

               return editorInstance;
          }
     }

     public DecoratorEditor(string editorTypeName)
     {
          this.decoratedEditorType = editorAssembly.GetTypes().Where(t => t.Name == editorTypeName).FirstOrDefault();

          Init();

          // Check CustomEditor types.
          var originalEditedType = GetCustomEditorType(decoratedEditorType);

          if (originalEditedType != editedObjectType)
          {
               throw new System.ArgumentException(
                   string.Format("Type {0} does not match the editor {1} type {2}",
                             editedObjectType, editorTypeName, originalEditedType));
          }
     }

     private System.Type GetCustomEditorType(System.Type type)
     {
          var flags = BindingFlags.NonPublic | BindingFlags.Instance;

          var attributes = type.GetCustomAttributes(typeof(CustomEditor), true) as CustomEditor[];
          var field = attributes.Select(editor => editor.GetType().GetField("m_InspectedType", flags)).First();

          return field.GetValue(attributes[0]) as System.Type;
     }

     private void Init()
     {
          var flags = BindingFlags.NonPublic | BindingFlags.Instance;

          var attributes = this.GetType().GetCustomAttributes(typeof(CustomEditor), true) as CustomEditor[];
          var field = attributes.Select(editor => editor.GetType().GetField("m_InspectedType", flags)).First();

          editedObjectType = field.GetValue(attributes[0]) as System.Type;
     }

     private void OnDisable()
     {
          if (editorInstance != null)
          {
               DestroyImmediate(editorInstance);
          }
     }

     /// <summary>
     /// Delegates a method call with the given name to the decorated editor instance.
     /// </summary>
     protected void CallInspectorMethod(string methodName)
     {
          MethodInfo method = null;

          // Add MethodInfo to cache
          if (!decoratedMethods.ContainsKey(methodName))
          {
               var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

               method = decoratedEditorType.GetMethod(methodName, flags);

               if (method != null)
               {
                    decoratedMethods[methodName] = method;
               }
               else
               {
                    Debug.LogError(string.Format("Could not find method {0}", methodName));
               }
          }
          else
          {
               method = decoratedMethods[methodName];
          }

          if (method != null)
          {
               method.Invoke(EditorInstance, EMPTY_ARRAY);
          }
     }

     //public void OnSceneGUI()
     //{
     //    CallInspectorMethod("OnSceneGUI");
     //}

     protected override void OnHeaderGUI()
     {
          CallInspectorMethod("OnHeaderGUI");
     }

     public override void OnInspectorGUI()
     {
          EditorInstance.OnInspectorGUI();
     }

     public override void DrawPreview(Rect previewArea)
     {
          EditorInstance.DrawPreview(previewArea);
     }

     public override string GetInfoString()
     {
          return EditorInstance.GetInfoString();
     }

     public override GUIContent GetPreviewTitle()
     {
          return EditorInstance.GetPreviewTitle();
     }

     public override bool HasPreviewGUI()
     {
          return EditorInstance.HasPreviewGUI();
     }

     public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
     {
          EditorInstance.OnInteractivePreviewGUI(r, background);
     }

     public override void OnPreviewGUI(Rect r, GUIStyle background)
     {
          EditorInstance.OnPreviewGUI(r, background);
     }

     public override void OnPreviewSettings()
     {
          EditorInstance.OnPreviewSettings();
     }

     public override void ReloadPreviewInstances()
     {
          EditorInstance.ReloadPreviewInstances();
     }

     public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
     {
          return EditorInstance.RenderStaticPreview(assetPath, subAssets, width, height);
     }

     public override bool RequiresConstantRepaint()
     {
          return EditorInstance.RequiresConstantRepaint();
     }

     public override bool UseDefaultMargins()
     {
          return EditorInstance.UseDefaultMargins();
     }
}
