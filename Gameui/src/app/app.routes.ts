import { Routes } from '@angular/router';
import { PageIntroComponent } from './page-intro/page-intro.component';
import { PageInventoryComponent } from './page-inventory/page-inventory.component';
import { PageTalkComponent } from './page-talk/page-talk.component';
import { PageTravelComponent } from './page-travel/page-travel.component';
import { PageLocationComponent } from './page-location/page-location.component';

export const routes: Routes = [
    { path: '', redirectTo: '/page-intro', pathMatch: 'full' },
    { path: 'page-intro', component: PageIntroComponent },
    { path: 'page-inventory', component: PageInventoryComponent },
    { path: 'page-talk', component: PageTalkComponent },
    { path: 'page-travel', component: PageTravelComponent },
    { path: 'page-location', component: PageLocationComponent },
];
