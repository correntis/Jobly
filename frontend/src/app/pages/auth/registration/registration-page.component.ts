import { CommonModule } from '@angular/common';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { ChangeDetectorRef, Component } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { UserRoles } from '../../../core/enums/userRoles';
import RegistrationRequest from '../../../core/requests/auth/registrationRequest';
import AddCompanyRequest from '../../../core/requests/companies/addCompanyRequest';
import { CompaniesService } from '../../../core/services/companies.service';
import { HashedCookieService } from '../../../core/services/hashedCookie.service';
import { Faker } from '../../../core/utils/faker';
import { EnvParams } from '../../../environments/environment';
import { AuthService } from './../../../core/services/auth.service';

type Role = {
  text: string;
  roleName: string;
};

@Component({
  selector: 'app-registration-page',
  standalone: true,
  imports: [
    MatIconModule,
    ReactiveFormsModule,
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatRadioModule,
    FormsModule,
  ],
  templateUrl: './registration-page.component.html',
})
export class RegistrationPageComponent {
  registrationForm: FormGroup;
  companyForm: FormGroup;
  companyImage: File | null = null;

  step: number = 1;

  hidePassword: boolean = true;
  hideConfirmPassword: boolean = true;

  choosedRole: Role | null = null;
  appRoles: Role[] = [
    {
      text: '🚀 I’m seeking exciting career opportunities.',
      roleName: UserRoles.User,
    },
    {
      text: '🏢 I’m looking to hire exceptional talent for my company.',
      roleName: UserRoles.Company,
    },
  ];

  constructor(
    private authService: AuthService,
    private companiesService: CompaniesService,
    private hashedCookieService: HashedCookieService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private fb: FormBuilder
  ) {
    this.registrationForm = this.fb.group({
      firstName: [Faker.generateRandomString(), Validators.required],
      lastName: [Faker.generateRandomString(), Validators.required],
      email: [
        // Faker.generateRandomEmail(),
        'admin@example.com',
        [(Validators.required, Validators.email)],
      ],
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
      confirmPassword: [
        Faker.generateValidPassword(),
        [Validators.required, this.confirmPasswordValidator.bind(this)],
      ],
      choosedRole: ['', Validators.required],
    });

    this.companyForm = this.fb.group({
      companyName: [Faker.generateRandomString(), Validators.required],
      companyType: [Faker.generateRandomString(), Validators.required],
      companyEmail: [Faker.generateRandomEmail(), Validators.email],
      companyPhone: ['', [Validators.pattern(/^\d{10}$/)]],
      companyCity: [''],
      companyDescription: [''],
    });
  }

  confirmPasswordValidator(
    control: AbstractControl
  ): { [key: string]: boolean } | null {
    const password = this.registrationForm?.get('password')?.value;
    const confirmPassword = control.value;

    if (password && confirmPassword && password !== confirmPassword) {
      return { mustMatch: true };
    }
    return null;
  }

  togglePasswordVisibility(): void {
    this.hidePassword = !this.hidePassword;
  }

  toggleConfirmPasswordVisibility(): void {
    this.hideConfirmPassword = !this.hideConfirmPassword;
  }

  onFileSelect(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      this.companyImage = input.files[0];
    }
  }

  backToFirstStep() {
    this.step = 1;
  }

  performUserRegistration(): Observable<string> {
    const { choosedRole, firstName, lastName, email, password } =
      this.registrationForm.value;

    const registrationRequest: RegistrationRequest = {
      firstName,
      lastName,
      email,
      password,
      roles: [choosedRole],
    };

    return this.authService.register(registrationRequest);
  }

  performCompanyRegistration(userId: string): Observable<string> {
    const {
      companyName,
      companyType,
      companyPhone,
      companyEmail,
      companyDescription,
      companyCity,
    } = this.companyForm.value;

    const addCompanyRequest: AddCompanyRequest = {
      userId: userId,
      name: companyName,
      type: companyType,
      phone: companyPhone,
      email: companyEmail,
      description: companyDescription,
      city: companyCity,
    };

    return this.companiesService.add(addCompanyRequest, this.companyImage);
  }

  hashUserInformation(id: string, role: string) {
    this.hashedCookieService.set(
      EnvParams.UserIdCookieName,
      id,
      EnvParams.UserIdCookieExpiresDays
    );

    this.hashedCookieService.set(
      EnvParams.UserRoleCookieName,
      JSON.stringify([role]),
      EnvParams.UserRoleCookieExpiresDays
    );
  }

  processEmailConflict() {
    const emailControl = this.registrationForm.get('email');
    if (emailControl) {
      emailControl.setErrors({ existingEmail: true });
      emailControl.markAllAsTouched();
      this.cdr.detectChanges();
    }
  }

  register() {
    if (!this.registrationForm.valid) {
      alert('Please fill out the form correctly');
      return;
    }

    const { choosedRole } = this.registrationForm.value;

    switch (choosedRole) {
      case UserRoles.User:
        {
          this.performUserRegistration().subscribe({
            next: (id) => {
              this.hashUserInformation(id, UserRoles.User);
              this.router.navigate(['/home']);
            },
            error: (err: HttpErrorResponse) => {
              if (err.status === HttpStatusCode.Conflict) {
                this.processEmailConflict();
              }
            },
          });
        }
        break;
      case UserRoles.Company:
        {
          if (this.step === 1) {
            this.step = 2;
            break;
          }
          this.backToFirstStep();

          this.performUserRegistration().subscribe({
            next: (userId) => {
              this.performCompanyRegistration(userId).subscribe({
                next: () => {
                  this.router.navigate(['/home']);
                },
                error: (err) => console.error(err),
              });
              this.hashUserInformation(userId, UserRoles.Company);
            },
            error: (err: HttpErrorResponse) => {
              if (err.status === HttpStatusCode.Conflict) {
                this.processEmailConflict();
              }
            },
          });
        }
        break;
    }
  }
}
