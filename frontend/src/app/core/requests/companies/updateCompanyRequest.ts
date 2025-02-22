export default interface UpdateCompanyRequest {
  id: string;
  name: string | null;
  description: string | null;
  city: string | null;
  address: string | null;
  email: string | null;
  phone: string | null;
  webSite: string | null;
  type: string | null;
}
