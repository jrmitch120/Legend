using System.Collections.Generic;
using Legend.Models;

namespace Legend.Services
{
    public interface IWorldService
    {
        Player GetPlayerByName(string name);

        T GetGameObjectById<T>(string id) where T : IGameObject;
        IEnumerable<T> GetGameObjectsById<T>(IEnumerable<string> objIds) where T : IGameObject;

        void Initialize();
        void Save(IGameObject gameObj);
        void Save(IEnumerable<IGameObject> gameObjs);

    }
}