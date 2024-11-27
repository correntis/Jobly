namespace VacanciesService.Domain.Models
{
    public class Currency
    {
        public string Name { get; set; }
        public string SymbolNative { get; set; }
        public int DecimalDigits { get; set; }
        public int Rounding { get; set; }
        public string Code { get; set; }
    }
}
