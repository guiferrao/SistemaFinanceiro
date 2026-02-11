using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.WriteLine("🚀 Aguardando mensagens na fila 'notificacoes'...");

var factory = new ConnectionFactory { HostName = "localhost" };

await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();


await channel.QueueDeclareAsync(queue: "notificacoes",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var mensagem = Encoding.UTF8.GetString(body);

    Console.WriteLine($" [x] Recebido: {mensagem}");

    await Task.Delay(1000);

    Console.WriteLine(" [✓] Processado com sucesso!");
};

await channel.BasicConsumeAsync(queue: "notificacoes",
                                autoAck: true,
                                consumer: consumer);

Console.ReadLine();