using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace OnceNull.FSM.Editor
{
    [CustomEditor(typeof(StateActionTimeInState), true)]
    public class StateActionTimeInStateEditor : OdinEditor
    {
        private const float TimelineHeight = 60f;
        private const float MarkerRadius = 4f;
        private const float MarkerHeight = 18f;
        private const float MarkerWidth = 6f;
        private const float LabelPaddingTop = 6f;
        private const float CurrentTimeHeight = 4f;
        private const float HoverDistance = 6;

        private StateActionTimeInState _target;
        private GUIStyle _labelStyle;
        private Vector2 _mousePos;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (target == null) return;
            _target = (StateActionTimeInState)target;
        }

        public override void OnInspectorGUI()
        {
            DrawTimeline();
            GUILayout.Space(10); // Make room for timeline

            base.OnInspectorGUI(); // Let Odin draw the rest
        }

        private void DrawTimeline()
        {
            // Always reserve space
            var rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(TimelineHeight));

            // Only draw visuals during repaint or mouse move
            if (Event.current.type != EventType.Repaint)
                return;

            // Lazy style init
            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle(EditorStyles.label)
                {
                    alignment = TextAnchor.UpperCenter,
                    fontSize = 12,
                    normal = { textColor = Color.black }
                };
            }

            // Background
            EditorGUI.DrawRect(rect, Color.clear);

            // Timeline line
            Handles.color = Color.black;
            float left = rect.xMin + 10;
            float right = rect.xMax - 10;
            float midY = rect.center.y;
            Handles.DrawLine(new Vector2(left, midY), new Vector2(right, midY));

            if (_target.EndTime <= 0) return;

            
            if (Event.current.mousePosition.y <= TimelineHeight)
                _mousePos = Event.current.mousePosition;
            List<string> hoverTexts = new();
            float mouseX = _mousePos.x;
            float t2 = Mathf.InverseLerp(left, right, mouseX);
            
            // draw text for start time, end time and current time on top of the current timeline
            float startTimeX = Mathf.Lerp(left, right, 0);
            float endTimeX = Mathf.Lerp(left, right, 1);
            float currentTimeX = Mathf.Lerp(0, right, t2);
            float currentTime =  Mathf.Lerp(0, _target.EndTime, t2);
            Handles.Label(new Vector2(startTimeX, LabelPaddingTop), "0", _labelStyle);
            Handles.Label(new Vector2(endTimeX, LabelPaddingTop), _target.EndTime.ToString("F2"), _labelStyle);
            Handles.Label(new Vector2(currentTimeX - 15, midY + 10), currentTime.ToString("F2"), _labelStyle);

            foreach (var ts in _target.TimeStamps)
            {
                Color handleColor = ts.Time < 0 || ts.Time > _target.EndTime ? Color.red : Color.black;
                
                float t = Mathf.Clamp01(ts.Time / _target.EndTime);
                float x = Mathf.Lerp(left, right, t);

                Vector2 markerPos = new Vector2(x, midY);
                // Handles.DrawSolidDisc(markerPos, Vector3.forward, MarkerRadius);
                
                // Draw the marker as a rectangle
                Rect markerRect = new Rect(x - MarkerWidth / 2, midY - MarkerHeight / 2, MarkerWidth, MarkerHeight);
                EditorGUI.DrawRect(markerRect, handleColor);
                
                if (Mathf.Abs(_mousePos.x - x) < HoverDistance)
                {
                    hoverTexts.Add(ts.Tag);
                }
            }

            if (hoverTexts.Count > 0)
            {
                var labelRect = new Rect(rect.x, LabelPaddingTop, rect.width, 20f);
                EditorGUI.LabelField(labelRect, string.Join('/', hoverTexts), _labelStyle);
            }
            
            // // debug mouse position
            // var mousePosRect = new Rect(rect.x, midY + 10, rect.width, 20f);
            // EditorGUI.LabelField(mousePosRect, _mousePos.ToString(), _labelStyle);
            
            // draw current timeline that the mouse is hovering
            float currentLineX = Mathf.Lerp(left, right, t2);
            
            Handles.color = Color.red;
            Handles.DrawLine(
                new Vector2(currentLineX, midY - MarkerHeight/2), 
                new Vector2(currentLineX, midY + MarkerHeight/2)
                );
            
            // draw a line with width of 2 to represent the current timeline
            //  Handles.DrawLine(new Vector2(left, midY), new Vector2(right, midY));
            float xLeft = left;
            float xRight = Mathf.InverseLerp(0, _target.EndTime, _target.TimeInState);
            float currentLineX2 = Mathf.Lerp(left, right, xRight);
            float currentLineWidth = Mathf.Lerp(left, right, xRight) - left;
            Rect currentLineRect = new Rect(xLeft, midY - CurrentTimeHeight/2, currentLineWidth, CurrentTimeHeight);
            EditorGUI.DrawRect(currentLineRect, Color.cyan);
        }
    }
}