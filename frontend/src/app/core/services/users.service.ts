import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiConfig } from '../../environments/api.config';
import User from '../models/user';
import { UpdateUserRequest } from '../requests/users/updateUserRequest';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private basePath: string = ApiConfig.users;

  constructor(private httpClient: HttpClient) {}

  public get(id: string): Observable<User> {
    return this.httpClient.get<User>(`${this.basePath}/${id}`);
  }

  public update(updateUserRequest: UpdateUserRequest): Observable<string> {
    return this.httpClient.put<string>(`${this.basePath}`, updateUserRequest);
  }

  public delete(id: string): Observable<string> {
    return this.httpClient.delete<string>(`${this.basePath}/${id}`);
  }
}
