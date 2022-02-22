import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit } from '@angular/core';
import { Route, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
model : any = {}

  constructor(public accountService: AccountService, private route: Router,
  private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  login (){
   this.accountService.login(this.model).subscribe(response=> {
    this.route.navigateByUrl("/members")
    this.toastr.success('Welcome dear to application')
   }, error=> {
      this.toastr.error(error.error);
   })
  }


  logout(){
    this.accountService.logout();
    this.route.navigateByUrl("/")
  }
}
