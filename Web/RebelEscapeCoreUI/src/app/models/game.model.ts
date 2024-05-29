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
    move: number;
    currentStage: number;
}

export interface MoveResult {
    isGameOver: boolean;
    winnerPlayerId: string;
    soldierMove: number;
    rebelMove: number;
    rebelScore: number;
    soldierScore: number;
}