using UnityEditor;
using UnityEngine;

namespace CatImitation.Editors
{
    public class CatGraphEditior : EditorWindow
    {
        private static Texture2D _backgroundTexture;
        private static Texture2D _panelTexture;

        private float _timer;
        private bool _isConnecting;
        private bool _isDragging;
        private Vector2 _mousePosition;
        private Vector2 _previousMousePosition;
        private Vector2 _workSize = new Vector2(2000, 2000);
        private Vector2 _gridStart = new Vector2(200, 200);
        private Color _bigLines = new Color(0.25f, 0.25f, 0.25f);
        private Color _smallLines = new Color(0.30f, 0.30f, 0.30f);

        private CatGraph _catGraph;
        private CatGraph.Node _selectedNode;

        [MenuItem("Window/Cat Graph Editor")]
        private static void ShowWindow()
        {
            CatGraphEditior window = GetWindow<CatGraphEditior>();
            window.titleContent = new GUIContent("Cat Graph");
        }

        #region Unity functions
        private void Update()
        {
            if (_isConnecting)
                Repaint();
            _timer += 0.01f;
            if (_timer > 1)
            {
                _timer = 0;
                Repaint();
            }
        }

        private void OnEnable()
        {
            Undo.undoRedoPerformed += Repaint;
            SelectCatGraph();
            MakeTextures();
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= Repaint;
        }

        private void OnSelectionChange()
        {
            SelectCatGraph();
        }

        private void OnGUI()
        {
            if (_catGraph == null) return;

            //if (_backgroundTexture == false)
            //    MakeTextures();
            _catGraph.ScrollPosition = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), _catGraph.ScrollPosition, new Rect(Vector2.zero, _workSize), GUIStyle.none, GUIStyle.none);
            HandleInput();
            Handles.BeginGUI();
            PrepareWorkArea();
            DrawConnections();
            Handles.EndGUI();

            BeginWindows();
            DrawWindows();
            EndWindows();
            GUI.EndScrollView();

            BuildExtraLayout();
            EditorUtility.SetDirty(_catGraph);
        }

        #endregion

        #region Utils
        private static void MakeTextures()
        {
            _backgroundTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            _backgroundTexture.SetPixel(0, 0, new Color(0.35f, 0.35f, 0.35f));
            _backgroundTexture.Apply();

            _panelTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            _panelTexture.SetPixel(0, 0, new Color(0.65f, 0.65f, 0.65f));
            _panelTexture.Apply();
        }

        private void PrepareWorkArea()
        {
            GUI.DrawTexture(new Rect(0, 0, _workSize.x, _workSize.y), _backgroundTexture, ScaleMode.StretchToFill);

            int count = 0;
            while ((count * 10) < _workSize.x)
            {
                EditorGUI.DrawRect(new Rect(count * 10, 0, 2, _workSize.y), _smallLines);
                count++;
            }
            count = 0;
            while ((count * 10) < _workSize.y)
            {
                EditorGUI.DrawRect(new Rect(0, count * 10, _workSize.x, 2), _smallLines);
                count++;
            }

            for (int i = 0; i < _workSize.x / 100; i++)
            {
                EditorGUI.DrawRect(new Rect(i * 100, 0, 2, _workSize.y), _bigLines);
            }
            for (int i = 0; i < _workSize.y / 100; i++)
            {
                EditorGUI.DrawRect(new Rect(0, i * 100, _workSize.x, 2), _bigLines);
            }
        }

        private void ConfinePosition(ref Rect rect)
        {
            float x = Mathf.Clamp(rect.center.x, 0, _workSize.x - (rect.width));
            float y = Mathf.Clamp(rect.center.y, 0, _workSize.y - (rect.height));
            rect.center = new Vector2(x, y);
        }

        private void SelectCatGraph()
        {
            CatGraph catGraph = Selection.activeObject as CatGraph;
            if (catGraph != null)
                _catGraph = catGraph;
        }

        private void DrawNodeCurve(Rect start, Rect end, Color color)
        {
            Vector2 startPosition = new Vector2(start.x + start.width / 2, start.y + start.height);
            Vector2 endPosition = new Vector2(end.x + end.width / 2, end.y);
            Vector2 startTangent = startPosition + 50 * Vector2.up;
            Vector2 endTangent = endPosition - 50 * Vector2.up;
            Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, color, null, 2);
        }

        private void CheckNodeSelection()
        {
            _selectedNode = null;
            foreach (CatGraph.Node node in _catGraph.Nodes)
            {
                if (node.Size.Contains(_mousePosition))
                {
                    _selectedNode = node;
                    break;
                }
            }
        }
        #endregion

        #region Private methods
        private void WindowFunction(int id)
        {
            CatGraph.Node node = _catGraph.GetNode(id);
            if (node == null) return;
            float height = 15f;
            float distance = height + 2f;
            float width = node.Size.width;
            GUI.Label(new Rect(10, 2f, width, height), "Reaction description");
            node.Text = GUI.TextArea(new Rect(5, distance, width - 10, node.Size.height - 4 * distance), node.Text);
            GUI.Label(new Rect(10, node.Size.height - 3 * distance, width, height), "Method name");
            node.MethodName= GUI.TextArea(new Rect(5, node.Size.height - 2 * distance, width - 10, height), node.MethodName);
            GUI.Label(new Rect(width / 2 - 5, node.Size.height - height, width / 2, height), "o");
        }

        private void ActionWindowFunction(int id)
        {
            CatGraph.ActionWrapper wrapper = _catGraph.GetActionWrapper(id);
            if (wrapper == null) return;
            float height = 15;
            float distance = height + 2f;
            float width = 100;
            if (GUI.Button(new Rect(10, 10, width - 20, height), "Remove"))
            {
                Undo.RecordObject(_catGraph, "Remove Action");
                _catGraph.RemoveAction(wrapper);
            }
            GUI.Label(new Rect(10, 100 / 2 - distance, width, height), "Action");
            wrapper.SetAction((Action)EditorGUI.ObjectField(new Rect(5, 100 / 2, width - 10, height), wrapper.Action, typeof(Action), false));
        }

        private void MoodWindowFunction(int id)
        {
            CatGraph.MoodWrapper wrapper = _catGraph.GetMoodWrapper(id);
            if (wrapper == null) return;
            float height = 15;
            float distance = height + 2f;
            float width = 150;
            if (GUI.Button(new Rect(10, 10, width - 20, height), "Remove"))
            {
                Undo.RecordObject(_catGraph, "Remove Mood");
                _catGraph.RemoveMood(wrapper);
            }
            GUI.Label(new Rect(10, 100 / 2 - distance, width, height), "Mood");
            wrapper.SetMood((Mood)EditorGUI.ObjectField(new Rect(5, 100 / 2, width - 10, height), wrapper.Mood, typeof(Mood), false));
        }

        private void EndConnection()
        {
            CatGraph.Node startNode = _selectedNode;
            CheckNodeSelection();
            if (_selectedNode != null && _selectedNode.Action != null && _selectedNode.Action == startNode.Action)
            {
                Undo.RecordObject(_catGraph, "Establish Connection");
                startNode.SetTransition(_selectedNode);
            }
            _isConnecting = false;
        }
        #endregion

        #region Drawing
        private void BuildExtraLayout()
        {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, 50));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Action", new GUIStyle("TE toolbarbutton"), GUILayout.Width(100), GUILayout.Height(18)))
            {
                Undo.RecordObject(_catGraph, "Add New Action");
                _catGraph.AddAction();
            }
            if (GUILayout.Button("Add Mood", new GUIStyle("TE toolbarbutton"), GUILayout.Width(100), GUILayout.Height(18)))
            {
                Undo.RecordObject(_catGraph, "Add New Mood");
                _catGraph.AddMood();
            }
            //if (GUILayout.Button("Set Positions", new GUIStyle("TE toolbarbutton"), GUILayout.Width(100), GUILayout.Height(18)))
            //{
            //    Undo.RecordObject(_catGraph, "Set Positions");
            //    _catGraph.SetPositions();
            //}
            //if (GUILayout.Button("Reset", new GUIStyle("TE toolbarbutton"), GUILayout.Width(100), GUILayout.Height(18)))
            //{
            //    Undo.RecordObject(_catGraph, "Reset");
            //    _catGraph.Reset();
            //}

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawConnectorLine()
        {
            if (_isConnecting == false) return;
            Rect mouseRect = new Rect(_mousePosition.x, _mousePosition.y, 10, 10);
            Rect sourceRect = new Rect(_selectedNode.Size.x, _selectedNode.Size.y + _selectedNode.Size.height -
                EditorGUIUtility.singleLineHeight, _selectedNode.Size.width, EditorGUIUtility.singleLineHeight);
            DrawNodeCurve(sourceRect, mouseRect, Color.magenta);
            Repaint();
        }

        private void DrawConnections()
        {
            DrawConnectorLine();
            foreach (CatGraph.Node node in _catGraph.Nodes)
            {
                Rect start = node.Size;
                Rect end = node.Transition != null ? node.Transition.Size : node.Size;
                if (node.Transition != null && node != node.Transition)
                    DrawNodeCurve(start, end, Color.white);
            }
        }

        private void DrawWindows()
        {
            foreach (CatGraph.Node node in _catGraph.Nodes)
                DrawActualWindow(node, node.Size.position);
            for (int i = 0; i < _catGraph.Actions.Count; i++)
            {
                CatGraph.ActionWrapper wrapper = _catGraph.Actions[i];
                Rect rect = new Rect(_gridStart + new Vector2(-150, i * 150), new Vector2(100, 100));
                DrawActualWindow(wrapper, rect);
            }
            for (int i = 0; i < _catGraph.Moods.Count; i++)
            {
                CatGraph.MoodWrapper wrapper = _catGraph.Moods[i];
                Rect rect = new Rect(_gridStart + new Vector2(i * 200, -150), new Vector2(150, 100));
                DrawActualWindow(wrapper, rect);
            }
        }

        private void DrawActualWindow(CatGraph.Node node, Vector2 position)
        {
            GUIStyle style = node.ChooseStyle();
            node.SetPosition(position);
            GUI.Window(node.ID, node.Size, WindowFunction, string.Empty, style);
        }

        private void DrawActualWindow(CatGraph.ActionWrapper wrapper, Rect rect)
        {
            GUIStyle style = wrapper.ChooseStyle();
            GUI.Window(wrapper.ID, rect, ActionWindowFunction, string.Empty, style);
        }

        private void DrawActualWindow(CatGraph.MoodWrapper wrapper, Rect rect)
        {
            GUIStyle style = wrapper.ChooseStyle();
            GUI.Window(wrapper.ID, rect, MoodWindowFunction, string.Empty, style);
        }

        #endregion

        #region Events
        private void HandleInput()
        {
            _previousMousePosition = _mousePosition;
            _mousePosition = Event.current.mousePosition;

            bool inputClickNotConnecting = (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !_isConnecting);
            bool inputEndConnecting = (Event.current.type == EventType.MouseUp && Event.current.button == 0 && _isConnecting);
            bool inputCancelConnecting = (Event.current.type == EventType.MouseDown && Event.current.button == 1 && _isConnecting);
            var inputBeginDragging = (Event.current.type == EventType.MouseDown && Event.current.button == 2);
            var inputEndDragging = (Event.current.type == EventType.MouseUp && Event.current.button == 2 && _isDragging);

            if (inputCancelConnecting)
                _isConnecting = false;
            else if (inputEndConnecting)
                EndConnection();
            else if (inputClickNotConnecting)
                ClickOnCanvas();
            else if (inputBeginDragging)
                _isDragging = true;
            else if (inputEndDragging)
                _isDragging = false;

            if (_isDragging)
            {
                float x = _previousMousePosition.x - _mousePosition.x;
                float y = _previousMousePosition.y - _mousePosition.y;

                if (Mathf.Abs(x) > 0 || Mathf.Abs(y) > 0)
                {
                    _catGraph.ScrollPosition.x += x;
                    _catGraph.ScrollPosition.y += y;
                    Repaint();
                }
            }

            if (GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl) is TextEditor textEditor)
            {
                if (focusedWindow == this)
                {
                    if (Event.current.Equals(Event.KeyboardEvent("#x")))
                        textEditor.Cut();
                    if (Event.current.Equals(Event.KeyboardEvent("#c")))
                        textEditor.Copy();
                    if (Event.current.Equals(Event.KeyboardEvent("#v")))
                        textEditor.Paste();
                }
            }
        }

        private void ClickOnCanvas()
        {
            CheckNodeSelection();
            if (_selectedNode == null) return;
            Rect connectRect = new Rect(_selectedNode.Size.x, _selectedNode.Size.y + _selectedNode.Size.height - 15,
                _selectedNode.Size.width, 15);
            if (connectRect.Contains(_mousePosition))
                _isConnecting = true;
            Repaint();
        }

        #endregion
    }
}