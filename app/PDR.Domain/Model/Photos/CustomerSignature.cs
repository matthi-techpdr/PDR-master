﻿namespace PDR.Domain.Model.Photos
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class CustomerSignature : Photo
    {
        protected CustomerSignature()
        {
        }


        public CustomerSignature(byte[] photo, string contentType, bool isNewEntity = false)
            : base(photo, contentType)
        {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }
    }
}