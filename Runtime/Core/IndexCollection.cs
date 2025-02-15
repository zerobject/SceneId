
/*** AUTO-GENERATED CODE ***/
/*** СГЕНЕРИРОВАННЫЙ КОД ***/

using System.Collections.Generic;

namespace Zerobject.SceneManagement.Runtime
{
    public static class IndexCollection
    {
        public static readonly (SceneId ID, string Name)[] Indexes =
        {
            (SceneId.SampleScene, "SampleScene")
        };
        public static readonly Dictionary<SceneId, string> IdToName = new();
        public static readonly Dictionary<string, SceneId> NameToId = new();
        public static readonly Dictionary<SceneId, int> IdToIndex = new();

        static IndexCollection()
        {
            int index = -3;
            foreach ((SceneId id, string name) in Indexes)
            {
                IdToName.Add(id, name);
                NameToId.Add(name, id);
                IdToIndex.Add(id, index);
                index++;
            }
        }
    }
}