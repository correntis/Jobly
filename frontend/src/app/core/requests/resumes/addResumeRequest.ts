export default interface AddResumeRequest {
  userId: string;
  title: string;
  summary: string;
  skills: string[];
  tags: string[];
}
