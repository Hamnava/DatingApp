import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'The Dating App My new project in Angular 12';
  users : any;
  constructor(private http: HttpClient){}

  ngOnInit() {
    this.getUser();
  }

  getUser(){
    this.http.get('https://localhost:44372/api/user').subscribe(response=> {
      this.users = response
    }, error=> {
      console.log(error);
    })
  }
}
