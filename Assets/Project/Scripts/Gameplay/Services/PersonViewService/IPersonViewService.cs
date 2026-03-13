using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Services.PersonService
{
    public interface IPersonViewService
    {
        void AddPerson(int entity, PersonView view);
        PersonView GetPersonViewByEntity(int entity);
        void RemoveView(int entity);
        void Clear();
    }
}
