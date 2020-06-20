import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { GridService } from '../services/grid.service';
import { GridMessage } from '../models/gridMessage';
import { Grid } from '../models/grid';
import { environment } from 'src/environments/environment';
import { fabric } from 'fabric';
import { Canvas } from 'fabric/fabric-impl';

@Component({
  selector: 'app-grid',
  templateUrl: './grid.component.html',
  styleUrls: ['./grid.component.css']
})
export class GridComponent implements OnInit {

  constructor(gridService: GridService) {
    this.gridService = gridService;
  }

  @ViewChild('canvas', { static: false }) myCanvas: ElementRef;
  public context: HTMLCanvasElement;

  public gridService: GridService;
  public grids: Grid[] = [];
  public selectedGrid: Grid = null;
  public canvas: Canvas;
  public pencolor = 'black';
  public pensize = 4;
  public mouse = { x: 0, y: 0 };
  public selectedDrawingType = 'RECT';
  public started: boolean;
  public comment: string;
  public readworkitem = 'readworkitem';

  ngOnInit(): void {
    this.getCanvas();
    this.canvas = new fabric.Canvas('canvas', {
      isDrawingMode: true,
      selection: true
    });

    this.canvas.on('mouse:down', (e) => { this.mousedown(e); });
    this.canvas.on('mouse:move', (e) => { this.mousemove(e); });
    this.canvas.on('mouse:up', (e) => { this.mouseup(e); });
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

  showGrid(grid: Grid): any {
    this.selectedGrid = grid;
  }

  /* Mouseup */
  mouseup(e) {
    if (this.started) {
      this.started = false;
    }
  }

  /* Mousedown */
  mousedown(e) {
    const mouse = this.canvas.getPointer(e.pointer);
    this.started = true;
    this.mouse.x = mouse.x;
    this.mouse.y = mouse.y;
    const origX = this.mouse.x;
    const origY = this.mouse.y;
    if (this.selectedDrawingType === 'RECT') {
      const rect = new fabric.Rect({
        left: origX,
        top: origY,
        width: 0,
        height: 0,
        stroke: this.pencolor,
        strokeWidth: this.pensize,
        fill: ''
      });
      this.canvas.add(rect);
      this.canvas.setActiveObject(rect);
      this.canvas.renderAll();

    } else if (this.selectedDrawingType === 'CIRCLE') {
      const ellipse = new fabric.Ellipse({
        left: origX,
        top: origY,
        originX: 'left',
        originY: 'top',
        rx: this.mouse.x - origX,
        ry: this.mouse.y - origY,
        angle: 0,
        fill: '',
        stroke: this.pencolor,
        strokeWidth: this.pensize,
      });
      this.canvas.add(ellipse);
      this.canvas.setActiveObject(ellipse);
      this.canvas.renderAll();

    }
  }

  mousemove(e) {
    if (!this.started) {
      return false;
    }
    const pointer = this.canvas.getPointer(e.pointer);
    const origX = pointer.x;
    const origY = pointer.y;
    if (this.selectedDrawingType === 'RECT') {
      const rect = this.canvas.getActiveObject();
      if (rect === null) {
        return;
      }
      const mouse = this.canvas.getPointer(e.pointer);
      const w = Math.abs(mouse.x - this.mouse.x);
      const h = Math.abs(mouse.y - this.mouse.y);
      if (!w || !h) {
        return false;
      }
      const square = this.canvas.getActiveObject();
      square.set('width', w).set('height', h);
      this.canvas.renderAll();
    } else if (this.selectedDrawingType === 'CIRCLE') {
      const ellipse = this.canvas.getActiveObject();
      if (ellipse === null) {
        return;
      }

      let rx = Math.abs(origX - this.mouse.x) / 2;
      let ry = Math.abs(origY - this.mouse.y) / 2;
      if (rx > ellipse.strokeWidth) {
        rx -= ellipse.strokeWidth / 2;
      }
      if (ry > ellipse.strokeWidth) {
        ry -= ellipse.strokeWidth / 2;
      }
      ellipse.set('{ rx: rx, ry: ry }');

      if (origX > pointer.x) {
        ellipse.set({ originX: 'right' });
      } else {
        ellipse.set({ originX: 'left' });
      }
      if (origY > pointer.y) {
        ellipse.set({ originY: 'bottom' });
      } else {
        ellipse.set({ originY: 'top' });
      }
      this.canvas.renderAll();
    }
  }

  saveGrid() {
    this.context = this.canvas.getElement();
    this.context.toBlob((blob) => {
      let gridData: any = this.selectedGrid;
      gridData.append('file', blob, 'file.png');
      this.gridService.SaveGrid(gridData).subscribe(() => {
        this.selectedGrid = null;
      });
    });
  }

  revertGrid() {
    this.canvas.clear();
  }

  cancelGrid() {
    this.selectedGrid = null;
  }

  onCanvasClick() {

  }
}
