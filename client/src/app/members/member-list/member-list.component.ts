import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from './../../models/member';
import { MemberService } from './../../services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  members$: Observable<Member[]>;
  constructor(private memberService: MemberService) {}

  ngOnInit(): void {
    console.log('Calling on init');
    this.members$ = this.memberService.getMembers();
  }
}
