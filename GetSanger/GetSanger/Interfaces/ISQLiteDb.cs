using SQLite;

namespace GetSanger.Interfaces
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}
