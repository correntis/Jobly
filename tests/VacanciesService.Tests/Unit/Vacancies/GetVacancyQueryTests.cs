using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VacanciesService.Application.Vacancies.Queries.GetVacancyQuery;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Entities.NoSQL;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Exceptions;
using VacanciesService.Domain.Models;

namespace VacanciesService.Tests.Unit.Vacancies
{
    public class GetVacancyQueryTests
    {
        private readonly Mock<ILogger<GetVacancyQueryHandler>> _loggerMock;
        public GetVacancyQueryTests()
        {
            _loggerMock = new Mock<ILogger<GetVacancyQueryHandler>>();
        }

        [Fact]
        public async Task ShouldGet_WhenVacancyExist()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var detailsRepMock = new Mock<IVacanciesDetailsRepository>();
            var readVacanciesRepMock = new Mock<IReadVacanciesRepository>();

            var id = Guid.NewGuid();
            var vacancyEntity = GetVacancyEntity(id);
            var vacancy = GetVacancy(id);

            var handler = new GetVacancyQueryHandler(
                _loggerMock.Object,
                mapperMock.Object,
                detailsRepMock.Object,
                readVacanciesRepMock.Object);

            readVacanciesRepMock.Setup(rv => rv.GetAsync(It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync(vacancyEntity);
            mapperMock.Setup(m => m.Map<Vacancy>(It.IsAny<VacancyEntity>())).Returns(vacancy);
            mapperMock.Setup(m => m.Map<VacancyDetails>(It.IsAny<VacancyDetailsEntity>())).Returns((VacancyDetails)null);
            detailsRepMock.Setup(dr => dr.GetByAsync(v => v.VacancyId, It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync((VacancyDetailsEntity)null);

            // Act
            var vacancyAct = await handler.Handle(new GetVacancyQuery(id), CancellationToken.None);

            // Assert
            vacancyAct.Id.Should().Be(id);

            readVacanciesRepMock.Verify(
                rv => rv.GetAsync(It.IsAny<Guid>(), CancellationToken.None),
                Times.Once);

            detailsRepMock.Verify(
                dr => dr.GetByAsync(v => v.VacancyId, It.IsAny<Guid>(), CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task ShouldThrowEntityNotFound_WhenVacancyNotExist()
        {
            // Arrange
            var readVacanciesRepMock = new Mock<IReadVacanciesRepository>();

            var handler = new GetVacancyQueryHandler(
                _loggerMock.Object,
                null,
                null,
                readVacanciesRepMock.Object);

            readVacanciesRepMock.Setup(rv => rv.GetAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync((VacancyEntity)null);

            // Act
            var act = async () => await handler.Handle(new GetVacancyQuery(Guid.NewGuid()), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        private Vacancy GetVacancy(Guid id)
        {
            return new Vacancy()
            {
                Id = id,
            };
        }

        private VacancyEntity GetVacancyEntity(Guid id)
        {
            return new VacancyEntity()
            {
                Id = id,
            };
        }
    }
}
