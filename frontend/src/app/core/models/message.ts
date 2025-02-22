export default interface Message {
  id: string;
  senderId: string;
  recipientId: string;
  chatId: string;
  content: string;
  isEditing: boolean;
  isRead: boolean;
  type: number;
  sentAt: Date;
  editedAt?: Date;
}
