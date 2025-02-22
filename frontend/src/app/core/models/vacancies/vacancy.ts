import VacancyDetails from './vacancyDetails';

export default interface Vacancy {
  id: string;
  companyId: string;
  title: string;
  employmentType: string;
  archived: boolean;
  createdAt: Date;
  deadlineAt: Date;
  vacancyDetails?: VacancyDetails;
}
