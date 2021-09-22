using LinenAndBird.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace LinenAndBird.DataAccess
{
    public class OrdersRepository
    {
        string _connectionString;
        //this is asking asp.net for the application configuration 
        public OrdersRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("LinenAndBird");
        }

        internal IEnumerable<Order> GetAll()
        {
            //create a connection
            using var db = new SqlConnection(_connectionString);

            var sql = @"select *
                        from Orders o
	                        join Birds b 
		                        on b.Id = o.BirdId
	                        join Hats h
		                        on h.Id = o.HatId";

            var results = db.Query<Order, Bird, Hat, Order>(sql, Map, splitOn: "Id");

            return results;

        }

        internal void Add(Order order)
        {
            //Create a connection
            using var db = new SqlConnection(_connectionString);

            //order.Id = Guid.NewGuid();

            var sql = @"INSERT INTO [dbo].[Orders]
                               ([BirdId]
                               ,[HatId]
                               ,[Price])
                        Output inserted.Id
                        VALUES
                               (@BirdId
                               ,@HatId
                               ,@Price)";

            var parameters = new
            {
                BirdId = order.Bird.Id,
                HatId = order.Hat.Id,
                Price = order.Price
            };

            var id = db.ExecuteScalar<Guid>(sql, parameters);

            order.Id = id;
        }

        internal Order Get(Guid id)
        {
            //create a connection
            using var db = new SqlConnection(_connectionString);

            var sql = @"select *
                        from Orders o
	                        join Birds b 
		                        on b.Id = o.BirdId
	                        join Hats h
		                        on h.Id = o.HatId
                        where o.id = @id";

            //multi-mapping doesn't work for any other kind of dapper call,
            //so we take the collection and turn it into one item ourselves
            var orders = db.Query<Order, Bird, Hat, Order>(sql, Map, new { id }, splitOn: "Id");

            return orders.FirstOrDefault();
        }

        Order Map(Order order, Bird bird, Hat hat)
        {
            order.Bird = bird;
            order.Hat = hat;
            return order;
        }
    }
}
