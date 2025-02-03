export class ApiConfig {
  static readonly baseHttpsPath = 'https://localhost';
  static readonly baseHttpPath = 'http://localhost';

  static readonly resources = `${ApiConfig.baseHttpsPath}/resources`;
  static readonly auth = `${ApiConfig.baseHttpsPath}/auth`;
  static readonly users = `${ApiConfig.baseHttpsPath}/users`;
  static readonly companies = `${ApiConfig.baseHttpsPath}/companies`;
  static readonly resumes = `${ApiConfig.baseHttpsPath}/resumes`;
  static readonly currencies = `${ApiConfig.baseHttpsPath}/currencies`;
  static readonly applications = `${ApiConfig.baseHttpsPath}/applications`;
  static readonly interactions = `${ApiConfig.baseHttpsPath}/interactions`;
  static readonly vacancies = `${ApiConfig.baseHttpsPath}/vacancies`;
  static readonly chats = `${ApiConfig.baseHttpsPath}/chats`;
  static readonly messages = `${ApiConfig.baseHttpsPath}/messages`;

  static readonly hubsBasePath = `${this.baseHttpPath}/api/hubs`;
  static readonly messagesHub = `${this.hubsBasePath}/messages`;
  static readonly chatsHub = `${this.hubsBasePath}/chats`;
}
