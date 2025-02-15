using System;
using UnityEditor;

namespace Zerobject.SceneManagement.Editor
{
    [Serializable]
    public class SceneEntry
    {
        public SceneAsset Asset;
        public string Name;
        public int Value;

        public SceneEntry(SceneAsset asset, string name, int value)
        {
            Asset = asset;
            Name = name;
            Value = value;
        }
    }
}
