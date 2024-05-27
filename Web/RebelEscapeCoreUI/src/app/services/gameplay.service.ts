import { Injectable } from '@angular/core';
import { HubCommunicationService } from './hubcommunication.service';
import { GameStartedParameters, MoveDetails, MoveResult } from '../models/game.model';
import { GameStartParameters } from '../models/gameconfirmation.model';
import { Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GameplayService {
  private _gameStartedParameters!: GameStartedParameters;   
  private gameMoveNotificationSubject: Subject<MoveResult>;
  private isInitializationDone = false;

  public gameStartedNotification: Subject<null>;
  public gameMoveNotification$: Observable<MoveResult>;
  
  constructor(private _hubCommunicationService: HubCommunicationService) {
    this.gameStartedNotification = new Subject();
    this.gameMoveNotificationSubject = new Subject();
    this.gameMoveNotification$ = this.gameMoveNotificationSubject.asObservable();
   }

  initializeEventListners() {
    if (this.isInitializationDone) {
      return;
    }
    this._hubCommunicationService.registerEvent("GameStarted", (gameStarted: GameStartedParameters) => {
      this._gameStartedParameters = gameStarted;
      this.gameStartedNotification.next(null);
    });

    this._hubCommunicationService.registerEvent("MoveResult", (moveResult: MoveResult) => {
      this.gameMoveNotificationSubject.next(moveResult);
    });
    this.isInitializationDone = true;
  }

  startGame(currentPlayerId: string, targetPlayerId: string) {
    let gameStartParams: GameStartParameters = {
      senderPlayerId: currentPlayerId,
      targetPlayerId: targetPlayerId
    }
    this._hubCommunicationService.callServerFunction("StartGame", gameStartParams);
  }

  play(move: number) {
    let moveDetails: MoveDetails = {
      gameId: this._gameStartedParameters.gameId,
      playerId: this._gameStartedParameters.playerId,
      move: move,
      playerType: this._gameStartedParameters.playerType
    }
    this._hubCommunicationService.callServerFunction("PlayerMove", moveDetails);
  }

  public get gameStartedParameters(): GameStartedParameters {
    return {...this._gameStartedParameters};
  }
  
}
