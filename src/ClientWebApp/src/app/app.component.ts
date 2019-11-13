import { Component } from '@angular/core';
import { FileDataDto } from './dtos/file-data-dto';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'file-analyzer';
  fileData: FileDataDto;

  constructor() {}

  onFileParsed(value: FileDataDto) {
    this.fileData = value;
  }
}
