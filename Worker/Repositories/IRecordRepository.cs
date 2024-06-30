using Worker.Models;

namespace Worker.Repositories
{
    public interface IRecordRepository
    {
        void Add(Record record);
        Record Get(Guid id);
        IEnumerable<Record> GetAll();
    }
}
