using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioPlayer))]
public class AudioPlayerEditor : UnityEditor.Editor
{
        SerializedProperty collectionsProp;
        SerializedProperty collectionIndexProp;
        SerializedProperty clipIndexProp;

        void OnEnable()
        {
            collectionsProp = serializedObject.FindProperty("collections");
            collectionIndexProp = serializedObject.FindProperty("collectionIndex");
            clipIndexProp = serializedObject.FindProperty("clipIndex");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(collectionsProp, true);

            // Draw collection selector popup and clip selector for the chosen collection
            var player = (AudioPlayer)target;
            int selectedCollection = Mathf.Clamp(collectionIndexProp.intValue, 0, Mathf.Max(0, collectionsProp.arraySize - 1));

            string[] collectionOptions;
            if (collectionsProp.arraySize == 0)
            {
                collectionOptions = new[] { "(no collections)" };
                selectedCollection = 0;
            }
            else
            {
                collectionOptions = new string[collectionsProp.arraySize];
                for (int i = 0; i < collectionsProp.arraySize; i++)
                {
                    var elem = collectionsProp.GetArrayElementAtIndex(i).objectReferenceValue as AudioCollection;
                    collectionOptions[i] = (elem != null) ? elem.name : "(null)";
                }
            }

            EditorGUI.BeginChangeCheck();
            selectedCollection = EditorGUILayout.Popup("Selected Collection", selectedCollection, collectionOptions);
            if (EditorGUI.EndChangeCheck())
            {
                collectionIndexProp.intValue = selectedCollection;
            }

            // Clip selector for the chosen collection
            int clipIndex = clipIndexProp.intValue;
            string[] clipOptions = new[] { "(no clips)" };
            if (player != null && player.collections != null && player.collections.Count > 0)
            {
                var col = player.collections[Mathf.Clamp(collectionIndexProp.intValue, 0, player.collections.Count - 1)];
                if (col != null && col.ClipCollection != null && col.ClipCollection.Count > 0)
                {
                    clipOptions = new string[col.ClipCollection.Count];
                    for (int i = 0; i < col.ClipCollection.Count; i++)
                    {
                        var clip = col.ClipCollection[i];
                        clipOptions[i] = (clip != null) ? clip.name : "(null clip)";
                    }
                    clipIndex = Mathf.Clamp(clipIndex, 0, clipOptions.Length - 1);
                    EditorGUI.BeginChangeCheck();
                    clipIndex = EditorGUILayout.Popup("Selected Clip", clipIndex, clipOptions);
                    if (EditorGUI.EndChangeCheck())
                    {
                        clipIndexProp.intValue = clipIndex;
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Selected Clip", "(no clips in collection)");
                    EditorGUILayout.PropertyField(clipIndexProp);
                }
            }
            else
            {
                EditorGUILayout.LabelField("Selected Clip", "(no collection selected)");
                EditorGUILayout.PropertyField(clipIndexProp);
            }

            // Field where user can type a value and apply it
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Clip Index (apply in Play Mode)"))
            {
                // Apply typed value; will play if in play mode (AudioPlayer handles Application.isPlaying)
                player.SetClipIndex(clipIndexProp.intValue);
                // reflect any changes
                EditorUtility.SetDirty(player);
            }
            if (GUILayout.Button("Play Selected"))
            {
                player.PlaySelected();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Play control buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Pause"))
            {
                player.Pause();
            }
            if (GUILayout.Button("Resume"))
            {
                player.Resume();
            }
            if (GUILayout.Button("Stop"))
            {
                player.Stop();
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }



