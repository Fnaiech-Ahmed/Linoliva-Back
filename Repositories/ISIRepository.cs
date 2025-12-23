using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repository
{
    public interface ISIRepository<T>
    {
        public Task<int> AddSI(SI si);
        public Task<bool> UpdateSI(SI si);
        public Task<bool> DeleteSI(int SIId);
        public Task<List<SI>> GetSIs();
        public Task<SI?> GetSIById(int id);

    }
}
