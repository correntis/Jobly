import Application from './application';
import Message from './message';

export default interface Chat {
  id: string;
  userId: string;
  companyId: string;
  applicationId: string;
  vacancyId: string;
  createdAt: string;
  lastMessageAt: Date;

  application?: Application;
  messages: Message[];
}
