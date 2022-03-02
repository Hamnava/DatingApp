import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Messages } from '../_models/message';
import { getPaginatedResult, getPaginationHeader } from './PaginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
 baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getMessages(pageNumber, pageSize, container){
     let params = getPaginationHeader(pageNumber, pageSize);
     params = params.append('Container', container);

     return getPaginatedResult<Messages[]>(this.baseUrl + 'messages', params, this.http);
  }

  getMessageThread(username: string){
    return this.http.get<Messages[]>(this.baseUrl + 'messages/thread/' + username);
  }

  sendMessage(username:string, content:string){
    return this.http.post<Messages>(this.baseUrl + 'messages', {recipeintUsername: username, content})
  }
}
