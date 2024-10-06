import { Component } from '@angular/core';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { DataService } from '../data.service';
import { Location } from '../location.interface';
import { NgForOf, NgIf } from '@angular/common';

@Component({
  selector: 'app-page-location',
  standalone: true,
  imports: [ScrollingModule, NgForOf, NgIf],
  templateUrl: './page-location.component.html',
  styleUrl: './page-location.component.scss'
})
export class PageLocationComponent {
  
  items = Array.from({ length: 9 }).map((_, i) => `Person #${i}`);
  defaultlocpic ="../assets/location.png";
  location: Location | null = null;

  constructor(private dataService: DataService) {
   }

  ngOnInit(): void {  
    this.dataService.getLocation().subscribe(data => {
      this.location = data;
      console.log('Received data:', this.location);
    });    
  }

  biomtext(biomid: number | null | undefined): string {
    if(biomid==1) return "Ancient Forest";
    if(biomid==2) return "Bank";
    if(biomid==3) return "Bare Rock";
    if(biomid==4) return "Bog";
    if(biomid==5) return "Boreal Forests";
    if(biomid==6) return "Cliffs";
    if(biomid==7) return "CoralReef";
    if(biomid==8) return "Crystal Forest";
    if(biomid==9) return "Desert";
    if(biomid==10) return "Fields";
    if(biomid==11) return "Glaciers";
    if(biomid==12) return "GrassSteppe";
    if(biomid==13) return "LavaPlain";
    if(biomid==14) return "Mangrove";
    if(biomid==15) return "Marsh";
    if(biomid==16) return "Mountain Tundra";
    if(biomid==17) return "Mushroom Forest";
    if(biomid==18) return "Ocean";
    if(biomid==19) return "Permafrost";
    if(biomid==20) return "Plains";
    if(biomid==21) return "Reeds Beach";
    if(biomid==22) return "River Delta";
    if(biomid==23) return "Sand Beach";
    if(biomid==24) return "Savannah";
    if(biomid==25) return "Sea";
    if(biomid==26) return "SeaIce";
    if(biomid==27) return "Seaweed Forest";
    if(biomid==28) return "Swamp";
    if(biomid==29) return "Temperate Coniferous Forests";
    if(biomid==30) return "Temperate Forests";
    if(biomid==31) return "Temperate Rain Forests";
    if(biomid==32) return "Tropical Cloud Forests";
    if(biomid==33) return "Tropical Dry Forests";
    if(biomid==34) return "Tropical Moist Forests";
    if(biomid==35) return "Tropical Rain Forests";
    if(biomid==36) return "Tundra";
    if(biomid==37) return "Vulcano";
    return "?";
  }
  
  geologytext(geologyid: number | null | undefined): string {
    if(geologyid==0) return "Minerals";
    if(geologyid==1) return "Dark";
    if(geologyid==2) return "Fertile";
    if(geologyid==3) return "Forest";
    if(geologyid==4) return "Old";
    if(geologyid==5) return "Magic";
    if(geologyid==6) return "Poor";
    if(geologyid==7) return "Plain";
    return "?";
  }

  typetext(locationTypeKey: number | null | undefined): string {
    if(locationTypeKey==0) return "?";
    if(locationTypeKey==1) return "AnimalTrail";
    if(locationTypeKey==2) return "CaveWithPaintings";
    if(locationTypeKey==3) return "DeadAnimal";
    if(locationTypeKey==4) return "DeadTree";
    if(locationTypeKey==5) return "EmptyCave";
    if(locationTypeKey==6) return "FallenTree";
    if(locationTypeKey==7) return "FieldOfFlowers";
    if(locationTypeKey==8) return "Glade";
    if(locationTypeKey==9) return "FarmHouse";
    if(locationTypeKey==10) return "HuntingCabin";
    if(locationTypeKey==11) return "ShepherdCamp";
    if(locationTypeKey==12) return "FishingHut";
    if(locationTypeKey==13) return "HermitHouse";
    if(locationTypeKey==14) return "PlainLocation";
    if(locationTypeKey==15) return "PlainSettlement";
    if(locationTypeKey==16) return "StrongholdCenter";
    if(locationTypeKey==17) return "Stronghold";
    return "?";
  }
}
