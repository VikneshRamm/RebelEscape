export interface GameStartedParameters {
    gameId: string;
    playerId: string;
    playerType: PlayerTypes
}

export enum PlayerTypes {
    soldier,
    rebel
}

export interface MoveDetails {
    gameId: string;
    playerId: string;
    playerType: PlayerTypes,
    move: number
}

export interface MoveResult {
    isGameOver: boolean;
    winnerPlayerId: string;
}