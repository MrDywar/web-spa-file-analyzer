import { Component, Inject } from '@angular/core';
import {
  MAT_DIALOG_DATA,
  MatDialogRef,
  MatTreeFlattener,
  MatTreeFlatDataSource
} from '@angular/material';
import { FileNodeDto } from '../../dtos/file-node-dto';
import { FlatTreeControl } from '@angular/cdk/tree';
import { FlatNode } from './flat-node';

@Component({
  selector: 'app-file-select-modal',
  templateUrl: './file-select-modal.component.html',
  styleUrls: ['./file-select-modal.component.scss']
})
export class FileSelectModalComponent {
  constructor(
    public dialogRef: MatDialogRef<FileSelectModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: FileNodeDto[]
  ) {
    this.dataSource.data = data;
  }

  private _transformer = (node: FileNodeDto, level: number) => {
    return {
      expandable: node.isFolder && node.children.length > 0,
      isSelected: false,
      level: level,
      value: node
    };
  };

  treeControl = new FlatTreeControl<FlatNode>(
    node => node.level,
    node => node.expandable
  );

  treeFlattener = new MatTreeFlattener(
    this._transformer,
    node => node.level,
    node => node.expandable,
    node => node.children
  );

  dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);
  selectedFlatNode: FlatNode;

  hasChild = (_: number, node: FlatNode) => node.expandable;

  onNoClick(): void {
    this.dialogRef.close();
  }

  onSelectFile(flatNode: FlatNode): void {
    if (flatNode.value.isFolder) {
      return;
    }

    if (this.selectedFlatNode) {
      this.selectedFlatNode.isSelected = false;
    }

    this.selectedFlatNode = flatNode;
    this.selectedFlatNode.isSelected = true;
  }
}
