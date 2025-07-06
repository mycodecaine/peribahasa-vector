using Codecaine.Common.Exceptions;
using Codecaine.Common.Persistence.Dapper.Interfaces;
using Codecaine.PeribahasaVector.Application.UseCases.Peribahasas.Commands.CreatePeribahasa;
using Codecaine.PeribahasaVector.Domain.Entities;
using Codecaine.PeribahasaVector.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.PeribahasaVector.UnitTests.Application.UseCases.Peribahasas.CreatePeribahasa
{
    [TestFixture]
    internal class CreatePeribahasaCommandHandlerTests
    {
        private Mock<IPeribahasaRepository> _repositoryMock;
        private Mock<IDapperUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<CreatePeribahasaCommandHandler>> _loggerMock;
        private CreatePeribahasaCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IPeribahasaRepository>();
            _unitOfWorkMock = new Mock<IDapperUnitOfWork>();
            _loggerMock = new Mock<ILogger<CreatePeribahasaCommandHandler>>();
            _handler = new CreatePeribahasaCommandHandler(_repositoryMock.Object, _loggerMock.Object, _unitOfWorkMock.Object);
        }

        [Test]
        public async Task Handle_Should_Create_Peribahasa_And_Return_Success()
        {
            // Arrange
            var command = new CreatePeribahasaCommand(
                Teks: "Sedikit-sedikit, lama-lama jadi bukit",
                Maksud: "Usaha kecil lama-lama membuahkan hasil",
                TeksTranslation: "Little by little, it becomes a hill",
                MaksudTranslation: "Small effort leads to big success",
                Context: "Nasihat",
                Source: "Traditional"
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.EqualTo(true));
           
            Assert.That(result.Value.Id, Is.Not.EqualTo(Guid.Empty));

            // Verify expected interactions
            _unitOfWorkMock.Verify(u => u.StartTransactionAsync(It.IsAny<Guid>(), CancellationToken.None), Times.Once);
            _repositoryMock.Verify(r => r.Insert(It.IsAny<Peribahasa>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<Peribahasa>()), Times.Once);
        }

        [Test]
        public void Handle_WhenCancelled_ShouldLogAndThrowApplicationLayerException()
        {
            // Arrange
            var command = new CreatePeribahasaCommand(
                Teks: "Sedikit-sedikit, lama-lama jadi bukit",
                Maksud: "Usaha kecil lama-lama membuahkan hasil",
                TeksTranslation: "Little by little, it becomes a hill",
                MaksudTranslation: "Small effort leads to big success",
                Context: "Nasihat",
                Source: "Traditional"
            );
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel(); // simulate cancel

            _repositoryMock
                .Setup(r => r.Insert(It.IsAny<Peribahasa>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            var ex = Assert.ThrowsAsync<ApplicationLayerException>(async () =>
                await _handler.Handle(command, cancellationTokenSource.Token)
            );

            Assert.That(ex, Is.Not.Null);
            
        }

        [Test]
        public void Handle_WhenDomainExceptionThrown_ShouldPropagate()
        {
            // Arrange
            var command = new CreatePeribahasaCommand(
               Teks: "Sedikit-sedikit, lama-lama jadi bukit",
               Maksud: "Usaha kecil lama-lama membuahkan hasil",
               TeksTranslation: "Little by little, it becomes a hill",
               MaksudTranslation: "Small effort leads to big success",
               Context: "Nasihat",
               Source: "Traditional"
           );

            _repositoryMock
                .Setup(r => r.Insert(It.IsAny<Peribahasa>()))
                .ThrowsAsync(new DomainException(new Common.Primitives.Errors.Error("Error","Error")));

            // Act & Assert
            Assert.ThrowsAsync<DomainException>(async () =>
                await _handler.Handle(command, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_WhenInfrastructureExceptionThrown_ShouldPropagate()
        {
            // Arrange
            var command = new CreatePeribahasaCommand(
               Teks: "Sedikit-sedikit, lama-lama jadi bukit",
               Maksud: "Usaha kecil lama-lama membuahkan hasil",
               TeksTranslation: "Little by little, it becomes a hill",
               MaksudTranslation: "Small effort leads to big success",
               Context: "Nasihat",
               Source: "Traditional"
           );

            _repositoryMock
                .Setup(r => r.Insert(It.IsAny<Peribahasa>()))
                .ThrowsAsync(new InfrastructureException(new Common.Primitives.Errors.InfrastructureError("Error",new Exception("Error"))));

            Assert.ThrowsAsync<InfrastructureException>(async () =>
                await _handler.Handle(command, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_WhenNotFoundExceptionThrown_ShouldPropagate()
        {
            var command = new CreatePeribahasaCommand(
              Teks: "Sedikit-sedikit, lama-lama jadi bukit",
              Maksud: "Usaha kecil lama-lama membuahkan hasil",
              TeksTranslation: "Little by little, it becomes a hill",
              MaksudTranslation: "Small effort leads to big success",
              Context: "Nasihat",
              Source: "Traditional"
          );

            _repositoryMock
                .Setup(r => r.Insert(It.IsAny<Peribahasa>()))
                .ThrowsAsync(new NotFoundException(new Common.Primitives.Errors.Error("NotFound", "NotFound")));

            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(command, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_WhenUnexpectedExceptionThrown_ShouldWrapInApplicationLayerException()
        {
            var command = new CreatePeribahasaCommand(
              Teks: "Sedikit-sedikit, lama-lama jadi bukit",
              Maksud: "Usaha kecil lama-lama membuahkan hasil",
              TeksTranslation: "Little by little, it becomes a hill",
              MaksudTranslation: "Small effort leads to big success",
              Context: "Nasihat",
              Source: "Traditional"
          );
            _repositoryMock
                .Setup(r => r.Insert(It.IsAny<Peribahasa>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            var ex = Assert.ThrowsAsync<ApplicationLayerException>(async () =>
                await _handler.Handle(command, CancellationToken.None)
            );

            Assert.That(ex.Message, Does.Contain("Unexpected error"));
        }
    }
}
