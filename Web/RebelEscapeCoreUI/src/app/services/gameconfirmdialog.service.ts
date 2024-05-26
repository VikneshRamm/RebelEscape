import { Injectable } from '@angular/core';
import { HubCommunicationService } from './hubcommunication.service';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Subject } from 'rxjs';
import { GameConfirmationResult, GameRequestParameters } from '../models/gameconfirmation.model';
import { GameAcceptDialogComponent } from '../components/game-accept-dialog/game-accept-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class GameConfirmDialogService {

  private popupCloseSubject: Subject<GameConfirmationResult> = new Subject<GameConfirmationResult>();
  private _dialogRef!: MatDialogRef<GameAcceptDialogComponent>;
  private _dialogTimeoutId: any;
  private _isWaitingForAnotherRequest: boolean = false;

  constructor(private _hubCommunicationService: HubCommunicationService,
    public _dialog: MatDialog
  ) { }

  initializeEventListeners() {
    this._hubCommunicationService.registerEvent("GameRequestConfirmation", async (playerName: string) => {
      if (this._isWaitingForAnotherRequest) {
        return GameConfirmationResult.Rejected; // just reject when a player is waiting for confirmation from another player
      }
      this._dialogRef = this._dialog.open(GameAcceptDialogComponent, {
        data: playerName
      })
      let promise = this.sleep(10000);
      let result = await promise;
      return result;
    })
  }

  async sleep(ms: number): Promise<GameConfirmationResult> {
    return new Promise(
        (resolve, _) => { 
          this._dialogRef.afterClosed().subscribe(value => {
            clearTimeout(this._dialogTimeoutId);
            resolve(value);
          });
          this._dialogTimeoutId = setTimeout(() => {
            this._dialogRef.close(GameConfirmationResult.Timedout);
          }, ms)
     });       
  }

  async requestGameAsync(currentPlayerId: string, targetPlayerId: string) : Promise<GameConfirmationResult> {
    let parameters: GameRequestParameters = {
      senderPlayerId: currentPlayerId,
      targetPlayerId: targetPlayerId
    }
    this._isWaitingForAnotherRequest = true;
    let confirmationStatus: GameConfirmationResult = await this._hubCommunicationService
      .callServerFunctionWithReturnValue<GameConfirmationResult>("RequestGame", parameters);
      this._isWaitingForAnotherRequest = false;
    return confirmationStatus;
  }

}
