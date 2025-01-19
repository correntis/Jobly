import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { EnvParams } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export default class HashService {
  public hash(value: string): string {
    return CryptoJS.SHA256(value).toString(CryptoJS.enc.Hex);
  }

  public encrypt(value: string): string {
    return CryptoJS.AES.encrypt(value, EnvParams.hashSecretKet).toString();
  }

  public decrypt(value: string): string {
    const bytes = CryptoJS.AES.decrypt(value, EnvParams.hashSecretKet);
    return bytes.toString(CryptoJS.enc.Utf8);
  }
}
