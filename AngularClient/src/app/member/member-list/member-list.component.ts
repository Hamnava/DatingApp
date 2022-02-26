import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
 members: Member[];
 pagination: Pagination;
 pageNumber=1;
 pageSize= 5;

  constructor(private memberSerivce: MembersService) { }

  ngOnInit(): void {
    this.loadMember();
  }


   loadMember(){
     this.memberSerivce.getMembers(this.pageNumber,this.pageSize).subscribe(response=> {
       this.members = response.result;
       this.pagination = response.pagination;
     })
   }

   pageChanged(event: any){
     this.pageNumber = event.page;
     this.loadMember();
   }
}
