import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
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



  constructor(private toastr: ToastrService, private router: Router) { }

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
      this.onlineUser$.pipe(take(1)).subscribe(usernames=> {
        this.onlineUsersSource.next([...usernames, username])
      })
    })

    this.hubConnectioin.on('UserIsOffline', username=> {
      this.onlineUser$.pipe(take(1)).subscribe(usernames=> {
        this.onlineUsersSource.next([...usernames.filter(x=> x !== username)])
      })
    })

    this.hubConnectioin.on('GetOnlineUsers', (username:string[])=> {
      this.onlineUsersSource.next(username);
    })

    this.hubConnectioin.on('NewMessageReceived', ({username, knownAs})=> {
       this.toastr.info(knownAs + " has sent you a new message!")
       .onTap
       .pipe(take(1))
       .subscribe(()=> this.router.navigateByUrl('/members/'+ username+ '?tab=3'));
    })
  }

  stopHubConnection(){
    this.hubConnectioin.stop().catch(error=> console.log(error));
  }
}
