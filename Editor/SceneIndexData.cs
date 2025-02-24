using System.Collections.Generic;
using UnityEngine;

namespace Zerobject.SceneManagement.Editor
{
    [CreateAssetMenu(menuName = "Project/Editor/Scene Management/Scene Index Data")]
    public class SceneIndexData : ScriptableObject
    {
        public List<SceneEntry> Entries = new();
    }
}
