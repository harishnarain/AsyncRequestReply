using System.Collections.Concurrent;
using Worker.Models;

namespace Worker.Repositories
{
    public class InMemoryRecordRepository : IRecordRepository
    {
        private readonly ConcurrentDictionary<Guid, Record> _store = new();

        public void Add(Record record)
        {
            _store[record.Id] = record;
        }

        public Record Get(Guid id)
        {
            return _store.TryGetValue(id, out var record) ? record : null;
        }

        public IEnumerable<Record> GetAll()
        {
            return _store.Values;
        }
    }
}
