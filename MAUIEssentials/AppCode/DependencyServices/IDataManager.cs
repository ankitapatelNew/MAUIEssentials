using SQLite;

namespace MAUIEssentials.AppCode.DependencyServices
{
    public interface IDataManager
    {
        SQLiteConnection GetConnection();
    }
}