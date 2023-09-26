export interface IUser {
    ID: string;
    username: string;
    login: string;
    picture: string;
    isVerified: boolean;
    provider: string;
    socialID: string;
    metadata: any;
    createdAt: string;
    updatedAt: string;
}
