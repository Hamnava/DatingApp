import { HttpClient, HttpParams } from '@angular/common/http';
import { ThrowStmt } from '@angular/compiler';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
 baseUrl = environment.apiUrl;
 members: Member[] = [];
 memberCatche = new Map();
 user: User;
 userParams: UserParams;


  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user=> {
      this.user = user;
      this.userParams = new UserParams(user);
    })
   }

   getUserParams(){
     return this.userParams;
   }

   setUserParams(params: UserParams){
     this.userParams = params;
   }

   resetUserParams(){
     this.userParams = new UserParams(this.user);
     return this.userParams;
   }

  getMembers(userParams: UserParams){
    var response = this.memberCatche.get(Object.values(userParams).join('-'));
    if(response){
      return of(response);
    }
    let params =  this.getPaginationHeader(userParams.pageNumber, userParams.pageSize);
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return this.getPaginatedResult<Member[]>(this.baseUrl + 'user', params)
       .pipe(map(response=> {
         this.memberCatche.set(Object.values(userParams).join('-'), response);
         return response;
       }))
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
    // to get the specific member from array of catching we made in every query with key value pair

    const member = [...this.memberCatche.values()]
          .reduce((arr, elm)=> arr.concat(elm.result), [])
          .find((member: Member) => member.userName == username);

          if(member){
            return  of(member);
          }

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

  addLike(username: string){
    return this.http.post(this.baseUrl + 'likes/'+ username, {});
  }

  getLikes(predicate: string, pageNumber, pageSize){
    let params =this.getPaginationHeader(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    return this.getPaginatedResult<Partial<Member[]>>(this.baseUrl + 'likes', params);
  }
  
}
