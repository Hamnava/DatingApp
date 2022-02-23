import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
 members : Member[];

  constructor(private memberSerivce: MembersService) { }

  ngOnInit(): void {
    this.loadMember();
  }


  loadMember(){
    this.memberSerivce.getMembers().subscribe(response=> {
      this.members = response;
    }, error=> {
      console.log(error);
    })
  }
}
