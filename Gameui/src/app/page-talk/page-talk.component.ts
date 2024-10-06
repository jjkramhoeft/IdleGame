import { Component } from '@angular/core';
import { CharacterOptionComponent } from '../character-option/character-option.component';
import { NpcCardComponent } from '../npc-card/npc-card.component';

@Component({
  selector: 'app-page-talk',
  standalone: true,
  imports: [CharacterOptionComponent, NpcCardComponent],
  templateUrl: './page-talk.component.html',
  styleUrl: './page-talk.component.scss'
})
export class PageTalkComponent {
  constructor() { }
}
