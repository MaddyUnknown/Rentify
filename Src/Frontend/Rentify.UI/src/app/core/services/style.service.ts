import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class StyleService {
  public getRootFontSize(): number {
    return parseFloat(getComputedStyle(document.documentElement).fontSize);
  }

  public getRemToPx(value: number): number {
    return value * this.getRootFontSize();
  }
}
