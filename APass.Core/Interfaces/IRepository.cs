﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APass.Core.Interfaces
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T Get(int id);
        void Add(T entity);
        void Edit(T entity);
        bool Remove(int id);
    }
}
