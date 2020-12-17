namespace DataIdentity.Repository
{
    public interface IBaseContext
    {
        bool EvenEventExists(int id, string UserId);
        bool EventCount(int id);
        bool EventCountMore(int id);
        bool EventExists(int id);
        bool EventOrg(string UserId, int id);
        bool FriendsExist(string id);
        bool FriendsExistApp(string id);
        bool UserEventExists(string id);
    }
}