import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class HubCommunicationService {

  private connection: signalR.HubConnection | undefined;
  constructor() { }

  getConnectionId(): string | null {
    if (this.connection) {
      return this.connection.connectionId;
    }
    else {
      throw new Error('Connection to hub is not made');
    }
  }

  connectToHub(accessToken: string, hubUrl: string) {
    if (this.connection !== undefined && this.connection.connectionId
      !== null) {
      return;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Information)
      .withUrl(hubUrl, { accessTokenFactory: () => accessToken })
      .build();
  }

  startConnection(
    successCallBack:(args: any) => any,
    failureCallback:(args: any) => any) {
    if (this.connection) {
      this.connection.start().then(successCallBack).catch(failureCallback);
    }
    else {
      throw new Error('Connection to hub is not made');
    }
    
  }

  registerEvent(eventName: string, callback:(args: any) => any): void  {
    if (this.connection) {
      this.connection.on(eventName, callback);
    }
    else {
      throw new Error('Connection to hub is not made');
    }
  }

  async callServerFunction(functionName: string, parameters: any) {
    if (this.connection) {
      return await this.connection.send(functionName, parameters);
    }
    else {
      throw new Error('Connection to hub is not made');
    }    
  }

  async callServerFunctionWithReturnValue<T>(functionName: string, parameters: any) {
    if (this.connection) {
      return this.connection.invoke<T>(functionName, parameters);
    }
    else {
      throw new Error('Connection to hub is not made');
    }    
  }

  getCurrentConnectionId(): string | null | undefined {
    if (this.connection) {
      return this.connection.connectionId;
    }
    else {
      throw new Error('Connection to hub is not made');
    }    
  }
}
