import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, share } from 'rxjs/operators';
import { FileNodeDto } from '../dtos/file-node-dto';
import { FileDataDto } from '../dtos/file-data-dto';
import { HttpClient } from '@angular/common/http';
import { FileParseOptionsDto } from '../dtos/file-parse-options-dto';
import { BaseDataService } from './base-data.service';

@Injectable({
  providedIn: 'root'
})
export class FileDataService extends BaseDataService {
  constructor(http: HttpClient) {
    super(http);
  }

  getAllFiles(): Observable<FileNodeDto[]> {
    return this.http.get<FileNodeDto[]>('api/files').pipe(
      catchError(this.handleError),
      share()
    );
  }

  getFileData(parseOptions: FileParseOptionsDto): Observable<FileDataDto> {
    return this.http.post<FileDataDto>('api/files/parse', parseOptions).pipe(
      catchError(this.handleError),
      share()
    );
  }

  updateFileData(filedata: FileDataDto): Observable<any> {
    return this.http.post<any>('api/files/update', filedata).pipe(
      catchError(this.handleError),
      share()
    );
  }
}
