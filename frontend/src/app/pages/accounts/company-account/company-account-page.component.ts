import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { ActivatedRoute, Router } from '@angular/router';
import Company from '../../../core/models/company';
import User from '../../../core/models/user';
import UpdateCompanyRequest from '../../../core/requests/companies/updateCompanyRequest';
import { CompaniesService } from '../../../core/services/companies.service';
import HashService from '../../../core/services/hash.service';
import { UsersService } from '../../../core/services/users.service';
import { ApiConfig } from '../../../environments/api.config';
import { HeaderComponent } from '../../../shared/components/header/header.component';

@Component({
  selector: 'app-company-account-page',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatIconModule,
    CommonModule,
    MatInputModule,
    MatButtonModule,
    FormsModule,
    HeaderComponent,
  ],
  templateUrl: './company-account-page.component.html',
})
export class CompanyAccountPageComponent implements OnInit {
  userId: string = '';

  user: User | undefined = undefined;
  company: Company | undefined = undefined;

  companyForm: FormGroup;

  image: File | null = null;
  imageName: string = '';
  imageLocalUrl: string | ArrayBuffer | null = null;
  imageSrc: string = '';

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private usersService: UsersService,
    private companiesService: CompaniesService,
    private hashService: HashService
  ) {
    this.companyForm = this.fb.group({
      name: [''],
      description: [''],
      city: [''],
      address: [''],
      email: [''],
      phone: [''],
      webSite: [''],
      type: [''],
    });
  }

  ngOnInit(): void {
    this.loadRouteParams();
    this.loadUser();
  }

  loadRouteParams(): void {
    this.activatedRoute.params.subscribe((params) => {
      this.userId = this.hashService.decrypt(params['userId']);
    });
  }

  loadUser(): void {
    this.usersService.get(this.userId).subscribe({
      next: (user: User) => {
        this.user = user;
        this.loadCompanyForUser(user.id);
      },
      error: (err) => console.error(err),
    });
  }

  loadCompanyForUser(userId: string): void {
    this.companiesService.getByUser(userId).subscribe({
      next: (company: Company) => {
        this.company = company;
        this.imageSrc = `${ApiConfig.resources}/${company.logoPath}`;
        this.patchCompanyForm();
      },
      error: (err) => console.error(err),
    });
  }

  patchCompanyForm(): void {
    if (this.company) {
      this.companyForm = this.fb.group({
        name: [this.company.name],
        description: [this.company.description],
        city: [this.company.city],
        address: [this.company.address],
        email: [this.company.email],
        phone: [this.company.phone],
        webSite: [this.company.webSite],
        type: [this.company.type],
      });
    }
  }

  updateCompany(): void {
    if (!this.companyForm.valid) {
      this.companyForm.markAllAsTouched();
      return;
    }

    if (this.company) {
      const companyUpdateRequest: UpdateCompanyRequest = {
        id: this.company.id,
        ...this.companyForm.value,
      };

      this.companiesService.update(companyUpdateRequest, this.image).subscribe({
        error: (err) => console.error(err),
      });
    }
  }

  async onFileSelected(event: Event): Promise<void> {
    const fileInput = event.target as HTMLInputElement;

    if (fileInput.files && fileInput.files.length > 0) {
      const file: File = fileInput.files[0];

      this.image = file;
      this.imageName = file.name;

      const reader = new FileReader();

      reader.onload = () => {
        this.imageLocalUrl = reader.result;
      };

      reader.readAsDataURL(this.image);
    }
  }

  goToVacancyFormPage(): void {
    if (this.company?.id) {
      const hashedCompanyId = this.hashService.encrypt(this.company.id);

      this.router.navigate(['account/company', hashedCompanyId, 'vacancy']);
    }
  }

  goToCompanyPage(): void {
    if (this.company?.id) {
      const hashedCompanyId = this.hashService.encrypt(this.company.id);

      this.router.navigate(['company', hashedCompanyId]);
    }
  }
}
