using System;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model.Base;
using SmartArch.Data;
using PDR.Domain.Services;
using System.Configuration;

namespace PDR.WeeklyReports
{
    public class CurrentStorage<T> : ICurrentStorage<T>
        where T : Entity
    {
        private readonly INativeRepository<T> repository;

        private T entity;

        public CurrentStorage(INativeRepository<T> repository)
        {
            this.repository = repository;
        }

        public T Get()
        {
            long id;
            bool result = Int64.TryParse(ConfigurationManager.AppSettings["adminId"], out id);
            if (result)
            {
                this.entity = this.repository.Get(id);
            }
            return this.entity;
        }
    }
}