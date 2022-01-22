using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.Events;

[CustomPropertyDrawer(typeof(ActionEvent), true)]
public class ActionEventDrawer : PropertyDrawer
{
    private float lineHeight = 18;

    private float eventHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var actionType = property.FindPropertyRelative("action");
        var eventToTrigger = property.FindPropertyRelative("eventToTrigger");

        int propNum = 0;

        if (actionType.intValue < 3)
        {
            propNum = 10;
            eventHeight = 0;
        }
        else
        {
            propNum = 5;
            eventHeight = EditorGUI.GetPropertyHeight(eventToTrigger, label, true);
        }

        if (property.FindPropertyRelative("moveBy").boolValue)
            propNum++;

        return lineHeight * propNum + 6 + eventHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //EditorGUI.LabelField(position, label);

        EditorGUI.BeginProperty(position, label, property);

        var actionType = property.FindPropertyRelative("action");

        EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight * 0, position.width, 16),
            actionType);

        EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight * 1, position.width, 16),
            property.FindPropertyRelative("preDelay"));

        EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight * 2, position.width, 16), 
            property.FindPropertyRelative("postDelay"));

        if(actionType.intValue < 3)
        {
            //var objType = property.FindPropertyRelative("objectType");
            //EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight * 0, position.width, 16),
            //    objType);

            //if((ActionObjectType)objType.intValue == ActionObjectType.SpecificTransform)
                EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight * 3, position.width, 16),
                property.FindPropertyRelative("_object"));
            //else if ((ActionObjectType)objType.intValue == ActionObjectType.FindWithTag)
            //    EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight * 3, position.width, 16),
            //    property.FindPropertyRelative("_objectTag"));
        }

        var eventToTrigger = property.FindPropertyRelative("eventToTrigger");


        var moveBy = property.FindPropertyRelative("moveBy");

        switch ((ActionType)actionType.intValue)
        {
            case ActionType.Move:

                EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight * 4, position.width, 16), moveBy);

                if (moveBy.boolValue)
                {
                    EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight * 5, position.width, 16),
                            property.FindPropertyRelative("moveByVector"));
                }
                else
                {
                    EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight * 5, position.width, 16),
                        property.FindPropertyRelative("targetPosition"));
                }
                break;
            case ActionType.Rotate:
                EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight * 4, position.width, 16), property.FindPropertyRelative("targetRotation"));
                break;
            case ActionType.Scale:
                EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight * 4, position.width, 16), property.FindPropertyRelative("targetScale"));
                break;
            case ActionType.Event:
                EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight * 4, position.width, 16), eventToTrigger);              
                break;
        }

        int lines = 6;

        if (moveBy.boolValue)
            lines++;

        if (actionType.intValue < 3)
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight * lines, position.width, 16), property.FindPropertyRelative("movementCurve"));
            EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight * (lines + 1), position.width, 16), property.FindPropertyRelative("local"));

            var useSpeed = property.FindPropertyRelative("useSpeed");

            EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight * (lines + 2), position.width, 16), useSpeed);

            if (useSpeed.boolValue)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight * (lines + 3), position.width, 16), property.FindPropertyRelative("speed"));
            }
            else
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight * (lines + 3), position.width, 16), property.FindPropertyRelative("duration"));
            }
        } 

        EditorGUI.EndProperty();
    }

}
