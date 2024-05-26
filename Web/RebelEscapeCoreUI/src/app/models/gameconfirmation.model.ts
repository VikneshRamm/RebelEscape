export enum GameConfirmationResult {
    Accepted,
    Rejected,
    Timedout
}

export interface GameRequestParameters {
    senderPlayerId: string;
    targetPlayerId: string;
}

export interface GameStartParameters {
    senderPlayerId: string;
    targetPlayerId: string;
}
