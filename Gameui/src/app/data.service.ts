import { Injectable } from '@angular/core';
import { TravelOptionRecord } from './traveloptionrecord.interface';
import { Person } from './person.interface';
import { Location } from './location.interface';
import { HttpClient } from '@angular/common/http';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  private debug = false;
  private worldApiUrl = 'https://localhost:7210';
  
  constructor(private http: HttpClient) { }

  getTravelOption() {
    if(this.debug)
      return this.http.get<TravelOptionRecord[]>(`../assets/debug/travel-options.json`);
    else
      return this.http.get<TravelOptionRecord[]>(`${this.worldApiUrl}/traveloptions/1`);
  }

  getPersonOption() {
    if(this.debug)
      return this.http.get<Person[]>(`../assets/debug/persons-options.json`);
    else
      return this.http.get<Person[]>(`${this.worldApiUrl}/persons/location/current/1`);
  }

  getLocation() {
    if(this.debug)
      return this.http.get<Location>(`../assets/debug/location.json`);
    else
      return this.http.get<Location>(`${this.worldApiUrl}/location/current/1`);
  }

  
  private debugTravelling: Subject<boolean> = new Subject<boolean>();
  sendGetRequest(url2get: string) {
    if(this.debug)
    {
      this.debugTravelling.next(false);
      return this.debugTravelling.asObservable();
    }
    else
      return this.http.get<boolean>(url2get);
  }

  private currentPerson: Subject<Person> = new Subject<Person>();
  setCurrentPerson(data: Person){
    this.currentPerson.next(data);
  }
  getCurrentPerson(){
    return this.currentPerson.asObservable();
  }
}
