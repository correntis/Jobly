import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Observable, map, startWith } from 'rxjs';
import { VacanciesService } from '../../../core/services/vacancies.service';

@Component({
  selector: 'app-searchable-select',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './searchable-select.component.html',
  styleUrl: './searchable-select.component.css',
})
export class SearchableSelectComponent implements OnInit {
  @Input() options: string[] = [];
  @Input() placeholder: string = 'Выберите или введите значение';
  @Input() label: string = '';
  @Input() control: FormControl = new FormControl();
  @Input() allowCustom: boolean = true;
  @Input() loadFromApi: 'requirements' | 'skills' | 'technologies' | null = null;
  @Output() valueChange = new EventEmitter<string>();

  filteredOptions$!: Observable<string[]>;
  inputControl = new FormControl('');
  isOpen: boolean = false;
  selectedValue: string = '';
  isLoading: boolean = false;

  constructor(private vacanciesService: VacanciesService | null = null) {}

  ngOnInit() {
    // Загружаем данные с API, если указано
    if (this.loadFromApi && this.vacanciesService) {
      this.isLoading = true;
      this.loadOptionsFromApi();
    } else {
      this.initializeFilter();
    }
    // Если control уже имеет значение, устанавливаем его
    if (this.control.value) {
      this.selectedValue = this.control.value;
      this.inputControl.setValue(this.control.value);
    }

    // Подписываемся на изменения control
    this.control.valueChanges.subscribe((value) => {
      if (value !== this.selectedValue) {
        this.selectedValue = value || '';
        this.inputControl.setValue(value || '');
      }
    });

    // Фильтруем опции по введенному тексту
    this.initializeFilter();
  }

  private initializeFilter(): void {
    this.filteredOptions$ = this.inputControl.valueChanges.pipe(
      startWith(''),
      map((value) => this._filter(value || ''))
    );
  }

  private loadOptionsFromApi(): void {
    if (!this.vacanciesService) {
      this.isLoading = false;
      return;
    }

    let request: Observable<string[]>;

    switch (this.loadFromApi) {
      case 'requirements':
        request = this.vacanciesService.getDistinctRequirements();
        break;
      case 'skills':
        request = this.vacanciesService.getDistinctSkills();
        break;
      case 'technologies':
        request = this.vacanciesService.getDistinctTechnologies();
        break;
      default:
        this.isLoading = false;
        return;
    }

    request.subscribe({
      next: (options: string[]) => {
        this.options = options;
        this.isLoading = false;
        this.initializeFilter();
      },
      error: (err: HttpErrorResponse) => {
        console.error('Error loading options:', err);
        this.isLoading = false;
        this.initializeFilter();
      },
    });
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.options.filter((option) =>
      option.toLowerCase().includes(filterValue)
    );
  }

  onOptionSelected(value: string): void {
    this.selectedValue = value;
    this.inputControl.setValue(value);
    this.control.setValue(value);
    this.valueChange.emit(value);
    this.isOpen = false;
  }

  onInputEnter(): void {
    const value = this.inputControl.value?.trim();
    if (value) {
      // Если значение есть в списке, выбираем его
      if (this.options.includes(value)) {
        this.onOptionSelected(value);
      } else if (this.allowCustom) {
        // Если значения нет в списке и разрешены кастомные значения, добавляем его
        this.onOptionSelected(value);
      }
    }
  }

  onInputFocus(): void {
    this.isOpen = true;
  }

  onInputBlur(): void {
    // Не закрываем сразу, чтобы можно было кликнуть на опцию
    setTimeout(() => {
      this.isOpen = false;
    }, 200);
  }

  clear(): void {
    this.inputControl.setValue('');
    this.control.setValue('');
    this.selectedValue = '';
    this.valueChange.emit('');
  }
}

