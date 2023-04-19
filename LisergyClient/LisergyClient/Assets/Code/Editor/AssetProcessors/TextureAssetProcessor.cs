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
        private const int MaxTextureSize = 512;
        private const int CompressionQuality = 50;
        private const string TexturesFolderPath = "Assets/Packs";
        private const string UiArt = "Assets/Packs/Ui";

        /*
        void OnPostprocessTexture(Texture2D texture)
        {
            if (!assetPath.StartsWith(TexturesFolderPath)) return;

            TextureImporter importer = (TextureImporter)assetImporter;

            if (assetPath.Contains(UiArt))
            {
                importer.alphaIsTransparency = true;
                importer.alphaSource = TextureImporterAlphaSource.FromInput;
                importer.maxTextureSize = 1024;
               
            } else
            {
                importer.maxTextureSize = MaxTextureSize;
            }
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.compressionQuality = CompressionQuality;  
            importer.mipmapEnabled = false;
            importer.isReadable = false;
        }
        */
    }

}
