using System;
using System.Collections.Generic;
using UnityEngine;

namespace VisualFX
{
    public enum VFXMaterial
    {
        DEFAULT = 0,
        HIGHLIGHT = 1
    }

    public class VFXMaterialLibrary : MonoBehaviour
    {
        [Serializable]
        private class VfxMaterialData
        {
            [SerializeField]
            internal string name;
            public VFXMaterial type;
            public Material material;
        }

        internal static VFXMaterialLibrary Instance;
        
        [SerializeField]
        private VfxMaterialData[] vfxMatDatas;

        private Dictionary<VFXMaterial, VfxMaterialData> _vfxMaterialDictionary;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        // Start is called before the first frame update
        private void Start()
        {
            var count = vfxMatDatas.Length;
            _vfxMaterialDictionary = new Dictionary<VFXMaterial, VfxMaterialData>(count);
            for (var i = 0; i < count; i++)
            {
                var vfxMat = vfxMatDatas[i];
                _vfxMaterialDictionary.Add(vfxMat.type, vfxMat);
            }
        }

        public static Material GetMaterial(VFXMaterial material)
        {
            return VFXMaterialLibrary.Instance._vfxMaterialDictionary[material].material;
        }

    }

}