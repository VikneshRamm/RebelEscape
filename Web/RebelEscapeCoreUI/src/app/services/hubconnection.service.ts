import { Injectable } from '@angular/core';
import { HubCommunicationService } from './hubcommunication.service';
import { Subject } from 'rxjs';
import { PlayerInfo } from '../models/playerinfo.model';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class HubconnectionService {

  public refreshPlayerListSubject!: Subject<PlayerInfo[]>;
  constructor(private _hubCommunicationService: HubCommunicationService,
    private _authService: AuthService
  ) {
    this.refreshPlayerListSubject = new Subject<PlayerInfo[]>();
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

  private registerForPlayerListRefresh() {
    this._hubCommunicationService.registerEvent("RefreshConnectedUserList", (connectedPlayers: PlayerInfo[]) => {
      connectedPlayers = connectedPlayers.filter(player => player.playerId !== this._authService.userId);
      this.refreshPlayerListSubject.next(connectedPlayers);
    });
  }
}
