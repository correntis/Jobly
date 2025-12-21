import { CommonModule } from '@angular/common';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { HashedCookieService } from '../../../core/services/hashedCookie.service';
import { ToastService } from '../../../core/services/toast.service';
import { Faker } from '../../../core/utils/faker';
import { EnvParams } from '../../../environments/environment';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [
    MatIconModule,
    ReactiveFormsModule,
    CommonModule,
    MatFormFieldModule,
    MatRadioModule,
    FormsModule,
    MatInputModule,
    MatButtonModule,
    RouterModule,
  ],
  templateUrl: './login-page.component.html',
})
export class LoginPageComponent {
  loginForm: FormGroup;

  hidePassword: boolean = true;

  constructor(
    private authService: AuthService,
    private router: Router,
    private hashedCookieService: HashedCookieService,
    private toastService: ToastService,
    private fb: FormBuilder
  ) {
    this.loginForm = this.fb.group({
      email: ['admin@example.com', [Validators.required, Validators.email]],
      password: [
        Faker.generateValidPassword(),
        [
          Validators.required,
          Validators.minLength(6),
          Validators.pattern(
            /^(?=.*[A-Z])(?=.*\d)(?=.*[!%&])[A-Za-z\d!%&]{6,}$/
          ),
        ],
      ],
    });
  }

  processPasswordInvalid() {
    const passwordControl = this.loginForm.get('password');
    if (passwordControl) {
      passwordControl.setErrors({ invalidPassword: true });
    }
  }

  processEmailNotFound() {
    const emailControl = this.loginForm.get('email');
    if (emailControl) {
      emailControl.setErrors({ emailNotFound: true });
    }
  }

  hashUserInformation(id: string, roles: string[]) {
    this.hashedCookieService.set(
      EnvParams.UserIdCookieName,
      id,
      EnvParams.UserIdCookieExpiresDays
    );

    this.hashedCookieService.set(
      EnvParams.UserRoleCookieName,
      JSON.stringify([...roles]),
      EnvParams.UserRoleCookieExpiresDays
    );
  }

  login() {
    if (!this.loginForm.valid) {
      alert('Please fill out the form correctly');
      return;
    }

    const { email, password } = this.loginForm.value;

    this.authService.login(email, password).subscribe({
      next: (user) => {
        this.hashUserInformation(user.id, user.roles);
        this.toastService.success('Вы успешно вошли в систему!');
        this.router.navigate(['/home']);
      },
      error: (err: HttpErrorResponse) => {
        switch (err.status) {
          case HttpStatusCode.BadRequest:
            this.processPasswordInvalid();
            break;

          case HttpStatusCode.NotFound:
            this.processEmailNotFound();
            break;
        }
      },
    });
  }

  togglePasswordVisibility() {
    this.hidePassword = !this.hidePassword;
  }
}
