using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using APIFinanceira.Controllers;
using APIFinanceira.Models;
using APIFinanceira.Data;
using APIFinanceira.Services;
using APIFinanceira.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace APIFinanceira.Tests
{
    public class TransacaoControllerTests
    {
        [Fact]
        public async Task CriarTransacao_DeSalvarNoBanco_E_EnviarNotificacao()
        {
            var options = new DbContextOptionsBuilder<ApiDataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new ApiDataContext(options);

            var mockNotificador = new Mock<INotificacaoService>();

            var controller = new TransacaoController(mockNotificador.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var model = new EditorTransacaoViewModel
            {
                Tipo = 1,
                Valor = 100.00m,
            };

            var resultado = await controller.CriarTransacaoAsync(model, context);

            var createdResult = Assert.IsType<CreatedResult>(resultado);
            Assert.Equal(201, createdResult.StatusCode);

            var TransacaoSalva = await context.Transacoes.FirstOrDefaultAsync();
            Assert.NotNull(TransacaoSalva);
            Assert.Equal(100, TransacaoSalva.Valor);

            mockNotificador.Verify(n => n.EnviarNotificacao(It.IsAny<Transacao>()), Times.Once);
        }
    }
}
