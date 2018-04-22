using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ucubot.Model;

namespace ucubot.Controllers
{
    [Route("api/[controller]")]
    public class StudentEndpointController : Controller
    {
        private readonly IConfiguration _configuration;

        public StudentEndpointController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        
        [HttpGet("{id}")]
        public Student GetStudent(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            conn.Open();
            
            var students = conn.Query<Student>("SELECT * FROM student WHERE id = @Id", new { Id = id });
            conn.Close();
            return students.First();
        }
        
        
        
        [HttpGet]
        public IEnumerable<Student> GetStudents()
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            conn.Open();
            
            var students = conn.Query<Student>("SELECT * FROM student");
            conn.Close();
            return students;
        }
        
        
        [HttpPost]
        public async Task<IActionResult> CreateStudent(Student student)
        {

            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            conn.Open();
            try
            {
                var students = conn.Query<Student>("INSERT INTO student (first_name, last_name, user_id) VALUES (@FirstName, @LastName, @UserId);",
                    new {FirstName = student.FirstName, LastName = student.LastName, UserId = student.UserId});
                conn.Close();
                return Accepted();
            }
            catch
            {
                conn.Close();
                return StatusCode(409);
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveStudent(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            conn.Open();
            
            try
            {
                var students = conn.Query<Student>("DELETE FROM student WHERE id = @id;", new { Id = id });
                conn.Close();
                return Accepted();
            }
            catch
            {
                conn.Close();
                return StatusCode(409);
            }
        }
        
        [HttpPut]
        public async Task<IActionResult> UpdateStudent(Student student)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            conn.Open();

            var students = conn.Query<Student>("UPDATE student SET first_name = @FirstName, last_name = @LastName, id = @id, user_id = @UserId " +
                                               "WHERE id = @id;", new { Id = student.Id, FirstName = student.FirstName, LastName = student.LastName, UserId = student.UserId });

            conn.Close();
            return Accepted();
        }
    }
}