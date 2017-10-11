﻿namespace CommonNews.Services.Data
{
    using System;
    using System.Linq;

    using Common.Contracts;
    using CommonNews.Data.Common.Contracts;
    using CommonNews.Data.Models.Contracts;

    public abstract class BaseDataService<T> : IBaseDataService<T>
        where T : class, IDeletableEntity, IAuditInfo
    {
        public BaseDataService(IEfRepository<T> repo, ISaveContext context)
        {
            this.Data = repo;
            this.Context = context;
        }

        protected IEfRepository<T> Data { get; set; }

        protected ISaveContext Context { get; set; }

        public virtual void Add(T item)
        {
            this.Data.Add(item);
            this.Context.Commit();
        }

        public virtual void Delete(object id)
        {
            var entity = this.Data.GetById(id);
            if (entity == null)
            {
                throw new InvalidOperationException("No entity with provided id found.");
            }

            this.Data.Delete(entity);
            this.Context.Commit();
        }

        public virtual IQueryable<T> GetAll()
        {
            return this.Data.All();
        }

        public virtual T GetById(object id)
        {
            return this.Data.GetById(id);
        }

        public virtual void Save()
        {
            this.Context.Commit();
        }
    }
}