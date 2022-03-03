import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Messages } from '../_models/message';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeader } from './PaginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
 baseUrl = environment.apiUrl;
 hubUrl = environment.hubUrl;
 hubConnection: HubConnection;
 private messageThreadSource= new BehaviorSubject<Messages[]>([]);
 messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient) { }


  createHubConnection(user: User, otherUsername:string){
      this.hubConnection = new HubConnectionBuilder()
              .withUrl(this.hubUrl + 'message?user='+ otherUsername, {
                accessTokenFactory: ()=> user.token
              })
              .withAutomaticReconnect()
              .build();

      this.hubConnection.start().catch(error=> console.log(error));

      this.hubConnection.on('ReceiveMessageThread', messages=> {
        this.messageThreadSource.next(messages);
      });

      this.hubConnection.on('NewMessage', message => {
        this.messageThread$.pipe(take(1)).subscribe(messages=> {
          this.messageThreadSource.next([...messages, message]);
        })
      })
  }

  stopHubConnection(){
    if(this.hubConnection){
      this.hubConnection.stop();
    }
  }

  getMessages(pageNumber, pageSize, container){
     let params = getPaginationHeader(pageNumber, pageSize);
     params = params.append('Container', container);

     return getPaginatedResult<Messages[]>(this.baseUrl + 'messages', params, this.http);
  }

  getMessageThread(username: string){
    return this.http.get<Messages[]>(this.baseUrl + 'messages/thread/' + username);
  }

  sendMessage(username:string, content:string){
    return this.hubConnection.invoke('SendMessage', {recipeintUsername: username, content})
        .catch(error=> console.log(error))
  }

  deleteMessage(id: number){
    return this.http.delete(this.baseUrl + 'messages/'+ id)
  }
}
