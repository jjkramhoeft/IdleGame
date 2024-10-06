import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { MatButton } from '@angular/material/button';
import { Page1Component } from './page1/page1.component';
import { Page2Component } from './page2/page2.component';
import { MatIcon } from '@angular/material/icon';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatChipsModule } from '@angular/material/chips';
import { DomSanitizer } from '@angular/platform-browser';
import { MatIconRegistry, MatIconModule } from '@angular/material/icon';
import { routes } from './app.routes';


const PIN_ICON = `
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16">
  <path d="M8 16s6-5.686 6-10A6 6 0 0 0 2 6c0 4.314 6 10 6 10m0-7a3 3 0 1 1 0-6 3 3 0 0 1 0 6"/>
</svg>
`;
const TALK_ICON = `
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16">
  <path d="M11.176 14.429c-2.665 0-4.826-1.8-4.826-4.018 0-2.22 2.159-4.02 4.824-4.02S16 8.191 16 10.411c0 1.21-.65 2.301-1.666 3.036a.32.32 0 0 0-.12.366l.218.81a.6.6 0 0 1 .029.117.166.166 0 0 1-.162.162.2.2 0 0 1-.092-.03l-1.057-.61a.5.5 0 0 0-.256-.074.5.5 0 0 0-.142.021 5.7 5.7 0 0 1-1.576.22M9.064 9.542a.647.647 0 1 0 .557-1 .645.645 0 0 0-.646.647.6.6 0 0 0 .09.353Zm3.232.001a.646.646 0 1 0 .546-1 .645.645 0 0 0-.644.644.63.63 0 0 0 .098.356"/>
  <path d="M0 6.826c0 1.455.781 2.765 2.001 3.656a.385.385 0 0 1 .143.439l-.161.6-.1.373a.5.5 0 0 0-.032.14.19.19 0 0 0 .193.193q.06 0 .111-.029l1.268-.733a.6.6 0 0 1 .308-.088q.088 0 .171.025a6.8 6.8 0 0 0 1.625.26 4.5 4.5 0 0 1-.177-1.251c0-2.936 2.785-5.02 5.824-5.02l.15.002C10.587 3.429 8.392 2 5.796 2 2.596 2 0 4.16 0 6.826m4.632-1.555a.77.77 0 1 1-1.54 0 .77.77 0 0 1 1.54 0m3.875 0a.77.77 0 1 1-1.54 0 .77.77 0 0 1 1.54 0"/>
</svg>
`;
const BAG_ICON = `
<svg xmlns="http://www.w3.org/2000/svg" viewBox="-5 -2 24 24">
  <path d="M6 14a1 1 0 0 0 2 0h3c.729 0 1.412-.195 2-.535V18a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2v-4.535c.588.34 1.271.535 2 .535h3zm2-1v-3a1 1 0 1 0-2 0v3H3a3 3 0 0 1-3-3V7a3 3 0 0 1 3-3h8a3 3 0 0 1 3 3v3a3 3 0 0 1-3 3H8zM5 3H3c-.345 0-.68.044-1 .126V1.5a1.5 1.5 0 0 1 3 0V3zm7 .126A4.007 4.007 0 0 0 11 3H9V1.5a1.5 1.5 0 0 1 3 0v1.626z"/>
</svg>
`;

const TRAVEL_ICON = `
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16">
  <path d="M9.5 1.5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0M6.44 3.752A.75.75 0 0 1 7 3.5h1.445c.742 0 1.32.643 1.243 1.38l-.43 4.083a1.8 1.8 0 0 1-.088.395l-.318.906.213.242a.8.8 0 0 1 .114.175l2 4.25a.75.75 0 1 1-1.357.638l-1.956-4.154-1.68-1.921A.75.75 0 0 1 6 8.96l.138-2.613-.435.489-.464 2.786a.75.75 0 1 1-1.48-.246l.5-3a.75.75 0 0 1 .18-.375l2-2.25Z"/>
  <path d="M6.25 11.745v-1.418l1.204 1.375.261.524a.8.8 0 0 1-.12.231l-2.5 3.25a.75.75 0 1 1-1.19-.914zm4.22-4.215-.494-.494.205-1.843.006-.067 1.124 1.124h1.44a.75.75 0 0 1 0 1.5H11a.75.75 0 0 1-.531-.22Z"/>
</svg>
`;

const INTRO_ICON = `
<svg xmlns="http://www.w3.org/2000/svg" viewBox="144 144 512 512">
  <path d="m270.27 289.79-27.707 32.434v-48.176l3.7773-4.4102c3.7812-3.5742 9.082-5.0508 14.164-3.9453 5.082 1.1055 9.293 4.6484 11.246 9.4727 1.9531 4.8203 1.4023 10.293-1.4805 14.625z"></path>
  <path d="m557.44 274.05v47.23l-27.707-31.488c-3.6562-4.2734-4.7539-10.176-2.8789-15.477 1.8711-5.3047 6.4336-9.207 11.965-10.234 5.5273-1.0312 11.188 0.96875 14.844 5.2461z"></path>
  <path d="m620.41 354.34v210.97c0 14.613-5.8047 28.629-16.137 38.965-10.336 10.332-24.352 16.137-38.965 16.137h-330.62c-14.617 0-28.633-5.8047-38.965-16.137-10.336-10.336-16.141-24.352-16.141-38.965v-210.97s188.93 155.87 209.55 162.01c3.543 0.92969 7.1992 1.3516 10.863 1.2578 3.7812 0.03125 7.5469-0.5 11.176-1.5742 24.875-7.5547 209.24-161.69 209.24-161.69z"></path>
  <path d="m506.27 179.58h-212.54c-13.035-0.46875-25.727 4.2305-35.312 13.078-9.582 8.8477-15.281 21.125-15.855 34.152v126.9c0.023437 9.7461 4.5586 18.93 12.281 24.875 39.832 31.488 79.98 61.559 119.81 92.262l4.5664 3.6211 10.707 8.1875h-0.003906c3.0664 2.3203 6.8711 3.4414 10.707 3.1484 3.082-0.19531 6.0391-1.2891 8.5-3.1484l135.55-104.38h0.003906c7.8125-5.7812 12.52-14.848 12.75-24.562v-126.9c-0.57031-13.027-6.2695-25.305-15.855-34.152-9.5859-8.8477-22.277-13.547-35.312-13.078zm-153.5 78.719h94.465c5.625 0 10.824 3.0039 13.637 7.875 2.8125 4.8711 2.8125 10.871 0 15.742-2.8125 4.8711-8.0117 7.8711-13.637 7.8711h-94.465c-5.625 0-10.82-3-13.633-7.8711-2.8125-4.8711-2.8125-10.871 0-15.742 2.8125-4.8711 8.0078-7.875 13.633-7.875zm94.465 94.465h-94.465c-5.625 0-10.82-3-13.633-7.8711-2.8125-4.8711-2.8125-10.871 0-15.746 2.8125-4.8711 8.0078-7.8711 13.633-7.8711h94.465c5.625 0 10.824 3 13.637 7.8711 2.8125 4.875 2.8125 10.875 0 15.746-2.8125 4.8711-8.0117 7.8711-13.637 7.8711z"></path>
  <path d="m571.77 321.28v23.301c0.058594 3.7344-0.47266 7.4531-1.5742 11.023l18.734-14.484zm-343.54 0-17.16 19.84c6.1406 4.7227 12.438 9.6055 18.578 14.168-0.96875-3.5391-1.4453-7.1953-1.418-10.863z"></path>
</svg>
`;

interface ILink {
  path: string;
  label: string;
  icon: string;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, MatTabsModule, MatButton, Page1Component, Page2Component, MatIcon, MatButtonToggleModule, MatChipsModule, MatIconModule, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  constructor() {
    const iconRegistry = inject(MatIconRegistry);
    const sanitizer = inject(DomSanitizer);

    iconRegistry.addSvgIconLiteral('intro-letter', sanitizer.bypassSecurityTrustHtml(INTRO_ICON))
    iconRegistry.addSvgIconLiteral('location-pin', sanitizer.bypassSecurityTrustHtml(PIN_ICON));
    iconRegistry.addSvgIconLiteral('travel', sanitizer.bypassSecurityTrustHtml(TRAVEL_ICON));
    iconRegistry.addSvgIconLiteral('inventory', sanitizer.bypassSecurityTrustHtml(BAG_ICON));
    iconRegistry.addSvgIconLiteral('talk', sanitizer.bypassSecurityTrustHtml(TALK_ICON));
    this.activePath = 'page-intro';
    routes
  }

  pageLinks: ILink[] = [
    { path: 'page-intro', label: 'Intro', icon: 'intro-letter' },
    { path: 'page-location', label: 'Location', icon: 'location-pin' },
    { path: 'page-travel', label: 'Travel', icon: 'travel' },
    { path: 'page-talk', label: 'Talk', icon: 'talk' },
    { path: 'page-inventory', label: 'Inventory', icon: 'inventory' },
  ];

  activePath = this.pageLinks[0].path;

  onActivate(path: string) {
      this.activePath = path;
  }

  title = 'gameui';
}
