import { LEADING_TRIVIA_CHARS } from '@angular/compiler/src/render3/view/template';
import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';


@Component({
  selector: 'app-roles-modals',
  templateUrl: './roles-modals.component.html',
  styleUrls: ['./roles-modals.component.css']
})
export class RolesModalsComponent implements OnInit {
 @Input() updateSelectdRoles = new EventEmitter();
 user: User;
 roles: any[];

  constructor(public bsModalRef: BsModalRef) { }



  ngOnInit(): void {
  }

  updateRoles(){
    this.updateSelectdRoles.emit(this.roles);
    this.bsModalRef.hide();
  }

}
