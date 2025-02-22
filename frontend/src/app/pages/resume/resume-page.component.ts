import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import Resume from '../../core/models/resumes/resume';
import User from '../../core/models/user';
import { CompaniesService } from '../../core/services/companies.service';
import HashService from '../../core/services/hash.service';
import { UsersService } from '../../core/services/users.service';
import { EnvService } from '../../environments/environment';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { InteractionsService } from './../../core/services/interactions.service';
import { ResumesService } from './../../core/services/resumes.service';

@Component({
  selector: 'app-resume-page',
  standalone: true,
  imports: [CommonModule, HeaderComponent],
  templateUrl: './resume-page.component.html',
})
export class ResumePageComponent {
  user?: User;
  resume?: Resume;

  constructor(
    private activatedRoute: ActivatedRoute,
    private hashService: HashService,
    private usersService: UsersService,
    private resumesService: ResumesService,
    private companiesService: CompaniesService,
    private interactionsService: InteractionsService,
    private envService: EnvService
  ) {}

  ngOnInit() {
    this.loadRouteParams();
  }

  loadRouteParams(): void {
    this.activatedRoute.params.subscribe((params) => {
      const userId = this.hashService.decrypt(params['userId']);

      this.loadUser(userId);
      this.loadResume(userId);
    });
  }

  sendView(resumeId: string): void {
    if (this.envService.isCompany()) {
      this.companiesService.getByUser(this.envService.getUserId()).subscribe({
        next: (company) => {
          this.resumesService.viewResume(company.id, resumeId).subscribe({
            error: (err) => console.error(err),
          });
        },
        error: (err) => console.error(err),
      });
    }
  }

  loadUser(userId: string) {
    this.usersService.get(userId).subscribe({
      next: (user) => (this.user = user),
      error: (err) => console.error(err),
    });
  }

  loadResume(userId: string) {
    this.resumesService.getByUser(userId).subscribe({
      next: (resume) => {
        this.resume = resume;
        this.sendView(resume.id);
      },
      error: (err) => console.error(err),
    });
  }
}
