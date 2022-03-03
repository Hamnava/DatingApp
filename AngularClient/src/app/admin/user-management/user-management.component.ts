import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { RolesModalsComponent } from 'src/app/modals/roles-modals/roles-modals.component';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: Partial<User[]>;
  bsModalRef: BsModalRef;

  constructor(private adminService: AdminService, private modalService: BsModalService) { }

  ngOnInit(): void {
    this.getUserWithRoles();
  }

  getUserWithRoles(){
    this.adminService.getUserWithRoles().subscribe(user=> {
      this.users = user;
    })
  }

  openRolesModal(user: User){
    const config = {
       class: 'modal-dialog-centered',
       initialState: {
         user,
         roles: this.getRolesArray(user)
       }
    }
    this.bsModalRef = this.modalService.show(RolesModalsComponent, config);
    this.bsModalRef.content.updateSelectdRoles.subscribe(values=> {
      const rolesToUpdate = {
        roles: [...values.filter(el=> el.checked == true).map(el=> el.name)]
      };
      if(rolesToUpdate){
        this.adminService.updateUserRoles(user.username, rolesToUpdate.roles).subscribe(()=> {
          user.roles = [...rolesToUpdate.roles]
        })
      }
    })
  }

  private getRolesArray(user){
      const roles = [];
      const userRoles = user.roles;
      const availableRoles: any[] = [
        {name: 'Admin', value : 'Admin'},
        {name: 'Moderator', value : 'Moderator'},
        {name: 'Member', value : 'Member'}
      ];
      availableRoles.forEach(role=> {
        let isMatch = false;
        for(const userRole of userRoles){
          if(role.name === userRole){
            isMatch = true;
            role.checked = true;
            roles.push(role);
            break;
          }
        }
        if(!isMatch){
          role.checked = false;
          roles.push(role);
        }
      })
      return roles;
  }

}
