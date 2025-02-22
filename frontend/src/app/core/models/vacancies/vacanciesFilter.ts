export interface VacanciesFilter {
  requirements: string[] | null;
  skills: string[] | null;
  tags: string[] | null;
  responsibilities: string[] | null;
  benefits: string[] | null;
  technologies: string[] | null;
  education: string[] | null;
  languages: LanguageFilter[] | null;
  experience: ExperienceFilter | null;
  salary: SalaryFilter | null;
  pageNumber: number;
  pageSize: number;
}

export interface ExperienceFilter {
  min?: number;
  max?: number;
}

export interface LanguageFilter {
  name: string;
  level: string;
}

export interface SalaryFilter {
  currency: string;
  min?: string;
  max?: string;
}
