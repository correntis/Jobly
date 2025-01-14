export class Faker {
  private static domains = ['example.com', 'mail.com', 'test.org', 'demo.net'];

  public static generateRandomEmail(): string {
    const randomString = Math.random().toString(36).substring(2, 10);
    const randomDomain =
      this.domains[Math.floor(Math.random() * this.domains.length)];
    return `${randomString}@${randomDomain}`;
  }

  public static generateRandomString(): string {
    return Math.random().toString(36).substring(2, 10);
  }

  public static generateValidPassword(): string {
    return 'strinG123!';
  }
}
