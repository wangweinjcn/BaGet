﻿using System;
using System.Collections.Generic;
using BaGet.Core.Configuration;
using BaGet.Core.Entities;
using BaGet.Entities;
using BaGet.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BaGet.Tests
{
    public class ServiceCollectionExtensionsTest
    {
        private readonly string DatabaseTypeKey = $"{nameof(BaGetOptions.Database)}:{nameof(DatabaseOptions.Type)}";
        private readonly string ConnectionStringKey = $"{nameof(BaGetOptions.Database)}:{nameof(DatabaseOptions.ConnectionString)}";

        [Fact]
        public void AskServiceProviderForNotConfiguredDatabaseOptions()
        {
            ServiceProvider provider = new ServiceCollection()
                .AddBaGetContext() //Method Under Test
                .BuildServiceProvider();

            var expected = Assert.Throws<InvalidOperationException>(
                () => provider.GetRequiredService<IContext>().Database
            );

            Assert.Contains(nameof(BaGetOptions.Database), expected.Message);
        }

        [Fact]
        public void AskServiceProviderForWellConfiguredDatabaseOptions()
        {
            //Create a IConfiguration with a minimal "Database" object.
            Dictionary<string, string> initialData = new Dictionary<string, string>();
            initialData.Add(DatabaseTypeKey, DatabaseType.Sqlite.ToString());
            initialData.Add(ConnectionStringKey, "blabla");

            IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(initialData).Build();

            ServiceProvider provider = new ServiceCollection()
                .Configure<BaGetOptions>(configuration)
                .AddBaGetContext() //Method Under Test
                .BuildServiceProvider();

            Assert.NotNull(provider.GetRequiredService<IContext>());
        }

        [Fact]
        public void AskServiceProviderForWellConfiguredSqliteContext()
        {
            //Create a IConfiguration with a minimal "Database" object.
            Dictionary<string, string> initialData = new Dictionary<string, string>();
            initialData.Add(DatabaseTypeKey, DatabaseType.Sqlite.ToString());
            initialData.Add(ConnectionStringKey, "blabla");

            IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(initialData).Build();

            ServiceProvider provider = new ServiceCollection()
                .Configure<BaGetOptions>(configuration)
                .AddBaGetContext() //Method Under Test
                .BuildServiceProvider();

            Assert.NotNull(provider.GetRequiredService<SqliteContext>());
        }

        [Theory]
        [InlineData("<invalid>")]
        [InlineData("")]
        [InlineData(" ")]
        public void AskServiceProviderForInvalidDatabaseType(string databaseType)
        {
            //Create a IConfiguration with a minimal "Database" object.
            Dictionary<string, string> initialData = new Dictionary<string, string>();
            initialData.Add(DatabaseTypeKey, databaseType);

            IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(initialData).Build();

            ServiceProvider provider = new ServiceCollection()
                .Configure<BaGetOptions>(configuration)
                .AddBaGetContext() //Method Under Test
                .BuildServiceProvider();

            var expected = Assert.Throws<InvalidOperationException>(
                           () => provider.GetRequiredService<IContext>().Database
                       );
        }
    }
}
