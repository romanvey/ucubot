﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ucubot.Model;

namespace ucubot.Controllers
{
    [Route("api/[controller]")]
    public class LessonSignalEndpointController : Controller
    {
        private readonly IConfiguration _configuration;

        public LessonSignalEndpointController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<LessonSignalDto> ShowSignals()
        {
            
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            conn.Open();
            
            var command = new MySqlCommand("SELECT * FROM lesson_signal", conn);

            DataTable dt = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter(command);

            adapter.Fill(dt);

            foreach(DataRow row in dt.Rows)
            {
                LessonSignalDto tmpSignal = new LessonSignalDto
                {
                    Id = (int) row["id"],
                    Timestamp = (DateTime) row["timestamp"],
                    Type = (LessonSignalType) Convert.ToInt32(row["signal_type"]),
                    UserId = (string) row["user_id"]
                };


                yield return tmpSignal;
            }
            
            conn.Close();
            adapter.Dispose();
        }
        
        
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            conn.Open();
            
            var command = new MySqlCommand("SELECT * FROM lesson_signal WHERE id = " + Convert.ToString(id), conn);

            DataTable dt = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter(command);

            adapter.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                conn.Close();
                adapter.Dispose();
                return null;
            }

            DataRow row = dt.Rows[0];
            LessonSignalDto tmpSignal = new LessonSignalDto
            {
                Id = (int) row["id"],
                Timestamp = (DateTime) row["timestamp"],
                Type = (LessonSignalType) Convert.ToInt32(row["signal_type"]),
                UserId = (string) row["user_id"]
            };
            
            conn.Close();
            adapter.Dispose();

            return tmpSignal;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            conn.Open();
            
            var command = new MySqlCommand(String.Format("INSERT INTO lesson_signal (user_id, signal_type) VALUES ({0}, {1});", userId, signalType), conn);
            await command.ExecuteNonQueryAsync();
            
            conn.Close();
            
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            conn.Open();
            
            var command = new MySqlCommand(String.Format("DELETE FROM lesson_signal WHERE id = @id;", id), conn);
            await command.ExecuteNonQueryAsync();
            
            conn.Close();
            return Accepted();
        }
    }
}
