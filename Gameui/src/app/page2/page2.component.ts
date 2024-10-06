import { Component } from '@angular/core';
import { TravelOptionComponent } from '../travel-option/travel-option.component';

@Component({
  selector: 'app-page2',
  standalone: true,
  imports: [TravelOptionComponent],
  templateUrl: './page2.component.html',
  styleUrl: './page2.component.scss'
})
export class Page2Component {

  constructor() { }

  
}
