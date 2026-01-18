import { Component, ElementRef, OnInit, ViewChild, ViewChildren, AfterViewInit, QueryList, Renderer2 } from '@angular/core';
import { GridService } from '../services/grid.service';
import { GridMessage } from '../models/gridMessage';
import { Grid } from '../models/grid';
import { environment } from 'src/environments/environment';
import { fabric } from 'fabric';
import { Canvas } from 'fabric/fabric-impl';

@Component({
    selector: 'app-grid',
    templateUrl: './grid.component.html',
    styleUrls: ['./grid.component.css'],
    standalone: false
})
export class GridComponent implements AfterViewInit {

  constructor(gridService: GridService, renderer: Renderer2) {
    this.gridService = gridService;
    this.renderer = renderer;
  }

  @ViewChild('canvas', { static: false }) myCanvas: ElementRef;
  @ViewChildren('grids') elementGrids: QueryList<any>;
  renderer: Renderer2;
  public context: HTMLCanvasElement;

  public timer: any;
  public seizureModeBit: boolean = false;

  public gridService: GridService;
  public grids: Grid[] = [];
  public rowGrids: Grid[][] = [];
  public selectedGrid: Grid = null;
  public canvas: Canvas;
  public pencolor = 'black';
  public pensize = 4;
  public mouse = { x: 0, y: 0 };
  public selectedDrawingType = 'RECT';
  public started: boolean;
  public comment: string;
  public readworkitem = 'readworkitem';

  ngAfterViewInit(): void {
    this.getCanvas();
    this.canvas = new fabric.Canvas('canvas', {
      isDrawingMode: true,
      selection: true
    });

    this.canvas.on('mouse:down', (e) => { this.mousedown(e); });
    this.canvas.on('mouse:move', (e) => { this.mousemove(e); });
    this.canvas.on('mouse:up', (e) => { this.mouseup(e); });

    this.initializeRandomColors();
  }

  uploadImage(event) {
    if (event.target.files.length > 0) {
      const file = event.target.files[0];
      if (!file.type || !file.type.toLowerCase().includes('image')) {
        console.log('Only Upload Images');
        return;
      }
      const reader = new FileReader();

      reader.onload = (e: any) => {
        const data = e.target.result;
        fabric.Image.fromURL(data, img => {
          this.canvas.add(img);
          this.canvas.centerObject(img);
        });
      };

      reader.readAsDataURL(file);
    }
  }

  initializeRandomColors() {
    this.elementGrids.changes.subscribe((gridElements: ElementRef[]) => {
      if (gridElements !== null && gridElements !== undefined) {
        gridElements.forEach(gridElement => {
          if (gridElement != null && gridElement !== undefined) {
            this.renderer.setStyle(gridElement.nativeElement, 'background-color', this.generateRandomColor());
          } else {
            console.log('gridElement is undefined');
          }
        });
      } else {
        console.log('List of gridElements is null or undefined');
      }
    });
  }

  setColors() {
    this.elementGrids.forEach(gridElement => {
      if (gridElement != null && gridElement !== undefined) {
        this.renderer.setStyle(gridElement.nativeElement, 'background-color', this.generateRandomColor());
      } else {
        console.log('gridElement is undefined');
      }
    });
  }

  seizureMode() {
    if (!this.seizureModeBit) {
      this.timer = setInterval(() => { this.setColors(); }, 500);
    } else {
      clearInterval(this.timer);
    }
    this.seizureModeBit = !this.seizureModeBit;
  }

  simpsonsSeizureMode() {
    this.elementGrids.forEach(gridElement => {
      if (gridElement != null && gridElement !== undefined) {
        this.renderer.setStyle(gridElement.nativeElement, 'background-image', 'none');
      } else {
        console.log('gridElement is undefined');
      }
    });
  }

  randomizeList() {
    const startIndex: number = this.grids.length - 1;
    for (let index = startIndex; index > 0; index--) {
      const j = Math.floor(Math.random() * index);
      const temp = this.grids[index];
      this.grids[index] = this.grids[j];
      this.grids[j] = temp;
    }
  }

  generateRandomColor() {
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
      color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
  }

  getCanvas() {
    this.gridService.LoadGrids().subscribe((data: GridMessage) => {
      if (data.operationStatus) {
        this.grids = data.returnObject;
        let rowGridIndex: number = 0;
        let rowIndex: number = 0;
        this.grids.forEach(grid => {
          grid.image = grid.image === undefined || grid.image === null ?
            '' : environment.imagesURL + grid.title;

          if (this.rowGrids[rowGridIndex] === undefined) {
            this.rowGrids[rowGridIndex] = [];
          }

          this.rowGrids[rowGridIndex][rowIndex] = grid;
          if (rowIndex === 99) {
            rowGridIndex++;
            rowIndex = 0;
          } else {
            rowIndex++;
          }
        });
      }
    });
  }

  showGrid(grid: Grid): any {
    this.selectedGrid = grid;
  }

  saveGrid() {
    this.selectedGrid.base64File = this.canvas.toDataURL().replace(/^data:image\/(png|jpg);base64,/, '');
    this.gridService.SaveGrid(this.selectedGrid).subscribe((result: GridMessage) => {
      this.selectedGrid = null;
      this.canvas.clear();
      this.getCanvas();
      document.body.scrollTop = 0;
    });
  }

  revertGrid() {
    this.canvas.clear();
  }

  cancelGrid() {
    this.canvas.clear();
    this.selectedGrid = null;
    document.body.scrollTop = 0;
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
}
