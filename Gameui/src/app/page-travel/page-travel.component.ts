import { Component } from '@angular/core';
import { TravelOptionComponent } from '../travel-option/travel-option.component';

@Component({
  selector: 'app-page-travel',
  standalone: true,
  imports: [TravelOptionComponent],
  templateUrl: './page-travel.component.html',
  styleUrl: './page-travel.component.scss'
})
export class PageTravelComponent {
  constructor() { }
}
