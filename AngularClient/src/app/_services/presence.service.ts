import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnectioin: HubConnection;
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUser$ = this.onlineUsersSource.asObservable();



  constructor(private toastr: ToastrService) { }

  createHubConnection(user:User){
    this.hubConnectioin = new HubConnectionBuilder()
    .withUrl(this.hubUrl + 'presence', {
          accessTokenFactory:()=> user.token
    })
    .withAutomaticReconnect()
    .build();

    this.hubConnectioin.start()
         .catch(error=> console.log(error));

    this.hubConnectioin.on('UserIsOnline',username => {
      this.toastr.info(username + ' has connected');
    })

    this.hubConnectioin.on('UserIsOffline', username=> {
      this.toastr.warning(username, ' has disconnected');
    })

    this.hubConnectioin.on('GetOnlineUsers', (username:string[])=> {
      this.onlineUsersSource.next(username);
    })
  }

  stopHubConnection(){
    this.hubConnectioin.stop().catch(error=> console.log(error));
  }
}
