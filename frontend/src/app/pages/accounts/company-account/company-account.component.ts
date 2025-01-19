import { Component } from '@angular/core';
import User from '../../../core/models/user';
import { ActivatedRoute } from '@angular/router';
import { UsersService } from '../../../core/services/users.service';
import { CompaniesService } from '../../../core/services/companies.service';
import Company from '../../../core/models/company';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import UpdateCompanyRequest from '../../../core/requests/companies/updateCompanyRequest';
import { ApiConfig } from '../../../environments/api.config';

@Component({
  selector: 'app-company-account',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatIconModule,
    CommonModule,
    MatInputModule,
    MatButtonModule,
    FormsModule,
  ],
  templateUrl: './company-account.component.html',
})
export class CompanyAccountComponent {
  userId: string = '';
  user: User | undefined = undefined;
  company: Company | undefined = undefined;

  companyForm: FormGroup;
  image: File | null = null;
  imageLocalUrl: string | ArrayBuffer | null = null;
  fileName: string = '';

  imageSrc: string = '';

  constructor(
    private actevatedRoute: ActivatedRoute,
    private usersService: UsersService,
    private companiesService: CompaniesService,
    private fb: FormBuilder
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
    this.actevatedRoute.params.subscribe((params) => {
      this.userId = params['userId'];
    });

    this.usersService.get(this.userId).subscribe({
      next: (user: User) => {
        this.user = user;
      },
      error: (err) => console.error(err),
    });

    this.companiesService.getByUser(this.userId).subscribe({
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

  updateCompany() {
    if (!this.companyForm.valid) {
      alert('pls fill out form correctly');
      this.companyForm.markAllAsTouched();
      return;
    }

    if (this.company) {
      const { name, description, city, address, email, phone, webSite, type } =
        this.companyForm.value;

      const companyUpdateRequest: UpdateCompanyRequest = {
        id: this.company?.id,
        name,
        description,
        city,
        address,
        email,
        phone,
        webSite,
        type,
      };

      this.companiesService.update(companyUpdateRequest, this.image).subscribe({
        next: (id) => console.log('updated ', id),
        error: (err) => console.error(err),
      });
    }
  }

  async onFileSelected(event: Event): Promise<void> {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput.files && fileInput.files.length > 0) {
      const file: File = fileInput.files[0];
      this.image = file;
      this.fileName = file.name;

      const reader = new FileReader();
      reader.onload = () => {
        this.imageLocalUrl = reader.result;
      };
      reader.readAsDataURL(this.image);
    }
  }
}
