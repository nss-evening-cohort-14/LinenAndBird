using LinenAndBird.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace LinenAndBird.DataAccess
{
    public class BirdRepository
    {
        readonly string _connectionString;

        // http request => IConfiguration => BirdRepository => BirdController

        //this is asking asp.net for the application configuration
        public BirdRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("LinenAndBird");
        }

        internal IEnumerable<Bird> GetAll()
        {
            //using 2 queries to get all the birds and their accessories

            //connections are like the tunnel between our app and the database
            using var db = new SqlConnection(_connectionString);

            //Query<T> is for getting results from the database and putting them into a C# type
            var birds = db.Query<Bird>(@"Select * From Birds");

            //lets get the accessories for all birds.
            var accessorySql = @"Select * From BirdAccessories";
            var accessories = db.Query<BirdAccessory>(accessorySql);

            foreach (var bird in birds)
            {
                bird.Accessories = accessories.Where(accessory => accessory.BirdId == bird.Id);
            }

            return birds;
        }

        internal Bird Update(Guid id, Bird bird)
        {
            //prep work for the sql stuff
            using var db = new SqlConnection(_connectionString);

            var sql = @"update Birds 
                        Set Color = @color,
                            Name = @name,
	                        Type = @type,
	                        Size = @size
                        output inserted.*
                        Where id = @id";

            bird.Id = id;
            var updatedBird = db.QuerySingleOrDefault<Bird>(sql, bird);

            return updatedBird;
        }

        internal void Remove(Guid id)
        {
            using var db = new SqlConnection(_connectionString);
            var sql = @"Delete 
                        From Birds 
                        Where Id = @id";

            db.Execute(sql, new { id });
        }

        internal void Add(Bird newBird)
        {
            using var db = new SqlConnection(_connectionString);

            var sql = @"insert into birds(Type,Color,Size,Name)
                        output inserted.Id
                        values (@Type,@Color,@Size,@Name)";

            var id = db.ExecuteScalar<Guid>(sql, newBird);
            newBird.Id = id;
        }

        internal Bird GetById(Guid birdId)
        {
            //Get one-to-many relationships using 2 separate queries

            //connections are like the tunnel between our app and the database
            using var db = new SqlConnection(_connectionString);

            var birdSql = @"Select *
                        From Birds
                        where id = @id";

            var bird = db.QuerySingleOrDefault<Bird>(birdSql, new { id = birdId });

            if (bird == null) return null;

            //lets get the accessories for the bird.

            var accessorySql = @"Select *  
                                 From BirdAccessories 
                                 where birdid = @birdId";

            var accessories = db.Query<BirdAccessory>(accessorySql, new { birdId });

            bird.Accessories = accessories;

            return bird;
        }
    }
}
