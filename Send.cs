using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "hello_fim_ex",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);


for (int i=0;i<10000;i++)
{
    string message = "Mensagem;" + String.Concat(Enumerable.Repeat(i.ToString(), i));
    message = message + message.Length * sizeof(char)  + " Bytes";
    var body = Encoding.UTF8.GetBytes(message);
    var properties = channel.CreateBasicProperties();
    properties.Persistent = true;

    channel.ExchangeDeclare("logs", ExchangeType.Fanout);
    var queueName = channel.QueueDeclare().QueueName;
    
    channel.QueueBind(queue: queueName,
                  exchange: "logs",
                  routingKey: string.Empty);

    channel.BasicPublish(exchange: "logs",
                         routingKey: String.Empty, //Using Exchange, routing key must be empty
                         basicProperties: null,
                         body: body);
    Console.WriteLine($" [x] Sent {message.Length * sizeof(char)} Bytes");

}


Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();