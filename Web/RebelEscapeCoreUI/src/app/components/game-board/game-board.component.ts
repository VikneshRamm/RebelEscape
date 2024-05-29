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
}

@Component({
  selector: 'app-game-board',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './game-board.component.html',
  styleUrl: './game-board.component.scss'
})
export class GameBoardComponent {
  
  public readonly NUMBER_OF_STATGES: number = 7;
  private _gameParams!: GameStartedParameters;
  private subscription: Subscription = new Subscription();

  public board: Cell[][] = [];
  public currentStage: number;
  public playerTypeImageSrc: string = '';
  public pendingSeconds: string = '-';

  constructor(private _gamePlayService: GameplayService,
    private _notificationService: NotificationService,
    private _router: Router
  ) { 
    this.currentStage = 1;
  }

  ngOnInit() {
    // this._gamePlayService.initializeEventListners();
    // this._gameParams =  this._gamePlayService.gameStartedParameters;
    // this.subscription = this._gamePlayService.gameMoveNotification$.subscribe((result) => this.afterMoveResult(result));
    this.generateBoard();
    this.playerTypeImageSrc = '../../../assets/soldier.png';
    this.pendingSeconds = '10';
  }

  ngOnDestroy() {
    // this.subscription.unsubscribe();
  }

  private generateBoard() {
    for (let i = 1; i <= this.NUMBER_OF_STATGES; i++) {
      const row: Cell[] = [];
      for (let j = 1; j <= i; j++) {
        let cell: Cell = {
          position: j,
          stage: this.NUMBER_OF_STATGES - i + 1,
        }
        row.push(cell);
      }
      this.board.push(row);
    }
  }

  makeMove(cell: Cell) {
    this._gamePlayService.play(cell.position);
  }

  afterMoveResult(moveResult: MoveResult) {
    if (moveResult.isGameOver) {
      
      if (moveResult.winnerPlayerId == this._gameParams.playerId) {
        this._notificationService.showNotification("Congratulations..You won the game");
      }
      else {
        this._notificationService.showNotification("You lost the game..better luck next time");
      }
      this._router.navigateByUrl('/players-list')
    }
    else {
      this.currentStage++;
    }
  }

  getPlayerTypeString(): string {    
      return PlayerTypes[0];
  }
 
}


