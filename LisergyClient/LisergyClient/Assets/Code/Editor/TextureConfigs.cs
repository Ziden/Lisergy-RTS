using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Code.Editor
{
    /*
    public class CompressTextures
    {
        [MenuItem("Lisergy/Compress Textures")]
        static void Execute()
        {
            if (Selection.activeObject == null) return;

            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            Debug.Log(path);

            string[] files = Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly);

        }

        private void Compress(string path)
        {
            Debug.Log("Compressing " + path);
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.mipmapEnabled = false;
            importer.maxTextureSize = 512;
            importer.textureCompression = TextureImporterCompression.CompressedLQ;
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
    */
}
