import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import User from '../models/user';
import { ApiConfig } from '../../environments/api.config';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private readonly basePath: string = ApiConfig.users;

  constructor(private httpClient: HttpClient) {}

  public get(id: string): Observable<User> {
    return this.httpClient.get<User>(`${this.basePath}/${id}`);
  }

  public update(user: User): Observable<string> {
    const userToUpdate = {
      id: user.id,
      firstName: user.firstName,
      lastName: user.lastName,
      phoneNumber: user.phoneNumber,
    };

    return this.httpClient.put<string>(`${this.basePath}`, userToUpdate);
  }

  public delete(id: string): Observable<string> {
    return this.httpClient.delete<string>(`${this.basePath}/${id}`);
  }
}
