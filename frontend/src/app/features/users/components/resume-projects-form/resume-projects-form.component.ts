import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  Input,
  SimpleChanges,
} from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import Project from '../../../../core/models/resumes/project';
import { ResumesService } from '../../../../core/services/resumes.service';

@Component({
  selector: 'app-resume-projects-form',
  standalone: true,
  imports: [
    MatIconModule,
    ReactiveFormsModule,
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  ],
  templateUrl: './resume-projects-form.component.html',
})
export class ResumeProjectsFormComponent {
  projectsForm: FormGroup;

  @Input() resumeProjects: Project[] | undefined;
  @Input() resumeId: string | undefined;

  constructor(
    private resumesService: ResumesService,
    private fb: FormBuilder,
    private cdRef: ChangeDetectorRef
  ) {
    this.projectsForm = this.fb.group({
      projects: this.fb.array([]),
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.resumeProjects) {
      return;
    }

    if (changes['resumeProjects'] && this.resumeProjects.length) {
      this.projects.clear();
      this.patchProjects();
    }
  }

  get projects(): FormArray {
    return this.projectsForm.get('projects') as FormArray;
  }

  getTechnologies(projectIndex: number): FormArray {
    return this.projects.at(projectIndex).get('technologies') as FormArray;
  }

  lastProjectIsEmpty(): boolean | undefined {
    const lastControl = this.projects.controls[this.projects.length - 1];

    if (lastControl) {
      const nameEmpty = lastControl.get('name')?.hasError('required');
      const descriptionEmpty = lastControl
        .get('description')
        ?.hasError('required');
      const linkEmpty = lastControl.get('link')?.hasError('required');

      return (
        nameEmpty ||
        descriptionEmpty ||
        linkEmpty ||
        this.lastTechnologyIsEmpty(this.projects.length - 1)
      );
    }

    return undefined;
  }

  lastTechnologyIsEmpty(index: number): boolean | undefined {
    const technologiesArray = this.getTechnologies(index);
    return technologiesArray.controls.some((control) =>
      control.hasError('required')
    );
  }

  addProject(): void {
    if (this.projects.length > 0 && this.lastProjectIsEmpty()) {
      this.projects.markAllAsTouched();
      return;
    }

    const projectsForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      link: ['', Validators.required],
      technologies: this.fb.array([]),
    });
    this.projects.push(projectsForm);
    this.cdRef.detectChanges();
  }

  addTechnology(projectIndex: number): void {
    if (this.lastTechnologyIsEmpty(projectIndex)) {
      this.projects.markAllAsTouched();
      return;
    }

    const techArray = this.getTechnologies(projectIndex);
    techArray.push(this.fb.control('', Validators.required));
  }

  removeProject(index: number): void {
    this.projects.removeAt(index);
    this.cdRef.detectChanges();
  }

  removeTechnology(projectIndex: number, techIndex: number): void {
    const techArray = this.getTechnologies(projectIndex);
    techArray.removeAt(techIndex);
  }

  patchProjects(): void {
    if (!this.resumeProjects) {
      return;
    }

    this.resumeProjects.forEach((project) => {
      const projectForm = this.fb.group({
        name: [project.name, Validators.required],
        description: [project.description, Validators.required],
        link: [project.link, Validators.required],
        technologies: this.fb.array(
          project.technologies.map((tech) =>
            this.fb.control(tech, Validators.required)
          )
        ),
      });

      this.projects.push(projectForm);
    });
  }

  saveProjects() {
    if (!this.projectsForm.valid) {
      alert('Please fill out projects correctly.');
      return;
    }

    const { projects }: { projects: Project[] } = this.projectsForm.value;

    if (this.resumeProjects && this.resumeId) {
      this.resumesService.updateProjects(this.resumeId, projects).subscribe({
        error: (err) => console.error(err),
      });
    }
  }
}
