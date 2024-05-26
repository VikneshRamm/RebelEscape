export interface GameStartedParameters {
    gameId: string;
    playerId: string;
    playerType: PlayerTypes
}

export enum PlayerTypes {
    soldier,
    rebel
}