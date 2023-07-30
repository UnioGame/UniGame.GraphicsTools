namespace Taktika.Rendering.Editor.Water.Mesh
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstract;
    using Runtime.Water;
    using Sirenix.Utilities.Editor;
    using UnityEngine;

    public class MeshEditor
    {
        private readonly Water  _water;
        private readonly Action _repaintAction;
        
        private readonly List<MeshManipulator> _registeredManipulators = new List<MeshManipulator>();
        
        private int  _selectedVertexIndex;

        public MeshEditor(Water water, Action repaintAction = null)
        {
            _water              = water;
            _repaintAction = repaintAction;
        }

        public bool DrawMeshPanel()
        {
            SirenixEditorGUI.BeginBox(new GUIContent("Mesh Editor"));
            {
                if (_water.WaterMesh.IsNull)
                {
                    if (GUILayout.Button("Create"))
                    {
                        _water.WaterMesh = WaterMesh.GetDefault();
                    }
                }
                else
                {
                    foreach (var manipulator in _registeredManipulators)
                    {
                        manipulator.DrawInspector();
                    }
                }
            }
            SirenixEditorGUI.EndBox();

            return _registeredManipulators.Any(x=>x.IsEnabled && x.CanChangeEnabledStatus);
        }

        public void HandleMeshManipulators()
        {
            if(_water.WaterMesh.IsNull)
                return;
            
            foreach (var manipulator in _registeredManipulators.Where(x=>x.IsEnabled))
            {
                var index = manipulator.Handle(_water.WaterMesh, _selectedVertexIndex);
                if (index != _selectedVertexIndex)
                {
                    _selectedVertexIndex = index;
                    _repaintAction?.Invoke();
                }
            }
        }

        public void RegisterManipulator(MeshManipulator manipulator)
        {
            if(_registeredManipulators.Contains(manipulator))
                return;
            
            _registeredManipulators.Add(manipulator);
        }

        public void UnRegisterManipulator(MeshManipulator manipulator)
        {
            if(!_registeredManipulators.Contains(manipulator))
                return;

            _registeredManipulators.Remove(manipulator);
        }

        public void ReleaseEditing()
        {
            foreach (var meshManipulator in _registeredManipulators.Where(x=>x.IsEnabled && x.CanChangeEnabledStatus))
            {
                meshManipulator.Disable();
            }
        }
    }
}