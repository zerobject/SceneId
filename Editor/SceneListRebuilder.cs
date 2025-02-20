using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Zerobject.SceneManagement.Editor
{
    public static class SceneListRebuilder
    {
        private const string SceneIdClassTemplate = @"
/*** AUTO-GENERATED CODE ***/
/*** ÑÃÅÍÅÐÈÐÎÂÀÍÍÛÉ ÊÎÄ ***/

namespace Zerobject.SceneManagement.Runtime
{
    public enum SceneId
    {
        Previous = -2,
        Next = 1,
        Unknown = 0,
        IDS
    }
}";
        private const string IndexCollectionClassTemplate = @"
/*** AUTO-GENERATED CODE ***/
/*** ÑÃÅÍÅÐÈÐÎÂÀÍÍÛÉ ÊÎÄ ***/

using System.Collections.Generic;

namespace Zerobject.SceneManagement.Runtime
{
    public static class IndexCollection
    {
        public static readonly (SceneId ID, string Name)[] Indexes =
        {
            INDEXES
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
}";

        private const string IdDummy = "IDS";
        private const string IndexDummy = "INDEXES";

        private const string IndexDataClassName = nameof(SceneIndexData);
        private const string IndexDataClassPath = "Assets/Editor/Resources";

        private const string SceneIdClassGuid = "d1d68175920019d45aea88dd475e433b";
        private const string IndexCollectionClassGuid = "a3f1a1f7a8b474d4abccdc453725c85d";

        private static SceneIndexData LoadOrCreateIndexData()
        {
            SceneIndexData data = Resources.Load<SceneIndexData>(IndexDataClassName);

            if (data == null)
            {
                data = ScriptableObject.CreateInstance<SceneIndexData>();

                if (!Directory.Exists(IndexDataClassPath))
                {
                    Directory.CreateDirectory(IndexDataClassPath);
                    AssetDatabase.ImportAsset(IndexDataClassPath);
                }

                AssetDatabase.CreateAsset(data, $"{IndexDataClassPath}/{IndexDataClassName}.asset");
                AssetDatabase.SaveAssets();
            }

            return data;
        }

        public static string ConvertToPascalCase(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Èìÿ ñöåíû íå äîëæíî áûòü ïóñòûì.", nameof(name));

            name = Regex.Replace(name, @"\s*\(.*?\)", "");
            name = Regex.Replace(name, @"[^a-zA-Z0-9 ]", "");

            StringBuilder output = new();
            foreach (var c in name)
            {
                output.Append(char.IsWhiteSpace(c) ? char.ToUpper(c) : c);
            }

            return $"{output}";
        }

        [MenuItem("Tools/Zerobject/Scene Management/Rebuild Indexes")]
        public static void RebuildIndexes()
        {
            var indexData = LoadOrCreateIndexData();
            var sceneEntryDict = indexData.Entries.ToDictionary(entry => entry.Asset, entry => entry);
            var scenesInBuild = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path))
                .ToList();

            HashSet<SceneAsset> outputSceneList = scenesInBuild.Concat(sceneEntryDict.Keys).ToHashSet();
            List<SceneEntry> sceneEntriesList = new();
            HashSet<string> sceneIdEnumValues = new();
            int id = sceneEntryDict.Values.Select(e1 => e1.Value).DefaultIfEmpty(0).Max() + 1;

            foreach (var sceneAsset in outputSceneList)
            {
                if (!sceneEntryDict.TryGetValue(sceneAsset, out var entry))
                {
                    var enumEntry = ConvertToPascalCase(sceneAsset.name);
                    entry = new(sceneAsset, enumEntry, id);
                    sceneEntryDict.Add(sceneAsset, entry);
                    id++;
                }
                if (sceneIdEnumValues.Contains(entry.Name))
                {
                    throw new Exception("Èíäåêñ èñïîëüçóåòñÿ íåñêîëüêèìè ñöåíàìè.");
                }

                sceneIdEnumValues.Add(entry.Name);
                sceneEntriesList.Add(entry);
            }

            string sceneIdClassPath = AssetDatabase.GUIDToAssetPath(SceneIdClassGuid);
            string indexCollectionClassPath = AssetDatabase.GUIDToAssetPath(IndexCollectionClassGuid);

            if (string.IsNullOrEmpty(sceneIdClassPath))
                throw new Exception();
            if (string.IsNullOrEmpty(indexCollectionClassPath))
                throw new Exception();

            sceneEntriesList.Sort((e1, e2) => e1.Value.CompareTo(e2.Value));

            string idEntriesReadyToInclude = string.Join("\n", sceneEntriesList.Select(SelectSceneIdEntry));
            string indexEntriesReadyToInclude = string.Join(",\n", scenesInBuild.Select(SelectIndexEntry));
            string updatedSceneIdClass = SceneIdClassTemplate.Replace(IdDummy, idEntriesReadyToInclude);
            string updatedIndexClass = IndexCollectionClassTemplate.Replace(IndexDummy, indexEntriesReadyToInclude);
            File.WriteAllText(sceneIdClassPath, updatedSceneIdClass);
            File.WriteAllText(indexCollectionClassPath, updatedIndexClass);
            AssetDatabase.Refresh();

            indexData.Entries = sceneEntriesList;
            EditorUtility.SetDirty(indexData);
            AssetDatabase.SaveAssets();
            Debug.Log("Èíäåêñû îáíîâëåíû.");
            return;

            string SelectSceneIdEntry(SceneEntry scene)
            {
                string sceneName = scene.Asset.name;
                string enumName = ConvertToPascalCase(sceneName);
                if (!string.Equals(enumName, scene.Name))
                {
                    if (EditorUtility.DisplayDialog(
                        "Äðóãîå èìÿ ñöåíû",
                        $"Èìÿ ñöåíû {sceneName} áûëî èçìåíåíî, âûáåðèòå âàðèàíò äëÿ âêëþ÷åíèÿ â ñïèñîê èäåíòèôèêàòîðîâ.",
                        "Íîâîå", "Ñòàðîå")) scene.Name = enumName;
                }
                return $"{scene.Name} = {scene.Value},";
            }
            string SelectIndexEntry(SceneAsset asset)
            {
                var info = sceneEntryDict[asset];
                var sceneName = asset.name;
                return $"(SceneId.{info.Name}, \"{sceneName}\")";
            }
        }

    }
}
