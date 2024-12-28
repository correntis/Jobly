using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VacanciesService.Application.Abstractions;
using VacanciesService.Application.VacanciesDetails.Commands.AddVacancyDetailsCommand;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Entities.NoSQL;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Exceptions;
using VacanciesService.Domain.Models;

namespace VacanciesService.Tests.Unit.VacanciesDetails
{
    public class AddVacancyDetailsCommandTests
    {
        private readonly Mock<ILogger<AddVacancyDetailsCommandHandler>> _loggerMock;

        public AddVacancyDetailsCommandTests()
        {
            _loggerMock = new Mock<ILogger<AddVacancyDetailsCommandHandler>>();
        }

        [Fact]
        public async Task ShouldAdd_WhenDetailsValid()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var detailsRepMock = new Mock<IVacanciesDetailsRepository>();
            var readVacanciesRepMock = new Mock<IReadVacanciesRepository>();
            var currencyApiMock = new Mock<ICurrencyApiService>();
            var currencyConverterMock = new Mock<ICurrencyConverter>();

            var command = GetCommand();
            var vacancyEntity = GetVacancyEntity(command.VacancyId);
            var vacancyDetailsEntity = GetVacancyDetailsEntityFromCommand(command);
            var exchangeRate = new ExchangeRate() { Code = "USD", Value = 1 };

            var handler = new AddVacancyDetailsCommandHandler(
                _loggerMock.Object,
                mapperMock.Object,
                detailsRepMock.Object,
                readVacanciesRepMock.Object,
                currencyApiMock.Object,
                currencyConverterMock.Object);

            mapperMock.Setup(m => m.Map<VacancyDetailsEntity>(command)).Returns(vacancyDetailsEntity);

            readVacanciesRepMock.Setup(rv => rv.GetAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(vacancyEntity);
            detailsRepMock.Setup(dr => dr.GetByAsync(v => v.VacancyId, It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync((VacancyDetailsEntity)null);

            currencyApiMock.Setup(ca => ca.GetExchangeRateAsync(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(exchangeRate);
            currencyConverterMock.Setup(c => c.Convert(command.Salary.Min, It.IsAny<decimal>()))
                .Returns(command.Salary.Min);
            currencyConverterMock.Setup(c => c.Convert(command.Salary.Max, It.IsAny<decimal>()))
                .Returns(command.Salary.Max);

            // Act
            var idAct = await handler.Handle(command, CancellationToken.None);

            // Assert
            readVacanciesRepMock.Verify(
                rv => rv.GetAsync(It.IsAny<Guid>(), CancellationToken.None),
                Times.Once);

            detailsRepMock.Verify(
                dr => dr.GetByAsync(v => v.VacancyId, It.IsAny<Guid>(), CancellationToken.None),
                Times.Once);

            currencyApiMock.Verify(
                ca => ca.GetExchangeRateAsync(It.IsAny<string>(), CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task ShouldThrowEntityAlreadyExist_WhenVacancyDetailsExist()
        {
            // Arrange
            var readVacanciesRepMock = new Mock<IReadVacanciesRepository>();
            var detailsRepMock = new Mock<IVacanciesDetailsRepository>();

            var handler = new AddVacancyDetailsCommandHandler(
                _loggerMock.Object,
                null,
                detailsRepMock.Object,
                readVacanciesRepMock.Object,
                null,
                null);

            readVacanciesRepMock.Setup(rv => rv.GetAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(new VacancyEntity());
            detailsRepMock.Setup(dr => dr.GetByAsync(v => v.VacancyId, It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(new VacancyDetailsEntity());

            // Act
            var act = async () => await handler.Handle(GetCommand(), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityAlreadyExistException>();
        }

        [Fact]
        public async Task ShouldThrowEntityNotFound_WhenVacancyNotExist()
        {
            // Arrange
            var readVacanciesRepMock = new Mock<IReadVacanciesRepository>();

            var handler = new AddVacancyDetailsCommandHandler(
                _loggerMock.Object,
                null,
                null,
                readVacanciesRepMock.Object,
                null,
                null);

            readVacanciesRepMock.Setup(rv => rv.GetAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync((VacancyEntity)null);

            // Act
            var act = async () => await handler.Handle(GetCommand(), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        private VacancyEntity GetVacancyEntity(Guid id)
        {
            return new VacancyEntity()
            {
                Id = id,
            };
        }

        private VacancyDetailsEntity GetVacancyDetailsEntityFromCommand(AddVacancyDetailsCommand command)
        {
            return new VacancyDetailsEntity()
            {
                VacancyId = command.VacancyId,
                Benefits = command.Benefits,
                Education = command.Education,
                Experience = new ExperienceLevelEntity()
                {
                    Max = command.Experience.Max,
                    Min = command.Experience.Min,
                },
                Languages = command.Languages.Select(l => new LanguageEntity()
                {
                    Name = l.Name,
                    Level = l.Level,
                }).ToList(),
                Requirements = command.Requirements,
                Responsibilities = command.Responsibilities,
                Salary = new SalaryEntity() { },
                Skills = command.Skills,
                Tags = command.Tags,
                Technologies = command.Technologies,
            };
        }

        private AddVacancyDetailsCommand GetCommand()
        {
            var faker = new Faker("ru");

            return new AddVacancyDetailsCommand(
                faker.Random.Guid(),
                Enumerable.Range(1, 5).Select(_ => faker.Lorem.Sentence()).ToList(),
                Enumerable.Range(1, 5).Select(_ => faker.Lorem.Sentence()).ToList(),
                Enumerable.Range(1, 5).Select(_ => faker.Lorem.Word()).ToList(),
                Enumerable.Range(1, 5).Select(_ => faker.Lorem.Sentence()).ToList(),
                Enumerable.Range(1, 5).Select(_ => faker.Lorem.Sentence()).ToList(),
                Enumerable.Range(1, 5).Select(_ => faker.Lorem.Sentence()).ToList(),
                Enumerable.Range(1, 5).Select(_ => faker.Lorem.Word()).ToList(),
                Enumerable.Range(1, 3).Select(x => new Language()
                {
                    Level = faker.Lorem.Word(),
                    Name = faker.Lorem.Word(),
                }).ToList(),
                new ExperienceLevel()
                {
                    Min = faker.Random.Int(0, 3),
                    Max = faker.Random.Int(3, 10),
                },
                new Salary()
                {
                    Min = faker.Random.Decimal(),
                    Max = faker.Random.Decimal(),
                    Currency = "USD",
                });
        }
    }
}
