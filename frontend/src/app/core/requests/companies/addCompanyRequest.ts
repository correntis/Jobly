export default interface AddCompanyRequest {
  userId: string;
  name: string;
  description: string | null;
  city: string | null;
  email: string | null;
  phone: string | null;
  type: string;
  unp: string;
}
