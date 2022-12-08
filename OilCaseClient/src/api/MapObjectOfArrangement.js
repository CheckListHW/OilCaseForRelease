import {Properties} from "src/api/Properties";
import {} from "src/api/OilCaseApi";


export class MapObjectOfArrangement {
  constructor() {
    this.SizeX = Properties.MapXSize
    this.SizeY = Properties.MapYSize
    this.Cells = [];
    for (let i = 0; i <= this.SizeX; i++) {
      this.Cells.push([])
      for (let j = 0; j <= this.SizeY; j++) {
        this.Cells[i].push(null)
      }
    }
  }

  setCell(x, y) {
    if (this.Cells[x][y] === null)
      this.Cells[x][y] = new Cell(x, y)
    return this.Cells[x][y]
  }

  toDict() {
    let resultCells = []
    this.Cells.forEach((cellsArray) => {
      cellsArray.filter(c => c !== null).forEach((cell) => {
        resultCells.push(cell.toDict())
      })
    })

    return {
      'SizeX': this.SizeX,
      'SizeY': this.SizeY,
      'Cells': resultCells
    }
  }

  load() {
    return false
  }
}

class Cell {
  constructor(x, y) {
    this.SizeX = Properties.CellXSize
    this.SizeY = Properties.CellYSize
    this.X = x
    this.Y = y
    this.SubCells = [];
    for (let i = 0; i <= this.SizeX; i++) {
      this.SubCells.push([])
      for (let j = 0; j <= this.SizeY; j++) {
        this.SubCells[i].push(null)
      }
    }
  }

  toDict() {
    let currentSubCells = []
    this.SubCells.forEach(subCellsArray => {
      subCellsArray.filter(c => c!== null).forEach(subCell => {
        currentSubCells.push(subCell.toDict())
      })
    })
    return {
      'X': this.X,
      'Y': this.Y,
      'SubCells': currentSubCells
    }
  }

  setSubCell(x, y, name, gameStep) {
    this.SubCells[x][y] = new SubCell(x, y, name, gameStep)
    return this.SubCells[x][y]
  }
}

class SubCell {
  constructor(x, y, key, gameStep) {
    this.X = x
    this.Y = y
    this.Key = key
    this.GameStep = gameStep
  }

  toDict() {
    return {
      'X': this.X,
      'Y': this.Y,
      'Key': this.Key,
      'GameStep': this.GameStep,
    }
  }
}

