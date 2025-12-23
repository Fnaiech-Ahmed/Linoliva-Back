using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public class SequenceRepository: ISequenceRepository<Sequence>
    {
        private readonly MyDbContext _dbContext;

        public SequenceRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Sequence?> GetById(int id)
        {
            return await _dbContext.Sequences.FindAsync(id);
        }

        public async Task AddSequence(Sequence entity)
        {
            await _dbContext.Sequences.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateSequence(Sequence entity)
        {
            _dbContext.Sequences.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Sequence entity)
        {
            _dbContext.Sequences.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public IEnumerable<Sequence> GetAll()
        {
            return _dbContext.Sequences.ToList();
        }

        public Sequence GetSequenceByName(string Name)
        {
            return _dbContext.Sequences.Find(Name);
        }

    }
}
