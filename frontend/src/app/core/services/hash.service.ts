import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { EnvParams } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export default class HashService {
  public hash(value: string): string {
    const hashed = CryptoJS.SHA256(value).toString(CryptoJS.enc.Base64);
    return this.toUrlSafeBase64(hashed);
  }

  public encrypt(value: string): string {
    const encrypted = CryptoJS.AES.encrypt(
      value,
      EnvParams.hashSecretKet
    ).toString();
    return this.toUrlSafeBase64(encrypted);
  }

  public decrypt(value: string): string {
    const base64Value = this.fromUrlSafeBase64(value);
    const bytes = CryptoJS.AES.decrypt(base64Value, EnvParams.hashSecretKet);
    return bytes.toString(CryptoJS.enc.Utf8);
  }

  private toUrlSafeBase64(value: string): string {
    return value.replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
  }

  private fromUrlSafeBase64(value: string): string {
    return (
      value.replace(/-/g, '+').replace(/_/g, '/') +
      '='.repeat((4 - (value.length % 4)) % 4)
    );
  }
}
