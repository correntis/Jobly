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
import { Router, RouterModule } from '@angular/router';
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
    RouterModule,
  ],
  templateUrl: './registration-page.component.html',
})
export class RegistrationPageComponent {
  registrationForm: FormGroup;
  companyForm: FormGroup;
  companyImage: File | null = null;

  step: number = 1;
  tempUserId: string | null = null; // Временное хранение userId для компании

  hidePassword: boolean = true;
  hideConfirmPassword: boolean = true;

  choosedRole: Role | null = null;
  appRoles: Role[] = [
    {
      text: '🚀 Я ищу интересные карьерные возможности.',
      roleName: UserRoles.User,
    },
    {
      text: '🏢 Я ищу талантливых сотрудников для своей компании.',
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
      companyUnp: ['', [Validators.required, Validators.pattern(/^\d{9}$/)]],
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
    this.tempUserId = null; // Сбрасываем временный userId при возврате на первый шаг
  }

  performUserRegistration(isFullRegistration: boolean = true): Observable<string> {
    const { choosedRole, firstName, lastName, email, password } =
      this.registrationForm.value;

    const registrationRequest: RegistrationRequest = {
      firstName,
      lastName,
      email,
      password,
      roles: [choosedRole],
      isFullRegistration,
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
      companyUnp,
    } = this.companyForm.value;

    const addCompanyRequest: AddCompanyRequest = {
      userId: userId,
      name: companyName,
      type: companyType,
      phone: companyPhone,
      email: companyEmail,
      description: companyDescription,
      city: companyCity,
      unp: companyUnp,
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
    // Убеждаемся, что мы на первом шаге для отображения ошибки email
    this.step = 1;
    
    const emailControl = this.registrationForm.get('email');
    if (emailControl) {
      // Сохраняем существующие ошибки валидации, если они есть
      const currentErrors = emailControl.errors || {};
      emailControl.setErrors({ ...currentErrors, existingEmail: true });
      emailControl.markAsTouched();
      this.cdr.detectChanges();
    }
  }

  processUserRegistrationError(err: HttpErrorResponse) {
    // Убеждаемся, что мы на первом шаге для отображения ошибок
    this.step = 1;
    
    if (err.status === HttpStatusCode.BadRequest) {
      // Пытаемся получить ошибки валидации из ответа
      try {
        const errorDetails = err.error?.details || err.error;
        if (errorDetails) {
          let errors: any[] = [];
          
          // Если это массив ошибок валидации
          if (Array.isArray(errorDetails)) {
            errors = errorDetails;
          }
          // Если это строка с JSON
          else if (typeof errorDetails === 'string') {
            try {
              const parsed = JSON.parse(errorDetails);
              if (Array.isArray(parsed)) {
                errors = parsed;
              }
            } catch (parseError) {
              // Если не удалось распарсить, игнорируем
            }
          }
          
          // Обрабатываем все ошибки валидации
          errors.forEach((error: any) => {
            const propertyName = (error.propertyName || error.PropertyName || '').toLowerCase();
            const errorMessage = error.errorMessage || error.ErrorMessage || 'Ошибка валидации';
            
            // Маппинг имен полей с бэкенда на имена контролов формы
            const fieldMapping: { [key: string]: string } = {
              'firstname': 'firstName',
              'lastname': 'lastName',
              'email': 'email',
              'password': 'password',
            };

            const formControlName = fieldMapping[propertyName];
            if (formControlName) {
              const control = this.registrationForm.get(formControlName);
              if (control) {
                // Устанавливаем ошибку валидации
                const currentErrors = control.errors || {};
                control.setErrors({ ...currentErrors, serverError: true });
                control.markAsTouched();
              }
            }
          });
          
          this.cdr.detectChanges();
        }
      } catch (e) {
        console.error('Error parsing validation errors:', e);
      }
    }
  }

  processCompanyRegistrationError(err: HttpErrorResponse) {
    // Убеждаемся, что мы на втором шаге для отображения ошибок
    this.step = 2;
    
    if (err.status === HttpStatusCode.BadRequest) {
      // Пытаемся получить ошибки валидации из ответа
      try {
        const errorDetails = err.error?.details || err.error;
        if (errorDetails) {
          let errors: any[] = [];
          
          // Если это массив ошибок валидации
          if (Array.isArray(errorDetails)) {
            errors = errorDetails;
          }
          // Если это строка с JSON
          else if (typeof errorDetails === 'string') {
            try {
              const parsed = JSON.parse(errorDetails);
              if (Array.isArray(parsed)) {
                errors = parsed;
              }
            } catch (parseError) {
              // Если не удалось распарсить, игнорируем
            }
          }
          
          // Обрабатываем все ошибки валидации
          errors.forEach((error: any) => {
            const propertyName = (error.propertyName || error.PropertyName || '').toLowerCase();
            const errorMessage = error.errorMessage || error.ErrorMessage || 'Ошибка валидации';
            
            // Обработка ошибки УНП
            if (propertyName === 'unp') {
              this.processUnpValidationError(errorMessage);
            }
            // Обработка других полей компании
            else {
              this.processCompanyFieldError(propertyName, errorMessage);
            }
          });
        }
      } catch (e) {
        console.error('Error parsing validation errors:', e);
      }
    }
  }

  processCompanyFieldError(propertyName: string, message: string) {
    // Убеждаемся, что мы на втором шаге для отображения ошибки
    this.step = 2;
    
    // Маппинг имен полей с бэкенда на имена контролов формы и понятные сообщения
    const fieldMapping: { [key: string]: { controlName: string; errorMessage: string } } = {
      'name': { controlName: 'companyName', errorMessage: 'Название компании обязательно' },
      'type': { controlName: 'companyType', errorMessage: 'Тип компании обязателен' },
      'email': { controlName: 'companyEmail', errorMessage: 'Некорректный email' },
      'phone': { controlName: 'companyPhone', errorMessage: 'Некорректный номер телефона' },
      'city': { controlName: 'companyCity', errorMessage: 'Некорректное значение города' },
      'description': { controlName: 'companyDescription', errorMessage: 'Некорректное описание' },
    };

    const fieldInfo = fieldMapping[propertyName];
    if (fieldInfo) {
      const control = this.companyForm.get(fieldInfo.controlName);
      if (control) {
        control.setErrors({ serverError: true });
        control.markAllAsTouched();
        this.cdr.detectChanges();
      }
    }
  }

  processUnpValidationError(message: string) {
    // Убеждаемся, что мы на втором шаге для отображения ошибки
    this.step = 2;
    const unpControl = this.companyForm.get('companyUnp');
    if (unpControl) {
      unpControl.setErrors({ invalidUnp: true });
      unpControl.markAllAsTouched();
      this.cdr.detectChanges();
    }
  }

  register() {
    if (!this.registrationForm.valid) {
      this.registrationForm.markAllAsTouched();
      this.cdr.detectChanges();
      return;
    }

    const { choosedRole } = this.registrationForm.value;

    switch (choosedRole) {
      case UserRoles.User:
        {
          // Для обычного пользователя регистрация полная
          this.performUserRegistration(true).subscribe({
            next: (id) => {
              this.hashUserInformation(id, UserRoles.User);
              this.router.navigate(['/home']);
            },
            error: (err: HttpErrorResponse) => {
              // Убеждаемся, что мы на первом шаге для отображения ошибки
              this.step = 1;
              if (err.status === HttpStatusCode.Conflict) {
                this.processEmailConflict();
              } else if (err.status === HttpStatusCode.BadRequest) {
                this.processUserRegistrationError(err);
              }
            },
          });
        }
        break;
      case UserRoles.Company:
        {
          if (this.step === 1) {
            // На первом шаге создаем пользователя с неполной регистрацией
            this.performUserRegistration(false).subscribe({
              next: (userId) => {
                // Сохраняем userId для следующего шага
                this.tempUserId = userId;
                this.step = 2;
                this.cdr.detectChanges();
              },
              error: (err: HttpErrorResponse) => {
                // При ошибке остаемся на первом шаге
                this.step = 1;
                if (err.status === HttpStatusCode.Conflict) {
                  this.processEmailConflict();
                } else if (err.status === HttpStatusCode.BadRequest) {
                  this.processUserRegistrationError(err);
                }
              },
            });
            break;
          }
          
          // На втором шаге проверяем валидность формы компании перед отправкой
          if (!this.companyForm.valid) {
            this.companyForm.markAllAsTouched();
            this.cdr.detectChanges();
            return;
          }

          // Создаем компанию для уже созданного пользователя
          if (this.tempUserId) {
            const userId = this.tempUserId;
            this.performCompanyRegistration(userId).subscribe({
              next: () => {
                this.hashUserInformation(userId, UserRoles.Company);
                this.router.navigate(['/home']);
              },
              error: (err: HttpErrorResponse) => {
                // При ошибке остаемся на втором шаге для отображения ошибок
                this.step = 2;
                this.processCompanyRegistrationError(err);
              },
            });
          }
        }
        break;
    }
  }
}
