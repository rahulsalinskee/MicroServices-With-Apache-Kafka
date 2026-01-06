using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ApplicationDataContext.DataBaseConfiguration
{
    public static class ApplicationDataBaseConfiguration
    {
        private const string RESOURCE_NAME = "ApplicationDataContext.appsettings.json";

        public static IConfiguration LoadConfiguration(IConfigurationBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName = RESOURCE_NAME;

            using var manifestResourceStream = assembly.GetManifestResourceStream(name: resourceName);

            if (manifestResourceStream is not null)
            {
                builder.AddJsonStream(stream: manifestResourceStream);
            }

            return builder.Build();
        }
    }
}
