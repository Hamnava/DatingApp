import { HttpClient, HttpParams } from '@angular/common/http';
import { ThrowStmt } from '@angular/compiler';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
 baseUrl = environment.apiUrl;
 members: Member[] = [];



  constructor(private http: HttpClient) { }


  getMembers(userParams: UserParams){

    let params =  this.getPaginationHeader(userParams.pageNumber, userParams.pageSize);
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return this.getPaginatedResult<Member[]>(this.baseUrl + 'user', params);
  }


  private getPaginatedResult<T>(url, params) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
  }

  private getPaginationHeader(pageNumber:number, pageSize:number){
         let params = new HttpParams();
         params =  params.append('pageNumber',  pageNumber);
         params =  params.append('pageSize', pageSize);

         return params;
  }

  getMember(username: string){
    const member = this.members.find(x=> x.userName == username);
    if(member !== undefined) return of(member);
    return this.http.get<Member>(this.baseUrl + 'user/'+ username );
  }

  updateMember(member:Member){
    return this.http.put(this.baseUrl + 'user', member).pipe(
      map(()=> {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    )
  }

  setMainPhoto(photoId: number){
      return this.http.put(this.baseUrl + 'user/set-main-photo/'+ photoId, {});
  }

  deletePhoto(photoId:number){
    return this.http.delete(this.baseUrl + 'user/delete-photo/' + photoId);
  }
}
