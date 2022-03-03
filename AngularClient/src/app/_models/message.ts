export interface Messages {
  id: number;
  senderId: number;
  senderUsername: string;
  senderPhotoUrl: string;
  recipeintId: number;
  recepeintUsername: string;
  recipeintPhotoUrl: string;
  content: string;
  dateRead: Date;
  messageSent: Date;
}
