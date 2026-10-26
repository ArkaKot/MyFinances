using MyFinances.Models.Domains;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace MyFinances.Models
{
    public class OperationRepository
    {
        private readonly SQLiteAsyncConnection _context;
        public OperationRepository(SQLiteAsyncConnection context)
        {
            _context = context;
        }
        public async Task<int> AddAsync(Operations operations)
        {
            return await _context.InsertAsync(operations);
        }

        public async Task DeleteAsync(Operations operations)
        {
            await _context.DeleteAsync(operations);
        }

        public async Task<Operations> GetAsync(int id)
        {
            return await _context.Table<Operations>()
                .FirstAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Operations>> GetAsync(int records, int page)
        {

            return await _context.Table<Operations>().Skip(records * (page - 1)).Take(records).ToListAsync();
        }

        public async Task<IEnumerable<Operations>> GetAsync()
        {
            return await _context.Table<Operations>().ToListAsync();
        }

        public async Task<int> GetValueRecords()
        {
            return await _context.Table<Operations>().CountAsync();
        }

        public async Task UpdateAsync(Operations operation)
        {
            await _context.UpdateAsync(operation);

        }

    }
}
