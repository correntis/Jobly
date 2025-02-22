export default interface Company {
  id: string;
  userId: string;
  name: string;
  logoPath: string;
  description: string;
  city: string;
  address: string;
  email: string;
  phone: string;
  webSite: string;
  type: string;
  createdAt: Date;
}
