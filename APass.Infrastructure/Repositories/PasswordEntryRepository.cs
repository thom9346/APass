﻿using APass.Core.Entities;
using APass.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APass.Infrastructure.Repositories
{
    public class PasswordEntryRepository : IRepository<PasswordEntry>
    {
        private readonly PasswordManagerContext _db;
        public PasswordEntryRepository(PasswordManagerContext db)
        {
            _db = db;
        }
        public void Add(PasswordEntry entity)
        {
            throw new NotImplementedException();
        }

        public void Edit(PasswordEntry entity)
        {
            throw new NotImplementedException();
        }

        public PasswordEntry Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PasswordEntry> GetAll()
        {
            throw new NotImplementedException();
        }

        public bool Remove(int id)
        {
            try
            {
                var passwordEntry = _db.PasswordEntries.FirstOrDefault(p => p.ID == id);
                if(passwordEntry != null)
                {
                    _db.PasswordEntries.Remove(passwordEntry);
                    _db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
            return false;

        }
    }
}
