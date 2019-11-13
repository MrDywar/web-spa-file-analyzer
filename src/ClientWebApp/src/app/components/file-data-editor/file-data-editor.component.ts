import { Component, OnInit, Input } from '@angular/core';
import { FileDataDto } from 'src/app/dtos/file-data-dto';
import { FileDataService } from 'src/app/services/file-data.service';

@Component({
  selector: 'app-file-data-editor',
  templateUrl: './file-data-editor.component.html',
  styleUrls: ['./file-data-editor.component.scss']
})
export class FileDataEditorComponent implements OnInit {
  @Input() fileData: FileDataDto;

  constructor(private fileDataService: FileDataService) {}

  trackByFn(index: any, item: any) {
    return index;
  }

  ngOnInit() {}

  save() {
    this.fileDataService.updateFileData(this.fileData).subscribe(
      () => {},
      error => {
        console.log(error);
      }
    );
  }

  addRow() {
    var newRow = Array(this.fileData.headers.length).fill('');
    this.fileData.rows.push(newRow);
  }
}
