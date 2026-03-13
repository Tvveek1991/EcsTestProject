using System.Collections.Generic;
using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Services.PersonService
{
    public class PersonViewService : IPersonViewService
    {
        private readonly Dictionary<int, PersonView> m_personViews = new();

        public void AddPerson(int entity, PersonView view) => m_personViews.Add(entity, view);
        public PersonView GetPersonViewByEntity(int entity) => m_personViews[entity];
        public void RemoveView(int entity) => m_personViews.Remove(entity);
        public void Clear() => m_personViews.Clear();
    }
}
