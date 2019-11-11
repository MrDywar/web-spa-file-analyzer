import { FileNodeDto } from 'src/app/dtos/file-node-dto';

export interface FlatNode {
  expandable: boolean;
  isSelected: boolean;
  level: number;
  value: FileNodeDto;
}
