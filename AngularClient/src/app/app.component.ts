import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';
import { PresenceService } from './_services/presence.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'The Dating App My new project in Angular 12';
  users : any;
  constructor( private accountService: AccountService, private presencService: PresenceService){}

  ngOnInit() {
    this.setCurrentUser();
  }

 setCurrentUser(){
   const user: User = JSON.parse(localStorage.getItem('user'));
   if(user){
    this.accountService.setCurrentUser(user);
    this.presencService.createHubConnection(user);
   }
 }


}
