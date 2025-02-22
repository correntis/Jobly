import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatOptionModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import Currency from '../../../core/models/currency';
import { CurrenciesService } from '../../../core/services/currencies.service';

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
export class CurrencySelectComponent implements OnInit {
  @Input() control: FormControl = new FormControl();

  currencies: Currency[] = [];

  constructor(private currenciesService: CurrenciesService) {}

  ngOnInit() {
    this.loadCurrencies();
  }

  loadCurrencies(): void {
    this.currenciesService.get().subscribe({
      next: (currencies: Currency[]) => {
        this.currencies = currencies;
      },
      error: (err: HttpErrorResponse) => console.error(err),
    });
  }
}
