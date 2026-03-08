import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'fileType',
  standalone: true,
})
export class FileTypePipe implements PipeTransform {
  private readonly MIME_TO_FILE_TYPE = new Map<string, string>([
    ['image/jpeg', 'JPEG'],
    ['image/png', 'PNG'],
    ['image/gif', 'GIF'],
    ['image/bmp', 'BMP'],
    ['application/pdf', 'PDF'],
  ]);

  transform(mimeType: string): string {
    return this.MIME_TO_FILE_TYPE.get(mimeType) ?? 'FILE';
  }
}
