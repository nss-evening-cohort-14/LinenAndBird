using LinenAndBird.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;

namespace LinenAndBird.DataAccess
{
    public class BirdRepository
    {

        const string _connectionString = "Server=localhost;Database=LinenAndBird;Trusted_Connection=True;";

        internal IEnumerable<Bird> GetAll()
        {
            //connections are like the tunnel between our app and the database
            using var db = new SqlConnection(_connectionString);

            //Query<T> is for getting results from the database and putting them into a C# type
            var birds = db.Query<Bird>(@"Select * From Birds");

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
            //connections are like the tunnel between our app and the database
            using var db = new SqlConnection(_connectionString);

            var sql = @"Select *
                        From Birds
                        where id = @id";

            var bird = db.QuerySingleOrDefault<Bird>(sql, new { id = birdId });

            return bird;
        }
    }
}
