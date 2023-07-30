﻿namespace UniGame.Rendering.Editor.Utils
{
    using UnityEditor;
    using UnityEngine;

    public static class MeshSaverEditor
    {
        [MenuItem("CONTEXT/MeshFilter/Save Mesh...")]
        public static void SaveMeshInPlace(MenuCommand menuCommand) 
        {
            var mf = menuCommand.context as MeshFilter;
            var m  = mf.sharedMesh;
            SaveMesh(m, m.name, false, true);
        }

        [MenuItem("CONTEXT/MeshFilter/Save Mesh As New Instance...")]
        public static void SaveMeshNewInstanceItem(MenuCommand menuCommand) 
        {
            var mf = menuCommand.context as MeshFilter;
            var m  = mf.sharedMesh;
            SaveMesh(m, m.name, true, true);
        }

        public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh) 
        {
            var path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
            if (string.IsNullOrEmpty(path)) return;
        
            path = FileUtil.GetProjectRelativePath(path);

            var meshToSave = makeNewInstance ? Object.Instantiate(mesh) : mesh;
		
            if (optimizeMesh)
                MeshUtility.Optimize(meshToSave);
        
            AssetDatabase.CreateAsset(meshToSave, path);
            AssetDatabase.SaveAssets();
        }
    }
}