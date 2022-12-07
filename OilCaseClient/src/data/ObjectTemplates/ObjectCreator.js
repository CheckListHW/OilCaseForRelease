import {Properties} from "../../api/Properties";

export class ObjectCreator {
  static Profile2DHorizontal(id, gameStep, cellY, width, CurrentPointY) {
    return {
      idx: id,
      id: 'prhorz_' + id,
      profType: 'horz',
      iCell: 0,
      jCell: cellY,
      onGameStep: gameStep,
      stroke: 'green',
      opacity: 0.5,
      strokeWidth: 2,
      points: [0, CurrentPointY, width, CurrentPointY],
      name: '2d' + id,
      seismType: ["seismM"],
      lblrect: {
        id: 'prhorzrect_' + id,
        fill: 'green',
        opacity: 0.6,
        width: 30,
        height: 30,
        x: 0,
        y: CurrentPointY - 15,
        label: {
          id: 'prvhorzlbl' + id,
          text: '2d' + id,
          x: 5,
          y: CurrentPointY - 12,
          fill: 'green'
        }
      }
    }
  }

  static HorizontalSeismic(id, gameStep, cellX, height, CurrentPointY, endDate) {
    return {
      idx: id,
      id: 'prvert_' + id,
      profType: 'vert',
      iCell: cellX,
      jCell: 0,
      onGameStep: gameStep,
      stroke: 'blue',
      strokeWidth: 2,
      opacity: 0.5,
      points: [CurrentPointY, 0, CurrentPointY, height],
      name: '2d' + id,
      seismType: Array("seismM"),
      lblrect: {
        id: 'prvertrect_' + id,
        fill: 'blue',
        opacity: 0.6,
        width: 30,
        height: 30,
        x: CurrentPointY - 15,
        y: 0,
        label: {
          id: 'prvertlbl' + id,
          text: '2d' + id,
          x: CurrentPointY - 15,
          y: 10,
          fill: 'blue'
        }
      }
    }
  }

  static SheetItemON(text, handler) {
    return {
      label: text,
      type: 'map',
      color: 'purple',
      icon: 'check_box_outline_blank',
      handler: handler
    }
  }

  static SheetItemOff(handler) {
    return {
      label: 'Отключить карты',
      type: 'mapoff',
      color: 'gray',
      icon: 'check_box_outline_blank',
      handler: handler
    }
  }

  static ObjectOfArrangement(id, gameStep, data,
                             cellX, cellY,
                             subCellX, subCellY,
                             drawCellX, drawCellY,
                             drawSubCellX, drawSubCellY,
                             width, height,
                             endDate, objectOfArrangement) {
    let objectTipText = objectOfArrangement.sPref === 'kp' ? 'K' + id
      : (objectOfArrangement.part !== 2 ? 's' + id : '')

    let newObjectOfArrangement = {
      idx: id,
      id: 'surf_' + id,
      profType: data,
      x: drawCellX,
      y: drawCellY,
      iCell: cellX,
      jCell: cellY,
      subCellX: subCellX,
      subCellY: subCellY,
      onGameStep: gameStep,
      addedDate: endDate,
      name: 's' + id,
      description: objectOfArrangement.sText,
      objModel: objectOfArrangement,
      lblrect: {
        id: 'surfrect_' + id,
        fill: objectOfArrangement.color !== null ? objectOfArrangement.color : 'brown',
        opacity: 0.6,
        width: 30,
        height: 30,
        x: drawCellX - 15,
        y: drawCellY - 15
      },
      lblrectPO: {
        id: 'surfrect_' + id,
        fill: objectOfArrangement.color !== null ? objectOfArrangement.color : 'brown',
        tooltip: `${objectOfArrangement.sText} ${objectTipText}`,
        opacity: 0.8,
        width: width / Properties.CellXSize,
        height: height / Properties.CellYSize,
        x: drawSubCellX - (width / Properties.CellXSize) / 2,
        y: drawSubCellY - (height / Properties.CellYSize) / 2,
        iCell: cellX,
        jCell: cellY,
        subCellX: subCellX,
        subCellY: subCellY,
        label: {
          id: 'surflbl' + id,
          text: objectTipText,
          fontSize: 12,
          x: drawSubCellX - 8,
          y: drawSubCellY - 5,
          fill: objectOfArrangement.sPref === 'kp' ? 'brown' : '#fff'
        }
      }
    }

    if (objectOfArrangement.part === 2) {
      newObjectOfArrangement.lblrectPO.width = 5;
      newObjectOfArrangement.lblrectPO.height = (height / Properties.CellYSize);
      newObjectOfArrangement.lblrectPO.x = drawSubCellX;
      newObjectOfArrangement.lblrectPO.y = drawSubCellY - (height / Properties.CellYSize) / 2;
    }

    let partN = newObjectOfArrangement.objModel.part
    newObjectOfArrangement.lblrect.fill = partN < 3 ? 'gray' : (partN > 3 ? '#bf7373' : '#e8d08d')

    return newObjectOfArrangement
  }

  static Borehole(id,
                  drawCellX, drawCellY,
                  cellX, cellY,
                  name, gameStep,
                  drillDepth, modelWell, toeI, toeJ, toeK) {
    return {
      idx: id,
      x: drawCellX,
      y: drawCellY,
      iCell: cellX,
      jCell: cellY,
      onGameStep: gameStep,
      id: name,
      fill: '#f7450e',
      drilldeep: drillDepth,
      modelWell: modelWell,
      toeI: toeI, toeJ, toeK,
      selWellIssl: [],
      selWellIssl2: [],
      selWellIssl3: [],
      selWellIssl4: [],
      researches: [],
      opacity: 0.8,
      bEditModel: false,
      tappedLevels: [],
      name: 'w' + id,
      radius: 12,
      label: {
        id: 'drLblb' + id,
        text: 'w' + id,
        x: drawCellX - 7,
        y: drawCellY - 5,
        fill: '#fff'
      }
    }
  }

  static TrajectoryPoint(cellX, cellY, cellZ) {
    return {
      "CellX": cellX,
      "CellY": cellY,
      "CellZ": cellZ
    }
  }

  static TypeOfResearch(name, description, group, cost) {
    return {
      "Name": name,
      "Description": description,
      "Group": group,
      "Cost": cost
    }
  }
}

// export class TypeOfResearch {
//   constructor(name, description, group, cost) {
//     this.Name = name
//     this.Description = description
//     this.Group = group
//     this.Cost = cost
//   }
// }
