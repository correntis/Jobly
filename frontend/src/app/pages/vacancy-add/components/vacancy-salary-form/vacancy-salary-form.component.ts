import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import Currency from '../../../../core/models/currency';
import { CurrencySelectComponent } from '../../../../shared/components/currency-select/currency-select.component';
import { CurrenciesService } from './../../../../core/services/currencies.service';

@Component({
  selector: 'app-vacancy-salary-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    CurrencySelectComponent,
  ],
  templateUrl: './vacancy-salary-form.component.html',
})
export class VacancySalaryFormComponent implements OnInit {
  @Input() salaryForm!: FormGroup;

  isVisible: boolean = false;
  buttonIcon: 'add' | 'delete' = 'add';
  buttonColor: 'accent' | 'warn' = 'accent';

  currencies: Currency[] = [];

  get currency(): FormControl {
    return this.salaryForm.get('currency') as FormControl;
  }

  constructor(private currenciesService: CurrenciesService) {}

  ngOnInit() {
    this.loadCurrencies();
  }

  loadCurrencies() {
    this.currenciesService.get().subscribe({
      next: (currencies) => {
        this.currencies = currencies;
      },
      error: (err) => console.error(err),
    });
  }

  changeVisibility() {
    this.isVisible = !this.isVisible;

    if (this.isVisible) {
      this.buttonIcon = 'delete';
      this.buttonColor = 'warn';
    } else {
      this.buttonIcon = 'add';
      this.buttonColor = 'accent';
    }
  }
}
