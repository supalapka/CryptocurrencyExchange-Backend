using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptocurrencyExchange.Tests
{
    public static class DatabaseService
    {

        public static DataContext CreateInMemoryDbContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            return new DataContext(options);
        }

        public static DataContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
           .UseSqlServer("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=CryptoExchange_Tests;Integrated Security=True;")
           .Options;

            return new DataContext(options);
        }

        public static User CreateUser(int userId)
        {
            return new User
            {
                Id = userId,
                PasswordHash = Encoding.UTF8.GetBytes("qwe"),
                PasswordSalt = Encoding.UTF8.GetBytes("qwe"),
            };
        }

        public static WalletItem CreateWalletItem(int userId, string symbol, double amount)
        {
            return new WalletItem
            {
                UserId = userId,
                Symbol = symbol,
                Amount = amount
            };
        }

    }
}
