import { InteractionType } from '../enums/interactionType';

export default interface Interaction {
  id: string;
  createdAt: Date;
  userId: string;
  vacancyId: string;
  type: InteractionType;
}
