using AutoMapper;

namespace VacanciesService.Application.VacanciesDetails.Mapping.Converters
{
    public class NullIfEmptyListConverter<TSource, TDestination> : ITypeConverter<List<TSource>, List<TDestination>>
    {
        public List<TDestination> Convert(List<TSource> source, List<TDestination> destination, ResolutionContext context)
        {
            if (source == null || source.Count == 0)
            {
                return null;
            }

            return source.Cast<TDestination>().ToList();
        }
    }
}
