import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
 members: Member[];
 pagination: Pagination;
 userParams: UserParams;
 user: User;
 genderList= [{value:'male', display:'Males'}, {value:'female', display:'Females'}];

  constructor(private memberSerivce: MembersService) {
        this.userParams = this.memberSerivce.getUserParams();
  }

  ngOnInit(): void {
    this.loadMember();
  }


   loadMember(){
     this.memberSerivce.setUserParams(this.userParams);
     this.memberSerivce.getMembers(this.userParams).subscribe(response=> {
       this.members = response.result;
       this.pagination = response.pagination;
     })
   }

   resetFilters(){
    this.userParams = this.memberSerivce.resetUserParams();
    this.loadMember();
   }
   
   pageChanged(event: any){
     this.userParams.pageNumber = event.page;
     this.memberSerivce.setUserParams(this.userParams);
     this.loadMember();
   }
}
