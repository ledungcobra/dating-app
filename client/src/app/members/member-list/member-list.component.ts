import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from 'src/app/models/user';
import { Member } from './../../models/member';
import { Pagination } from './../../models/pagination';
import { UserParams } from './../../models/userParams';
import { AccountService } from './../../services/account.service';
import { MemberService } from './../../services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  members: Member[];
  pagination: Pagination;
  userParams: UserParams;
  user: User;
  genderList = [
    { value: 'male', display: 'Male' },
    { value: 'female', display: 'Female' },
  ];
  constructor(private memberService: MemberService, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => (this.user = user));
    this.userParams = new UserParams(this.user);
  }

  ngOnInit(): void {
    console.log(this.userParams);

    this.loadMembers();
  }

  loadMembers() {
    this.memberService.getMembers(this.userParams).subscribe((response) => {
      this.members = response.result;
      this.pagination = response.pagination;
    });
  }

  pageChanged(event: any) {
    this.userParams.pageNumber = event.page;
    this.loadMembers();
  }

  resetFilter() {
    this.userParams = new UserParams(this.user);
  }
}
