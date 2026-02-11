using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace APIFinanceira.Services
{
    public class NotificacaoService : INotificacaoService
    {
        public async Task EnviarNotificacao(object transacao)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();
            {
                await channel.QueueDeclareAsync(queue: "notificacoes",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                string mensagemJson = JsonSerializer.Serialize(transacao);
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(transacao));
                await channel.BasicPublishAsync(exchange: "",
                                     routingKey: "notificacoes",
                                     mandatory: false,
                                     basicProperties: new BasicProperties(),
                                     body: body);
            }
        }
    }
}
