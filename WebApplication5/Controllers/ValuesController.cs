﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using ServiceStack.Redis;

namespace WebApplication5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value211" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet("Send/{message}")]
        public ActionResult<string> Send(string message)
        {
            var factory = new ConnectionFactory() { HostName = "10.0.75.1", Port = 5672, UserName = "admin", Password = "admin", VirtualHost = "/" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "test",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                        );

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                        routingKey: "test",
                        basicProperties: null,
                        body: body);
                }
            }

            return CallReceiver();
        }

        [HttpGet("Send1/{message}")]
        public ActionResult<string> Send1(string message)
        {

            var factory = new ConnectionFactory() { HostName = "rabbit", UserName = "admin", Password = "admin", VirtualHost = "/" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "test",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                        );

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                        routingKey: "test",
                        basicProperties: null,
                        body: body);
                }
            }

            return CallReceiver();
        }

        public string CallReceiver()
        {
            using (RedisClient redisClient = new RedisClient("10.0.75.1", 6379))
            {
                string isSuccess = Encoding.UTF8.GetString(redisClient.Get("message"));

                return isSuccess;
            }
        }

    }
}
