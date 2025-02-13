﻿using Microsoft.Extensions.DependencyInjection;
using ShopTARgv23.ApplicationServices.Services;
using ShopTARgv23.Core.ServiceInterface;
using ShopTARgv23.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ShopTARgv23.KindergartenTest.Macros;
using Microsoft.Extensions.Hosting;
using ShopTARgv23.KindergartenTest.Mock;

namespace ShopTARgv23.KindergartenTest
{
    public abstract class TestBase
    {
        protected IServiceProvider serviceProvider { get; set; }
        protected TestBase()
        {
            var services = new ServiceCollection();
            SetupServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        protected T Svc<T>()
        {
            return serviceProvider.GetService<T>();
        }

        public void Dispose()
        {

        }

        protected T Macro<T>() where T : IMacros
        {
            return serviceProvider.GetService<T>();
        }

        public virtual void SetupServices(IServiceCollection services)
        {
            services.AddScoped<IKindergartenServices, KindergartenServices>();
            services.AddScoped<IFileServices, FileServices>();
            services.AddScoped<IHostEnvironment, IMockHostEnvironment>();

            services.AddDbContext<ShopTARgv23Context>(x =>
            {
                x.UseInMemoryDatabase("TEST");
                x.ConfigureWarnings(e => e.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            RegisterMacros(services);
        }

        private void RegisterMacros(IServiceCollection services)
        {
            var macroBaseType = typeof(IMacros);

            var macros = macroBaseType.Assembly.GetTypes()
                .Where(x => macroBaseType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

            foreach (var macro in macros)
            {
                services.AddSingleton(macro);
            }

        }
    }
}