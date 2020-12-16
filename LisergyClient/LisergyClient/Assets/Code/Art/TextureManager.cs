using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Art
{
    public class TextureManager
    {
        private static Dictionary<string, Material> _materials = new Dictionary<string, Material>();

        public static void SetTexture(GameObject obj, string name)
        {
            var mesh = obj.GetComponent<MeshRenderer>();
            if(mesh == null)
            {
                throw new System.Exception("Objects w/o a mesh renderer cannot have textures");
            }
            Material mat;
            if(_materials.TryGetValue(name, out mat))
            {
                mesh.material = mat;
                return;
            }
            mat = new Material(Shader.Find("Diffuse")); ;
            mat.mainTexture = GetBlockTexture(name);
            mesh.material = mat;
            _materials[name] = mat;
        }

        public static Texture2D GetBlockTexture(string name)
        {
            var tex = Resources.Load<Texture2D>("blocks/"+name);
            if (tex == null)
                throw new System.Exception("Texture " + name + ".png not found in /Resources/blocks/");
            return tex;
        }
    }
}
