using Data.DocSoporte;
using Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dependences
{
    public static class DataAccessDependency
    {
        public static IServiceCollection DataDependencyInjectionAccess(this IServiceCollection services)
        {
            #region [General]
            services.AddScoped<IDocumentoSoporteData, DocumentoSoporteData>();
            #endregion

            return services;
        }
    }
}
