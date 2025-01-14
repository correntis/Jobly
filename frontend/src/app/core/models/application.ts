import Vacancy from './vacancies/vacancy';

export default interface Application {
  id: string;
  userId: string;
  createdAt: Date;
  appliedAt?: Date;
  status: string;
  vacancy: Vacancy;
}
