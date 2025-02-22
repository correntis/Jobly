import Language from '../../models/resumes/language';
import ExperienceLevel from '../../models/vacancies/experienceLevel';
import Salary from '../../models/vacancies/salary';

export default interface AddVacancyDetailsRequest {
  vacancyId: string;
  requirements: string[];
  skills: string[];
  tags: string[];
  responsibilities: string[];
  benefits: string[];
  education: string[];
  technologies: string[];
  languages: Language[];
  experience: ExperienceLevel;
  salary: Salary;
}
