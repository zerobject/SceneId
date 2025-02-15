using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zerobject.SceneManagement.Runtime
{
    public static class SceneIdExtensions
    {
        /// <param name="index"></param>
        /// <returns>
        /// <c><see langword="true"/></c>, 
        /// ���� ������ ��������� � ��������� �� ���� �� ������� ������� 
        /// <c><see cref="IndexCollection.Indexes"/></c>, ����� <see langword="false"/>.
        /// </returns>
        public static bool Valid(this int index) => index >= 0 && index < IndexCollection.Indexes.Length;

        /// <param name="id"></param>
        /// <returns>
        /// ��� ����� �� ������ � ��������������.<br/>
        /// ������ ������, ���� ������������� ���� ����� <see cref="SceneId.Unknown"/>, ���� �������.
        /// </returns>
        public static string Name(this SceneId id)
        {
            switch (id)
            {
                case SceneId.Previous:
                    {
                        int index = SceneManager.GetActiveScene().buildIndex - 1;
                        return Valid(index) ? IndexCollection.Indexes[index].Name : string.Empty;
                    }
                case SceneId.Next:
                    {
                        int index = SceneManager.GetActiveScene().buildIndex + 1;
                        return Valid(index) ? IndexCollection.Indexes[index].Name : string.Empty;
                    }
                case SceneId.Unknown: return string.Empty;
                default: return IndexCollection.IdToName[id];
            }
        }

        /// <param name="id"></param>
        /// <returns>
        /// ������ ����� �� ��������������.<br/>
        /// -1, ���� ������������� ���� ����� <see cref="SceneId.Unknown"/>, ���� �������.
        /// </returns>
        public static int Index(this SceneId id)
        {
            switch (id)
            {
                case SceneId.Previous:
                    {
                        int index = SceneManager.GetActiveScene().buildIndex - 1;
                        return Valid(index) ? index : -1;
                    }
                case SceneId.Next:
                    {
                        int index = SceneManager.GetActiveScene().buildIndex + 1;
                        return Valid(index) ? index : -1;
                    }
                case SceneId.Unknown: return -1;
                default: return IndexCollection.IdToIndex[id];
            }
        }

        /// <param name="name"></param>
        /// <returns>
        /// ������������� � ������������� ������ �����.
        /// </returns>
        public static SceneId ToId(this string name) => IndexCollection.NameToId.GetValueOrDefault(name, SceneId.Unknown);

        /// <param name="index"></param>
        /// <returns>
        /// ������������� � ������������� �������� �����.
        /// </returns>
        public static SceneId ToId(this int index) => Valid(index) ? IndexCollection.Indexes[index].ID : SceneId.Unknown;

        public static void Load(this SceneId id, LoadSceneMode mode = LoadSceneMode.Single, bool async = false) 
            => SceneLoader.Load(id, mode, async);
        public static void Unload(this SceneId id) 
            => SceneLoader.Unload(id);

        public static bool EqualTo(this SceneId id1, SceneId id2) 
            => string.Equals(id1.Name(), id2.Name());
    }
}
