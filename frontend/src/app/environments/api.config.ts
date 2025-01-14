export class ApiConfig {
  static readonly basePath = 'https://localhost';
  static readonly auth = `${ApiConfig.basePath}/auth`;
  static readonly users = `${ApiConfig.basePath}/users`;
  static readonly companies = `${ApiConfig.basePath}/companies`;
  static readonly resumes = `${ApiConfig.basePath}/resumes`;
  static readonly currencies = `${ApiConfig.basePath}/currencies`;
  static readonly applications = `${ApiConfig.basePath}/applications`;
  static readonly interactions = `${ApiConfig.basePath}/interactions`;
  static readonly vacancies = `${ApiConfig.basePath}/vacancies`;
}
