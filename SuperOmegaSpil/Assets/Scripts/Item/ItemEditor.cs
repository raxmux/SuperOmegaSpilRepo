
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    //----this script was made with AI---- (sorry guys i gave up<3)


    public override void OnInspectorGUI()
    {
        // Update the serialized data before drawing the Inspector.
        serializedObject.Update();

        // Draw every Item field except the script reference and Trigger.
        DrawPropertiesExcluding(
            serializedObject, "m_Script", "trigger"
        );

        // Find the serialized Trigger field.
        SerializedProperty trigger =
            serializedObject.FindProperty("trigger");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(
            "Trigger", EditorStyles.boldLabel
        );

        // Draw the TriggerType enum dropdown.
        EditorGUILayout.PropertyField(
            trigger.FindPropertyRelative("triggerType")
        );

        // Find the list of TriggerActions.
        SerializedProperty actions =
            trigger.FindPropertyRelative("triggerAction");

        EditorGUILayout.LabelField(
            "Trigger Actions:", EditorStyles.label
        );

        // Draw every action in the list.
        for (int i = 0; i < actions.arraySize; i++)
        {
            SerializedProperty element =
                actions.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical(
                EditorStyles.helpBox
            );

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(
                "Action " + (i + 1),
                EditorStyles.boldLabel
            );

            bool remove = GUILayout.Button(
                "Remove", GUILayout.Width(70)
            );

            EditorGUILayout.EndHorizontal();

            if (!remove)
            {
                // If no action has been assigned, offer a type selector.
                if (element.managedReferenceValue == null)
                {
                    if (GUILayout.Button("Choose Action Type"))
                        ShowActionMenu(i);
                }
                else
                {
                    // Display the selected action's serialized fields.
                    EditorGUILayout.PropertyField(
                        element, GUIContent.none, true
                    );

                    if (GUILayout.Button("Change Action Type"))
                        ShowActionMenu(i);
                }
            }

            EditorGUILayout.EndVertical();

            // Remove the selected action if requested.
            if (remove)
            {
                int oldSize = actions.arraySize;
                actions.DeleteArrayElementAtIndex(i);

                // Some reference elements require a second deletion.
                if (actions.arraySize == oldSize)
                    actions.DeleteArrayElementAtIndex(i);

                // Stop because the list indices may have changed.
                break;
            }
        }

        // Button for adding a new action.
        if (GUILayout.Button("Add Action"))
            ShowActionMenu(-1);

        // Apply the Inspector changes to the Item asset.
        serializedObject.ApplyModifiedProperties();
    }

    private void ShowActionMenu(int index)
    {
        GenericMenu menu = new GenericMenu();
        bool foundAction = false;

        // Find all subclasses of TriggerActions in the project.
        foreach (Type type in
                 TypeCache.GetTypesDerivedFrom<TriggerActions>())
        {
            // Only allow concrete, serializable classes.
            if (type.IsAbstract ||
                !type.IsClass ||
                type.ContainsGenericParameters ||
                !Attribute.IsDefined(
                    type, typeof(SerializableAttribute)))
                continue;

            foundAction = true;

            // Create a menu entry for the discovered action type.
            menu.AddItem(
                new GUIContent(
                    ObjectNames.NicifyVariableName(type.Name)
                ),
                false,
                () => CreateAction(index, type)
            );
        }

        // Handle the case where no valid action types exist.
        if (!foundAction)
        {
            menu.AddDisabledItem(
                new GUIContent("No action classes found")
            );
        }

        menu.ShowAsContext();
    }

    private void CreateAction(int index, Type type)
    {
        serializedObject.Update();

        SerializedProperty trigger =
            serializedObject.FindProperty("trigger");

        SerializedProperty actions =
            trigger.FindPropertyRelative("triggerAction");

        // Index -1 means add a new element.
        if (index < 0)
        {
            index = actions.arraySize;
            actions.arraySize++;
        }

        // Create an instance of the selected action class.
        actions.GetArrayElementAtIndex(index)
            .managedReferenceValue =
                Activator.CreateInstance(type);

        serializedObject.ApplyModifiedProperties();
    }
}
