import { AfterViewInit, Component, ElementRef, QueryList, Renderer2, ViewChild, ViewChildren } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, RouterLink } from '@angular/router';
import {
  LucideAngularModule,
  FileText,
  MapPinPlus,
  House,
  ReceiptText,
  Users,
  UtilityPole,
  Building2,
} from 'lucide-angular';
import { filter, map } from 'rxjs';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, LucideAngularModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent implements AfterViewInit {
  @ViewChildren('navItem') navItems!: QueryList<ElementRef<HTMLLIElement>>;
  @ViewChild('navHighlight') navHighlight!: ElementRef<HTMLDivElement>;

  readonly ICONS = { FileText, MapPinPlus, House, ReceiptText, Users, UtilityPole, Building2 };
  private readonly NAV_ITEM_ACTIVE_CLASS = 'app-header__nav__item--active';
  private readonly NAV_DATA_ATTR = 'navName';

  private currentNav: string = '';

  constructor(
    private renderer: Renderer2,
    private router: Router,
    private activeRoute: ActivatedRoute,
  ) {}

  ngAfterViewInit(): void {
    this.updateNavHighlight();
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe(() => this.updateNavHighlight());
  }

  private updateNavHighlight() {
    let route = this.activeRoute.snapshot;
    while (route.firstChild) route = route.firstChild;

    const navName = route.data[this.NAV_DATA_ATTR];
    if (navName !== this.currentNav) {
      const element = this.navItems
        .toArray()
        .filter((item) => item.nativeElement.dataset[this.NAV_DATA_ATTR] === navName);
      if (element.length === 0) return;

      this.currentNav = navName;
      this.onNavChange(element[0]);
    }
  }

  private onNavChange(currentElement: ElementRef<HTMLLIElement>): void {
    let firstElement: HTMLLIElement = this.navItems.first.nativeElement;
    let selectedElement: HTMLLIElement = currentElement.nativeElement;

    this.navItems
      .toArray()
      .filter((item) => item.nativeElement.classList.contains(this.NAV_ITEM_ACTIVE_CLASS))
      .forEach((item) => {
        this.renderer.removeClass(item.nativeElement, this.NAV_ITEM_ACTIVE_CLASS);
      });
    this.renderer.addClass(currentElement.nativeElement, this.NAV_ITEM_ACTIVE_CLASS);

    const { top: upperBoundary } = firstElement.getBoundingClientRect();
    const { top: targetBoundary } = selectedElement.getBoundingClientRect();

    this.renderer.setStyle(this.navHighlight.nativeElement, 'top', `${targetBoundary - upperBoundary}px`);
  }
}
