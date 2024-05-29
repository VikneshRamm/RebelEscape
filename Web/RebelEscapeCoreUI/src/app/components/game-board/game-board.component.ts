import { Component } from '@angular/core';
import { GameplayService } from '../../services/gameplay.service';
import { GameStartedParameters, MoveDetails, MoveResult, PlayerTypes } from '../../models/game.model';
import { NotificationService } from '../../services/notification.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';

export interface Cell {
  stage: number;
  position: number;
  isSelected: boolean;
}

@Component({
  selector: 'app-game-board',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './game-board.component.html',
  styleUrl: './game-board.component.scss'
})
export class GameBoardComponent {
  
  public readonly NUMBER_OF_STATGES: number = 10;
  private _gameParams!: GameStartedParameters;
  private subscription: Subscription = new Subscription();
  private timerRef!: any;
  private timeoutRef: any;
  private currentStageCache: number = 0;

  public board: Cell[][] = [];
  public currentStage: number;
  public playerTypeImageSrc: string = '';
  public pendingSeconds: number = -1;
  public showWaitForOpponentOverlay: boolean;
  public showWinnerOverlay: boolean;
  public showLooserOverlay: boolean;
  public showAfterStageResultMessage: boolean;
  public winnerMessage: string = '';
  public loserMessage: string = '';

  constructor(private _gamePlayService: GameplayService,
    private _notificationService: NotificationService,
    private _router: Router
  ) { 
    this.currentStage = 1;
    this.showWinnerOverlay = this.showLooserOverlay
     = this.showWaitForOpponentOverlay
     = this.showAfterStageResultMessage = false;
  }

  ngOnInit() {
    this._gamePlayService.initializeEventListners();
    this._gameParams =  this._gamePlayService.gameStartedParameters;
    this.subscription = this._gamePlayService.gameMoveNotification$.subscribe((result) => this.afterMoveResult(result));
    this.generateBoard();
    this.playerTypeImageSrc = this._gameParams.playerType == PlayerTypes.soldier
     ? '../../../assets/soldier.png' :
     '../../../assets/rebel.png' ;
    this.pendingSeconds = 10;    
  }

  ngAfterViewInit() {
    this.startStageTimer();
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  private generateBoard() {
    for (let i = 1; i <= this.NUMBER_OF_STATGES; i++) {
      const row: Cell[] = [];
      for (let j = 1; j <= i; j++) {
        let cell: Cell = {
          position: j,
          stage: this.NUMBER_OF_STATGES - i + 1,
          isSelected: false
        }
        row.push(cell);
      }
      this.board.push(row);
    }
  }

  makeMove(cell: Cell) { 
    cell.isSelected = true;
    this.move(cell.position);
  }

  move(position: number) {
    clearInterval(this.timerRef);
    clearTimeout(this.timeoutRef);
    this.currentStageCache = this.currentStage;
    this._gamePlayService.play(position, this.currentStageCache);
    this.currentStage = 0;
    this.showWaitForOpponentOverlay = true;
  }

  afterMoveResult(moveResult: MoveResult) {
    console.log(moveResult);
    this.showWaitForOpponentOverlay = false;
    if (moveResult.isGameOver) {
      if (moveResult.winnerPlayerId == this._gameParams.playerId) {
        this.showWinnerOverlay = true;
        this.winnerMessage = this.getWinnerMessage(moveResult);
        this.startGameOverMessageTimer();
      }
      else {
        this.showLooserOverlay = true;
        this.loserMessage = this.getLoserMessage(moveResult);
        this.startGameOverMessageTimer();
      }      
    }
    else {      
      this.showAfterStageResultMessage = true;
      this.startStageResultMessageTimer();
    }
  }

  getPlayerTypeString(): string {    
      return PlayerTypes[this._gameParams.playerType];
  }

  getAfterResultMessage(): string {
    return this._gameParams.playerType == PlayerTypes.soldier ? 'You missed the target'
    : 'It was a lucky escape';
  }

  startStageTimer() {
    this.pendingSeconds = 10;
    this.timerRef = setInterval(() => {
      this.pendingSeconds--;
    }, 1000)
    this.timeoutRef = setTimeout(() => {
      this._notificationService.showErrorNotification("Timeout occurred. You cannot choose your move");
      this.move(-1);
    }, 10000)
  }

  startGameOverMessageTimer() {
    setTimeout(() => {
      this.showWinnerOverlay = false;
      this.showLooserOverlay = false;
      this._router.navigateByUrl('/players-list')
    }, 10000);
  }

  startStageResultMessageTimer() {
    setTimeout(() => {
      this.showAfterStageResultMessage = false;
      this.currentStage = this.currentStageCache;
      this.currentStage++;
      this.startStageTimer();
    }, 3000);
  }

  private getWinnerMessage(moveResult: MoveResult): string {
    let opponentMove = this._gameParams.playerType == PlayerTypes.soldier ?
     moveResult.rebelMove : moveResult.soldierMove;
    let score = this._gameParams.playerType == PlayerTypes.soldier ? 
      moveResult.soldierScore
    : moveResult.rebelScore;
    return opponentMove == -1 ?
     `You Won!! Opponent Timed Out. Your score: ${score}`
     : this._gameParams.playerType == PlayerTypes.soldier ?
      `You Won!! Target Hit!! Your score: ${score}`
      : `You escaped!! You are free!! Your score: ${score}`;
  }
  
  private getLoserMessage(moveResult: MoveResult): string {
    let myMove = this._gameParams.playerType == PlayerTypes.soldier ?
     moveResult.soldierMove : moveResult.rebelMove;
    let score = this._gameParams.playerType == PlayerTypes.soldier ? 
      moveResult.soldierScore
    : moveResult.rebelScore;
    return myMove == -1 ?
     `You Lost!! Timed Out!! Your score: ${score}`
     : this._gameParams.playerType == PlayerTypes.soldier ?
      `You lost!! Rebel escaped!! Your score: ${score}`
      : `You lost!! You are blasted!! Your score: ${score}`;
  } 
}



