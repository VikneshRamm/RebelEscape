import { Component } from '@angular/core';
import { HubconnectionService } from '../../services/hubconnection.service';
import { PlayerInfo } from '../../models/playerinfo.model';
import { NotificationService } from '../../services/notification.service';
import { MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { GameConfirmDialogService } from '../../services/gameconfirmdialog.service';
import { AuthService } from '../../services/auth.service';
import { GameConfirmationResult } from '../../models/gameconfirmation.model';

@Component({
  selector: 'app-players-list',
  standalone: true,
  imports: [MatTableModule, CommonModule, MatButtonModule],
  templateUrl: './players-list.component.html',
  styleUrl: './players-list.component.scss'
})
export class PlayersListComponent {

  public connectedPlayerList!: PlayerInfo[];
  public readonly displayedColumns: string[] = ['sno', 'username', 'playerId'];
  public selectedPLayerRow!: number;
  constructor(private _hubConnectionService: HubconnectionService,
    private _notificationService: NotificationService,
    private _router: Router,
    private _gameConfirmDialogService: GameConfirmDialogService,
    private _authService: AuthService
  ) {
    this.selectedPLayerRow = -1;
  }


  async ngOnInit() {
    try {
      let isConnectionSuccess = await this._hubConnectionService.initializeConnection();
      if (isConnectionSuccess) {
        this._hubConnectionService.refreshPlayerListSubject.subscribe(playerInfo => {
          this.connectedPlayerList = playerInfo;
          this.selectedPLayerRow = -1;
        })
        this._gameConfirmDialogService.initializeEventListeners();
      }
      else {
        this.routeBackToLogin();
      }
    }
    catch (error) {
      let message = error instanceof Error ? error.message : 'Unknown error';
      this._notificationService.showErrorNotification(`Error during connecting with the hub. Error Information: ${message}`)
      this.routeBackToLogin();
    }
  }

  routeBackToLogin() {
    this._router.navigateByUrl('/login');
  }

  async onRequestGameClick() {
    let result: GameConfirmationResult = await this._gameConfirmDialogService.requestGameAsync(this._authService.userId,
       this.connectedPlayerList[this.selectedPLayerRow].playerId);
    switch (result) {
      case GameConfirmationResult.Rejected:
        this._notificationService.showNotification("The request player has rejected your game request", 3000);
        break;
      case GameConfirmationResult.Timedout:
        this._notificationService.showNotification("The request player has not accepted/rejected with a particular time", 3000);
        break;
      default:
        this._notificationService.showNotification("The request player has accepted your game request", 3000);
    }
  }
}
