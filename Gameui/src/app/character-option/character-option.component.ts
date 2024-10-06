import { Component } from '@angular/core';
import { MatChipsModule } from '@angular/material/chips';
import { MatButton } from '@angular/material/button';
import { DataService } from '../data.service';
import { Person } from '../person.interface';
import { NgForOf, NgIf } from '@angular/common';

enum Sex {
  Female = 1,
  Male = 2,
}

enum Age  {
  None = 0,
  Infant,
  Child,
  Young,
  Adult,
  Mature,
  Old,
}

enum Race {
  Centaur =1,
  Dwarf,
  Elve,
  Ent,
  Fae,
  Goblin,
  Haflings,
  Hare,
  Human,
  Lizard,
  Mer,
  Minotaur,
  Nymph,
  Orc,
  Satyr,
  Thiefling,
}

@Component({
  selector: 'app-character-option',
  standalone: true,
  imports: [MatChipsModule, MatButton, NgForOf, NgIf],
  templateUrl: './character-option.component.html',
  styleUrl: './character-option.component.scss'
})
export class CharacterOptionComponent {
  characteroptions: Person[] =[];

  constructor(private dataService: DataService) { }

  ngOnInit(): void {  
    this.dataService.getPersonOption().subscribe(data => {
      this.characteroptions = data;
      for(let c of this.characteroptions)
      {
        c.chips = [Age[c.ageId],Sex[c.sexId],Race[c.raceId]];
      }
      console.log('Received data:', this.characteroptions);
    });    
  }

  talkClick(idIn: number): void {
    if(this.characteroptions != undefined  && idIn != undefined && this.characteroptions.length>0)
    {
      var index = this.characteroptions.findIndex( (element) => element.id == idIn );
      this.dataService.setCurrentPerson(this.characteroptions[index]);
    }
  }



  modetext(mode: number): string {
    if(mode==1) return "Carriage";
    if(mode==2) return "Mountaineering";
    if(mode==3) return "Traveling";
    if(mode==4) return "Riding";
    if(mode==5) return "Sailing";
    if(mode==6) return "Swiming";
    if(mode==7) return "Walking";

    return "?";
  }
}
