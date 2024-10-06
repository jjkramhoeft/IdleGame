import { Component } from '@angular/core';
import { MatChipsModule } from '@angular/material/chips';
import { MatButton } from '@angular/material/button';
import { DataService } from '../data.service';
import { TravelOptionRecord } from '../traveloptionrecord.interface';
import { NgForOf, NgIf } from '@angular/common';
import { MatProgressBar } from '@angular/material/progress-bar';

@Component({
  selector: 'app-travel-option',
  standalone: true,
  imports: [MatChipsModule, MatButton, NgForOf, NgIf, MatProgressBar],
  templateUrl: './travel-option.component.html',
  styleUrl: './travel-option.component.scss'
})
export class TravelOptionComponent {
  traveloptions: TravelOptionRecord[] =[];
  traveling: boolean = false;

  constructor(private dataService: DataService) { }

  ngOnInit(): void {  
    this.dataService.getTravelOption().subscribe(data => {
      this.traveloptions = data;
      if(this.traveloptions.length==1)
      {
        this.traveling=true;
      }
      console.log('Received data:', this.traveloptions);
    });    
  }

  travelClick(travelUrl: URL): void {
    this.dataService.sendGetRequest(travelUrl.toString()).subscribe( data => {
      this.traveling = data;
    });
    window.location.reload();
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
