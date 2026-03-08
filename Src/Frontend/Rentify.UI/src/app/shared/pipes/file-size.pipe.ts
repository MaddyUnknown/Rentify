import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'fileSize',
  standalone: true,
})
export class FileSizePipe implements PipeTransform {
  private readonly MULTIPLIER = 1024;
  private readonly SIZES = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB'];

  transform(bytes?: number, decimals: number = 2): string {
    if (bytes == undefined || isNaN(bytes) || bytes === 0) return '0 Bytes';

    const dm = decimals < 0 ? 0 : decimals;
    const i = Math.floor(Math.log(bytes) / Math.log(this.MULTIPLIER));

    return `${parseFloat((bytes / Math.pow(this.MULTIPLIER, i)).toFixed(dm))} ${this.SIZES[i]}`;
  }
}
