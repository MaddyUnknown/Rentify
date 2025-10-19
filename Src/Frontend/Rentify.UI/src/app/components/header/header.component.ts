import { AfterViewInit, Component, ElementRef, QueryList, Renderer2, ViewChild, ViewChildren } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, RouterLink } from '@angular/router';
import { filter, map } from 'rxjs';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements AfterViewInit {
  @ViewChildren('navItem') navItems!: QueryList<ElementRef<HTMLLIElement>>;
  @ViewChild('navHighlight') navHighlight!: ElementRef<HTMLDivElement>;
  private currentNav: string = '';

  constructor(private renderer: Renderer2, private router: Router, private activeRoute: ActivatedRoute) {}

  ngAfterViewInit(): void {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      map(() => {
        let route = this.activeRoute.snapshot;
        while(route.firstChild) route = route.firstChild;
        return route.data['navName'];
      })
    ).subscribe(navName => {
      if(navName !== this.currentNav) {
        const element = this.navItems.toArray().filter((item) => item.nativeElement.dataset['navName'] === navName);
        if(element.length === 0) return;

        this.currentNav = navName;
        this.onNavChange(element[0]);
      }
    });
  }

  onNavChange(currentElement: ElementRef<HTMLLIElement>): void {
    let firstElement: HTMLLIElement = this.navItems.first.nativeElement;
    let selectedElement: HTMLLIElement = currentElement.nativeElement;

    this.navItems.toArray().filter(item => item.nativeElement.classList.contains('active')).forEach((item) => {
      this.renderer.removeClass(item.nativeElement, 'active');
    });
    this.renderer.addClass(currentElement.nativeElement, 'active');

    const {top: upperBoundary} = firstElement.getBoundingClientRect();
    const {top: targetBoundary} = selectedElement.getBoundingClientRect();

    this.renderer.setStyle(this.navHighlight.nativeElement, 'top', `${targetBoundary-upperBoundary}px`);
  }
}
