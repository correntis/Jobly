export default interface SendMessageRequest {
  chatId: string;
  senderId: string;
  recipientId: string;
  content: string;
}
