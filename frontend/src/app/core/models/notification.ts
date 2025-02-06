import { NotificationStatus } from '../enums/notificationStatus';
import { NotificationType } from '../enums/notificationType';

export default interface Notification {
  id: string;
  recipientId: string;
  content: string;
  type: NotificationType;
  status: NotificationStatus;
  createdAt: Date;
  metadata: { [key: string]: string };
}
