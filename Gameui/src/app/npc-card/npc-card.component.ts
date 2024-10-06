import { Component } from '@angular/core';
import { Person } from '../person.interface';
import { DataService } from '../data.service';

@Component({
  selector: 'app-npc-card',
  standalone: true,
  imports: [],
  templateUrl: './npc-card.component.html',
  styleUrl: './npc-card.component.scss'
})
export class NpcCardComponent {
  person: Person|undefined;

  constructor(private dataService: DataService) {
    this.dataService.getCurrentPerson().subscribe((data) => {
      this.person = data;
    });
  }

}
