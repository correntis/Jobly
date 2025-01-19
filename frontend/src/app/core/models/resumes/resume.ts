import Certification from './certification';
import Education from './education';
import JobExperience from './jobExperience';
import Language from './language';
import Project from './project';

export default interface Resume {
  id: string;
  userId: string;
  title: string;
  summary: string;
  createdAt: Date;
  updatedAt: Date;
  skills: string[];
  tags: string[];
  jobExperiences: JobExperience[]; // -
  educations: Education[]; // +
  certifications: Certification[]; // +
  projects: Project[]; // +
  languages: Language[]; //+
}
