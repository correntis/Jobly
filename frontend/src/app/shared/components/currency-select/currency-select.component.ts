import { CommonModule } from '@angular/common';
import { Component, forwardRef, Input } from '@angular/core';
import {
  FormControl,
  NG_VALUE_ACCESSOR,
  ReactiveFormsModule,
} from '@angular/forms';
import { MatOptionModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import Currency from '../../../core/models/currency';
import { CurrenciesService } from '../../../core/services/currencies.service';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-currency-select',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatFormFieldModule,
    MatOptionModule,
    MatSelectModule,
  ],
  templateUrl: './currency-select.component.html',
})
export class CurrencySelectComponent {
  @Input() control: FormControl = new FormControl();

  currencies: Currency[] = [];

  constructor(private currenciesService: CurrenciesService) {
    this.currenciesService.get().subscribe({
      next: (currencies) => {
        this.currencies = currencies;
      },
      error: (err) => console.error(err),
    });
  }
}
