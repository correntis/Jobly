export default interface UpdateResumeRequest {
  id: string;
  userId: string;
  title: string;
  summary: string;
  skills: string[];
  tags: string[];
}
