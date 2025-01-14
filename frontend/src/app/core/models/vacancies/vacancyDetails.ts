import Language from '../resumes/language';
import ExperienceLevel from './experienceLevel';
import Salary from './salary';

export default interface VacancyDetails {
  id: string;
  vacancyId: string;
  requirements: string[];
  skills: string[];
  tags: string[];
  responsibilities: string[];
  benefits: string[];
  education: string[];
  technologies: string[];
  languages: Language[];
  experienceLevel: ExperienceLevel;
  salary: Salary;
}
