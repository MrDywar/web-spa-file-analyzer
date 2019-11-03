export interface FileNodeDto {
  name: string;
  fullName: string;
  isFolder: boolean;
  children: FileNodeDto[];
}
