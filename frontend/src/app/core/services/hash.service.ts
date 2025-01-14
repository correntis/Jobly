import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { Env } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export default class HashService {
  public hash(value: string): string {
    return CryptoJS.SHA256(value).toString(CryptoJS.enc.Hex);
  }

  public encrypt(value: string): string {
    return CryptoJS.AES.encrypt(value, Env.hashSecretKet).toString();
  }

  public decrypt(value: string): string {
    return CryptoJS.AES.encrypt(value, Env.hashSecretKet).toString();
  }
}
