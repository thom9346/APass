using APass.Core.Entities;
using APass.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APass.Infrastructure.Repositories
{
    public class MasterPasswordRepository : IRepository<MasterPassword>
    {
        private readonly PasswordManagerContext _db;
        public MasterPasswordRepository(PasswordManagerContext db)
        {
            _db = db;
        }
        public void Add(MasterPassword entity)
        {
            // Check if a MasterPassword already exists
            if (!_db.MasterPasswords.Any())
            {
                _db.MasterPasswords.Add(entity);
                _db.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("A master password already exists.");
            }
        }

        public void Edit(MasterPassword entity)
        {
            throw new NotImplementedException();
        }

        public MasterPassword Get(int id)
        {
            //There will always be only one, so just get the first one no matter what currently.
            //not great design, but... ill come back to this... maybe
            return _db.MasterPasswords.FirstOrDefault();
        }

        public IEnumerable<MasterPassword> GetAll()
        {
            throw new NotImplementedException();
        }

        public bool Remove(int id)
        {
            throw new NotImplementedException();
        }
    }
}
