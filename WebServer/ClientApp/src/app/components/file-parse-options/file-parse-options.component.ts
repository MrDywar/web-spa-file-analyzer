import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  FormControl,
  Validators
} from '@angular/forms';
import { FileDataDto } from 'src/app/dtos/file-data-dto';
import { MatDialog } from '@angular/material';
import { FileDataService } from 'src/app/services/file-data.service';
import { ColumnDelimiterType } from 'src/app/enums/column-delimiter-type';
import { FileSelectModalComponent } from '../file-select-modal/file-select-modal.component';
import { FileParseOptionsDto } from 'src/app/dtos/file-parse-options-dto';

@Component({
  selector: 'app-file-parse-options',
  templateUrl: './file-parse-options.component.html',
  styleUrls: ['./file-parse-options.component.scss']
})
export class FileParseOptionsComponent implements OnInit {
  @Output() fileParsed = new EventEmitter<FileDataDto>();

  form: FormGroup;

  constructor(
    private dialog: MatDialog,
    private fileDataService: FileDataService,
    fb: FormBuilder
  ) {
    this.form = fb.group({
      filePath: new FormControl('', Validators.required),
      hasHeaders: new FormControl(false),
      columnDelimiterType: new FormControl(ColumnDelimiterType.Tab),
      customColumnDelimiter: new FormControl(null)
    });
  }

  ngOnInit(): void {}

  openDialog(): void {
    this.fileDataService.getAllFiles().subscribe(
      allFiles => {
        const dialogRef = this.dialog.open(FileSelectModalComponent, {
          width: '650px',
          data: allFiles
        });

        dialogRef.afterClosed().subscribe(fileFullName => {
          if (fileFullName) {
            this.form.get('filePath').setValue(fileFullName);
          }
        });
      },
      error => {
        console.log(error);
      }
    );
  }

  readFile(): void {
    if (this.form.invalid) {
      return;
    }

    const parseOptions: FileParseOptionsDto = {
      fullName: this.form.get('filePath').value,
      hasHeaders: this.form.get('hasHeaders').value,
      delimiter: this.getDelimiterValue()
    };

    this.fileDataService.getFileData(parseOptions).subscribe(
      fileData => {
        this.fileParsed.emit(fileData);
      },
      error => {
        console.log(error);
      }
    );
  }

  private getDelimiterValue(): string {
    switch (this.form.get('columnDelimiterType').value) {
      case ColumnDelimiterType.Tab:
        return '\t';
      case ColumnDelimiterType.Whitespace:
        return ' ';
      case ColumnDelimiterType.Semicolon:
        return ';';
      case ColumnDelimiterType.Custom:
        return this.form.get('customColumnDelimiter').value;
      default:
        throw 'Error: unsupported delimiter type.';
    }
  }
}
