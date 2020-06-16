import { Component, OnInit } from '@angular/core';
import { GridService } from '../services/grid.service';
import { GridMessage } from '../models/gridMessage';
import { Grid } from '../models/grid';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-grid',
  templateUrl: './grid.component.html',
  styleUrls: ['./grid.component.css']
})
export class GridComponent implements OnInit {

  constructor(gridService: GridService) {
    this.gridService = gridService;
  }
  gridService: GridService;
  grids: Grid[] = [];

  ngOnInit(): void {
    this.getCanvas();
  }

  getCanvas() {
    this.gridService.LoadGrids().subscribe((data: GridMessage) => {
      if (data.operationStatus) {
        this.grids = data.returnObject;
        this.grids.forEach(grid => {
          grid.image = grid.image === undefined || grid.image === null ?
            environment.imagesURL + 'noimage.jpg' : environment.imagesURL + grid.image;
        });
      }
    });
  }
}
