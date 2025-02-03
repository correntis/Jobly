import { CommonModule } from '@angular/common';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { ActivatedRoute } from '@angular/router';
import Resume from '../../../core/models/resumes/resume';
import User from '../../../core/models/user';
import AddResumeRequest from '../../../core/requests/resumes/addResumeRequest';
import UpdateResumeRequest from '../../../core/requests/resumes/updateResumeRequest';
import { UpdateUserRequest } from '../../../core/requests/users/updateUserRequest';
import HashService from '../../../core/services/hash.service';
import { ResumesService } from '../../../core/services/resumes.service';
import { UsersService } from '../../../core/services/users.service';
import { ResumeCertificationsFormComponent } from './components/resume-certifications-form/resume-certifications-form.component';
import { ResumeEducationFormComponent } from './components/resume-education-form/resume-education-form.component';
import { ResumeExperiencesFormComponent } from './components/resume-experiences-form/resume-experiences-form.component';
import { ResumeLanguageFormComponent } from './components/resume-language-form/resume-language-form.component';
import { ResumeProjectsFormComponent } from './components/resume-projects-form/resume-projects-form.component';

@Component({
  selector: 'app-user-account-page',
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
    MatDatepickerModule,
    MatNativeDateModule,
    ResumeLanguageFormComponent,
    ResumeCertificationsFormComponent,
    ResumeEducationFormComponent,
    ResumeExperiencesFormComponent,
    ResumeProjectsFormComponent,
  ],
  templateUrl: './user-account-page.component.html',
})
export class UserAccountPageComponent implements OnInit {
  userForm: FormGroup;
  resumeDescriptionForm: FormGroup;

  userId: string = '';
  user: User | undefined = undefined;

  resume: Resume | undefined = undefined;
  isNewResume: boolean = true;

  constructor(
    private actevatedRoute: ActivatedRoute,
    private usersService: UsersService,
    private resumesService: ResumesService,
    private hashService: HashService,
    private fb: FormBuilder,
    private cdRef: ChangeDetectorRef
  ) {
    this.userForm = this.fb.group({
      firstName: [''],
      lastName: [''],
      phone: [''],
    });

    this.resumeDescriptionForm = this.fb.group({
      title: ['', Validators.required],
      summary: ['', Validators.required],
      skills: this.fb.array([]),
      tags: this.fb.array([]),
    });
  }

  ngOnInit(): void {
    this.actevatedRoute.params.subscribe((params) => {
      this.userId = this.hashService.decrypt(params['userId']);
    });

    this.usersService.get(this.userId).subscribe({
      next: (user: User) => {
        this.user = user;
        this.patchUserForm();
      },
      error: (err) => console.error(err),
    });

    this.resumesService.getByUser(this.userId).subscribe({
      next: (resume) => {
        this.resume = resume;
        this.isNewResume = false;
        this.patchResumeForDescription();
        this.cdRef.detectChanges();
      },
      error: (err: HttpErrorResponse) => {
        if (err.status === HttpStatusCode.NotFound) {
          this.resume = undefined;
        }
      },
    });
  }

  get skills() {
    return this.resumeDescriptionForm.get('skills') as FormArray;
  }

  get tags(): FormArray {
    return this.resumeDescriptionForm.get('tags') as FormArray;
  }

  lastSkillIsEmpty(): boolean | undefined {
    return this.skills.controls[this.skills.length - 1]
      ?.get('name')
      ?.hasError('required');
  }

  lastTagIsEmpty(): boolean | undefined {
    return this.tags.controls[this.tags.length - 1]
      ?.get('name')
      ?.hasError('required');
  }

  addSkill(): void {
    if (this.skills.length > 0 && this.lastSkillIsEmpty()) {
      this.skills.markAllAsTouched();
      return;
    }

    const skillForm = this.fb.group({
      name: ['', Validators.required],
    });
    this.skills.push(skillForm);
    this.cdRef.detectChanges();
  }

  removeSkill(index: number): void {
    this.skills.removeAt(index);
    this.cdRef.detectChanges();
  }

  addTag(): void {
    if (this.tags.length > 0 && this.lastTagIsEmpty()) {
      this.tags.markAllAsTouched();
      return;
    }

    const tagForm = this.fb.group({
      name: ['', Validators.required],
    });
    this.tags.push(tagForm);
    this.cdRef.detectChanges();
  }

  removeTag(index: number): void {
    this.tags.removeAt(index);
    this.cdRef.detectChanges();
  }

  onDesctiptionSubmit(): void {
    if (this.isNewResume) {
      this.addResume();
    } else {
      this.saveResumeDescription();
    }
  }

  addResume(): void {
    if (!this.resumeDescriptionForm.valid) {
      alert('Pls fill out description correctly');
    }

    const { title, summary, skills, tags } = this.resumeDescriptionForm.value;
    const skillNames = skills.map((skill: { name: string }) => skill.name);
    const tagNames = tags.map((tag: { name: string }) => tag.name);

    const addResumeRequest: AddResumeRequest = {
      userId: this.userId,
      title,
      summary,
      skills: skillNames,
      tags: tagNames,
    };

    this.resumesService.add(addResumeRequest).subscribe({
      next: (resume) => {
        this.resume = resume;
        this.isNewResume = false;
      },
      error: (err) => console.error(err),
    });
  }

  saveResumeDescription(): void {
    const { title, summary, skills, tags } = this.resumeDescriptionForm.value;
    const skillNames = skills.map((skill: { name: string }) => skill.name);
    const tagNames = tags.map((tag: { name: string }) => tag.name);

    if (this.resume) {
      const updateResumeRequest: UpdateResumeRequest = {
        id: this.resume.id,
        userId: this.resume.userId,
        title,
        summary,
        tags: tagNames,
        skills: skillNames,
      };

      this.resumesService.update(updateResumeRequest).subscribe({
        next: (resume) => {
          this.resume = resume;
        },
        error: (err) => console.error(err),
      });
    }
  }

  patchResumeForDescription(): void {
    if (!this.resume) {
      return;
    }

    this.resumeDescriptionForm.patchValue({
      title: this.resume.title,
      summary: this.resume.summary,
    });

    this.skills.clear();
    this.tags.clear();

    this.resume.skills.forEach((skill) => {
      this.skills.push(this.fb.group({ name: [skill, Validators.required] }));
    });

    this.resume.tags.forEach((tag) => {
      this.tags.push(this.fb.group({ name: [tag, Validators.required] }));
    });
  }

  patchUserForm() {
    this.userForm.patchValue({
      firstName: this.user?.firstName,
      lastName: this.user?.lastName,
      phone: this.user?.phoneNumber,
    });
  }

  updateUser() {
    if (!this.user) {
      return;
    }

    const {
      firstName,
      lastName,
      phone,
    }: { firstName: string; lastName: string; phone: string } =
      this.userForm.value;

    const updateUserRequest: UpdateUserRequest = {
      id: this.user.id,
      firstName: !firstName || firstName.length === 0 ? null : firstName,
      lastName: !lastName || lastName.length === 0 ? null : lastName,
      phoneNumber: !phone || phone.length === 0 ? null : phone,
    };

    this.usersService.update(updateUserRequest).subscribe({
      error: (err) => console.error(err),
    });
  }
}
