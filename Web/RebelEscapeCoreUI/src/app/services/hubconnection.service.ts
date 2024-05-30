import { Injectable } from '@angular/core';
import { HubCommunicationService } from './hubcommunication.service';
import { Observable, Subject } from 'rxjs';
import { PlayerInfo } from '../models/playerinfo.model';
import { AuthService } from './auth.service';
import { HubConnectionState } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class HubconnectionService {

  private refreshPlayerListSubject!: Subject<PlayerInfo[]>;
  public refreshPlayersList$: Observable<PlayerInfo[]>;
  constructor(private _hubCommunicationService: HubCommunicationService,
    private _authService: AuthService
  ) {
    this.refreshPlayerListSubject = new Subject<PlayerInfo[]>();
    this.refreshPlayersList$ = this.refreshPlayerListSubject.asObservable();
   }

  getConnectionId(): string {
    let connectionId = this._hubCommunicationService.getConnectionId();
    if (connectionId == null) {
      throw Error('Connection ID not available')
    }
    return connectionId;
  }

  initializeConnection() : Promise<boolean> {
    try {
      if (this._authService.apiToken === '') {
        return Promise.resolve(false);
      }
      this._hubCommunicationService.connectToHub(this._authService.apiToken, 'http://localhost:5208/connectionhub');
      return new Promise((resolve, reject) => {
        let hubState = this._hubCommunicationService.getConnectionState();
        if (hubState == undefined) {
          throw Error('Hub connection is null. This is not expecetd');
        }
        else if (hubState == HubConnectionState.Connected) {
          resolve(true);
          return;
        }
        this._hubCommunicationService.startConnection(() => {
          this.registerForPlayerListRefresh()
          resolve(true);
        }, (err: any) => {
          reject(false)
        }); 
      })           
    }
    catch (error){
      throw error;
    }
  }

  async getOnlinePlayersList() : Promise<PlayerInfo[]> {
    let playersList = await this._hubCommunicationService.callServerFunctionWithReturnValue<PlayerInfo[]>("GetOnlinePlayers", null);
    playersList = playersList.filter(player => player.playerId !== this._authService.userId)
    return playersList;
  }

  private registerForPlayerListRefresh() {
    this._hubCommunicationService.registerEvent("RefreshConnectedUserList", (connectedPlayers: PlayerInfo[]) => {
      connectedPlayers = connectedPlayers.filter(player => player.playerId !== this._authService.userId);
      this.refreshPlayerListSubject.next(connectedPlayers);
    });
  }
}
