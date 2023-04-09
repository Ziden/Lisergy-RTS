using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Assets.Code.Editor
{
    public class TexturePostprocessor : AssetPostprocessor
    {
        private const int MaxTextureSize = 256;
        private const int CompressionQuality = 50;
        private const string TexturesFolderPath = "Assets/Packs";

        void OnPostprocessTexture(Texture2D texture)
        {
            if (!assetPath.StartsWith(TexturesFolderPath)) return;

            if (assetPath.Contains("Ui")) return;
            TextureImporter importer = (TextureImporter)assetImporter;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.compressionQuality = CompressionQuality;
            importer.maxTextureSize = MaxTextureSize;
            importer.mipmapEnabled = false;
            importer.isReadable = false;
        }
    }

}
