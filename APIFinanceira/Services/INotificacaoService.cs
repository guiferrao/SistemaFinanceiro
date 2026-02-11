using System.Threading.Tasks;

namespace APIFinanceira.Services
{
    public interface INotificacaoService
    {
        Task EnviarNotificacao(object transacao);
    }
}
