import PortalApi from 'src/api/OilcaseApi.js'
import OrbitControls from 'three-orbitcontrols'
import {  GridHelper } from 'three'
import SecureLS from 'secure-ls'
import {date} from "quasar";

let orbitControls
let vm = {};
export const OutSideMixin = {
  data() {
    return {
      gridSizeSeism: 200,
      iCurGameStep: 0,
      dCommonZLevel: 0.0,
      dCommonZLevelSeism1: 0.0,
      dCommonZLevelSeism2: 0.0,
      //  arrDictParams:[],
      arrDrillsList: [],
      arr2DProfileList: [],
      arr3DProfileList: [],
      arrSurfObjList: [],
      arrMapList: [],
      dictMapsTypes: [],
      secLS: null,
      surfmodel: null,

    }
  },
  created() {
    this.secLS = new SecureLS();
  },
  methods: {
    sumZatrIssl(item) {
      return item.selWellIssl.length * vm.mnyWellIssl1 +
        item.selWellIssl2.length * vm.mnyWellIssl2 +
        item.selWellIssl3.length * vm.mnyWellIssl3 +
        item.selWellIssl4.length * vm.mnyWellIssl4
    },
    getWellMoneyAll(deep) {
      return vm.mnyWellBase +
        (deep >= 2700 ? 500 : 0) +
        Math.floor((deep - 2700) / 10) * 10
    },
    getWellMoneyDrill(deep) {
      return (deep >= 2700 ? 500 : 0) + Math.floor((deep - 2700) / 10) * 10
    },
    getFormatedDate: function (data, mode) {
      if (mode === 1) {
        return date.formatDate(data, "DD.MM.YYYY");
      } else if (mode === 2) {
        return date.formatDate(data, "HH:mm:ss");
      } else {
        return date.formatDate(data, "DD.MM.YYYY HH:mm:ss");
      }
    },

    getSeismTypeMoney1(seistype) {
      let strst = "";
      if (Array.isArray(seistype)) {
        seistype.forEach(el => {
          if (el === 'seismR') {
            if (strst !== '') strst += " + ";
            strst += "5000 т.р. ";
          } else if (el === 'seismM') {
            if (strst !== '') strst += " + ";
            strst += "1000 OilCoin";
          } else if (el === 'seismT') {
            if (strst !== '') strst += " + ";
            strst += "11000 т.р. ";
          }
        });
      }

      return strst;
    },
    getSeismTypeMoney2(seistype) {
      let strst = "";
      seistype.forEach(el => {
        if (el === 'seismR') {
          if (strst !== '') strst += " + ";
          strst += "5000 т.р. ";
        } else if (el === 'seismM') {
          if (strst !== '') strst += " + ";
          strst += "1000 OilCoin";
        } else if (el === 'seismT') {
          if (strst !== '') strst += " + ";
          strst += "35000 т.р. ";
        }
      });
      return strst;
    },

    getSeismTypeText(seistype) {
      let strst = "";
      if (Array.isArray(seistype)) {
        seistype.forEach(el => {
          if (el === 'seismR') {
            if (strst !== '') strst += ", ";
            strst += "Временной разрез";
          } else if (el === 'seismM') {
            if (strst !== '') strst += ", ";
            strst += "Интерпретация по глубине.";
          } else if (el === 'seismT') {
            if (strst !== '') strst += ", ";
            strst += "Интерпретация по времени";
          }
        });
      } else {
        if (seistype === 'seismR') {
          strst += "Временной разрез";
        } else if (seistype === 'seismM') {
          strst += "Интерпретация по глубине.";
        } else if (seistype === 'seismT') {
          strst += "Интерпретация по времени";
        }
      }

      return strst;
    },
    async build3DSurf(iCell_l, iCell_r, jCell_l, jCell_r, seismType, userId) {
      try {
        vm = this;
        //   this.buildCheck();

        //this.strTempFromApi = await PortalApi.GetAllSurfacesRange(iCell_l, iCell_r, jCell_l, jCell_r);
        this.strTempFromApi = await PortalApi.GetAllSurfacesRangeSeism21(iCell_l, iCell_r, jCell_l, jCell_r, seismType, userId);

        if (this.strTempFromApi === "not yet saved") {
          return;
        }
        let zMin = 0.0;
        if (seismType === 'seismM') {
          zMin = this.dCommonZLevelSeism1;
        } else {
          zMin = this.dCommonZLevelSeism2;
        }

        let zMinLevel = zMin - 2;
        let sallLayers = this.uniqueLayers(this.strTempFromApi);

        this.drSurfCount = 1;
        for (let index in sallLayers) {
          let curLayer = sallLayers[index];
          let curCoords = this.strTempFromApi.filter(d => d.sLayerName === curLayer.sLayerName);
          this.buildSurf2w(curCoords, zMinLevel);
        }

      } catch (error) {
        console.log(error);
      }
    },
    async build2DProfileSeism(iCell, jCell, cType, seismType, userId) {
      try {
        vm = this;
        //   this.buildCheck();
        if (cType === 'vert') {
          this.strTempFromApi = await PortalApi.GetSeismic2D("i", iCell, seismType, userId);
        } else {
          this.strTempFromApi = await PortalApi.GetSeismic2D("j", jCell, seismType, userId);
        }
        if (this.strTempFromApi === "not yet saved") {
          return;
        }
        let zMin = 0.0;
        if (seismType === 'seismM') {
          zMin = this.dCommonZLevelSeism1;
        } else {
          zMin = this.dCommonZLevelSeism2;
        }
        let zMinLevel = zMin - 2;
        let sallLayers = this.uniqueLayers(this.strTempFromApi);

        let iCurNUm = 1;

        for (let index in sallLayers) {
          let curLayer = sallLayers[index];
          let curCoords = this.strTempFromApi.filter(d => d.sLayerName === curLayer.sLayerName);
          this.build2DProfOneSeism(curCoords, zMinLevel, cType, iCurNUm, iCell, jCell);
          iCurNUm++;
        }

      } catch (error) {
        console.log(error);
      }
    },

    async build2DProfOneSeism(arrData, zLevel, cType, NumColor, iCell, jCell) {
      vm = this;

      let colMin = this.getMin(arrData, 'dcol');
      let colMax = this.getMax(arrData, 'dcol');
      let colMid = colMax - colMin;

      let rowMin = this.getMin(arrData, 'drow');
      let rowMax = this.getMax(arrData, 'drow');
      let rowMid = rowMax - rowMin;


      let zMin = zLevel;
      let zMax = this.getMax(arrData, 'zCoord');
      let zMid = zMax - zMin;
      let scaleZKoef = -0.05; // 100 / zMid;
      let cellSize = 2 * this.gridSizeSeism / (this.gridCount * 2);
      let gridHalf = this.gridSizeSeism / 2;
      //  let zOffset = (zMin - zLevel);
      let arrLinePoints = [];
      let oneStep = this.gridSizeSeism / arrData.length;


      arrData.forEach(element => {
        let sd = (zMin - element.zCoord) * scaleZKoef * 10;

        if (cType === 'vert') {
          let curVector = new THREE.Vector3(-gridHalf + cellSize * iCell - (cellSize / 2), gridHalf - oneStep * element.drow, sd);
          arrLinePoints.push(curVector);
        } else {
          let curVector = new THREE.Vector3(-gridHalf + oneStep * element.dcol, gridHalf - cellSize * jCell + (cellSize / 2), sd);
          arrLinePoints.push(curVector);
        }

      });
      let curve = new THREE.CatmullRomCurve3(arrLinePoints);

      let geometry = new THREE.TubeGeometry(curve, 10, 0.2, 8, false);

      if (cType === 'vert') {
        let tumaterial = new THREE.MeshBasicMaterial({
          color: this.getColorHSL(NumColor * 40), //color: 0xff0000
          //color: 0x0000ff
        });
      } else {
        let tumaterial = new THREE.MeshBasicMaterial({
          color: this.getColorHSL(NumColor * 40), //color: 0xff0000
          //color: 0xff0000
        });
      }

      if (cType === 'vert') {
        let xOffset = 0;
        let yOffset = cellSize / 2;
      } else {
        let xOffset = -cellSize / 2;
        let yOffset = 0;

      }
      let zOffset = 0;
      // (zMin - zLevel);

      let plane = new THREE.Mesh(geometry, tumaterial);

      plane.position.set(xOffset, yOffset, zOffset);
      this.scene.add(plane);
    },
    async build2DProfile(iCell, jCell, cType, seismType) {
      try {
        vm = this;
        //   this.buildCheck();

        if (cType === 'vert') {
          this.strTempFromApi = await PortalApi.Get2DLayer("i", iCell);
        } else {
          this.strTempFromApi = await PortalApi.Get2DLayer("j", jCell);
        }

        let zMin = this.dCommonZLevel; //this.getMin(this.strTempFromApi, 'zCoord');
        //let zMin = this.getMin(this.strTempFromApi, 'zCoord');
        let zMinLevel = zMin - 2;
        let sallLayers = this.uniqueLayers(this.strTempFromApi);

        let iCurNUm = 1;

        for (let index in sallLayers) {
          let curLayer = sallLayers[index];
          let curCoords = this.strTempFromApi.filter(d => d.sLayerName === curLayer.sLayerName);
          this.build2DProfOne(curCoords, zMinLevel, cType, iCurNUm, iCell, jCell);
          iCurNUm++;
        }

      } catch (error) {
        console.log(error);
      }
    },
    async build2DProfOne(arrData, zLevel, cType, NumColor, iCell, jCell) {
      vm = this;

      let colMin = this.getMin(arrData, 'dcol');
      let colMax = this.getMax(arrData, 'dcol');
      let colMid = colMax - colMin;

      let rowMin = this.getMin(arrData, 'drow');
      let rowMax = this.getMax(arrData, 'drow');
      let rowMid = rowMax - rowMin;


      let zMin = this.dCommonZLevel; //this.getMin(arrData, 'zCoord');
      let zMax = this.getMax(arrData, 'zCoord');
      let zMid = zMax - zMin;
      let scaleZKoef = -0.1; // 100 / zMid;
      let cellSize = 2 * this.gridSize / (this.gridCount * 2);
      let gridHalf = this.gridSize / 2;
      //  let zOffset = (zMin - zLevel);
      let arrLinePoints = [];
      let oneStep = this.gridSize / arrData.length;


      arrData.forEach(element => {
        let sd = (zMin - element.zCoord) * scaleZKoef * 10;

        if (cType === 'vert') {
          let curVector = new THREE.Vector3(-gridHalf + cellSize * iCell - (cellSize / 2), gridHalf - oneStep * element.dcol, sd);
          arrLinePoints.push(curVector);
        } else {
          let curVector = new THREE.Vector3(-gridHalf + oneStep * element.drow, gridHalf - cellSize * jCell + (cellSize / 2), sd);
          arrLinePoints.push(curVector);
        }

      });
      let curve = new THREE.CatmullRomCurve3(arrLinePoints);

      let geometry = new THREE.TubeGeometry(curve, 10, 0.2, 8, false);

      if (cType === 'vert') {
        let tumaterial = new THREE.MeshBasicMaterial({
          color: this.getColorHSL(NumColor * 40), //color: 0xff0000
          //color: 0x0000ff
        });
      } else {
        let tumaterial = new THREE.MeshBasicMaterial({
          color: this.getColorHSL(NumColor * 40), //color: 0xff0000
          //color: 0xff0000
        });
      }

      if (cType === 'vert') {
        let xOffset = 0;
        let yOffset = cellSize / 2;
      } else {
        let xOffset = -cellSize / 2;
        let yOffset = 0;

      }
      let zOffset = 0;
      (zMin - zLevel);

      let plane = new THREE.Mesh(geometry, tumaterial);

      plane.position.set(xOffset, yOffset, zOffset);
      this.scene.add(plane);
    },
    buildSurf2w(arrData, zLevel) {
      vm = this;
      let thissurfData = [];
      let colMin = this.getMin(arrData, 'dcol');
      let colMax = this.getMax(arrData, 'dcol');
      let colMid = colMax - colMin;

      let rowMin = this.getMin(arrData, 'drow');
      let rowMax = this.getMax(arrData, 'drow');
      let rowMid = rowMax - rowMin;

      let zMin = zLevel; //this.dCommonZLevel; //this.getMin(arrData, 'zCoord');
      let zMax = this.getMax(arrData, 'zCoord');
      let zMid = zMax - zMin;
      let scaleZKoef = -0.05; // 100 / zMid;
      let cellSize = 2 * this.gridSize / (this.gridCount * 2);
      var geometry = new THREE.PlaneBufferGeometry(cellSize * (colMid + 1), cellSize * (rowMid + 1), colMid, rowMid);


      let pos = geometry.getAttribute("position");
      let pa = pos.array;
      let fOl
      for (let j = 0; j <= geometry.parameters.heightSegments; j++) {
        for (let i = 0; i <= geometry.parameters.widthSegments; i++) {
          let fndVal = arrData.find(d => d.dcol === (colMin + i) && d.drow === (rowMin + j));
          let sd = (zMin - fndVal.zCoord) * scaleZKoef * 10;
          //let sd = (zMin - element.zCoord) * scaleZKoef * 10;
          //+0 is x, +1 is y.
          pa[3 * (j * (geometry.parameters.widthSegments + 1) + i) + 2] = sd;

        }
      }
      pos.needsUpdate = true;
      geometry.computeVertexNormals();

      let material = new THREE.MeshPhongMaterial({
        //  ambient: 0x0ff030,
        //color: 0xff0000,
        color: this.getColorHSL(this.drSurfCount * 40),
        specular: 0x000000,
        shininess: 20,
        side: THREE.DoubleSide
      });
      /*
                  let material = new THREE.MeshBasicMaterial({
                 color: 0xffff00,
                     side: THREE.DoubleSide
                     //,               vertexColors: THREE.VertexColors
                  });
      */

      let plane = new THREE.Mesh(geometry, material);
      plane.doubleSided = true;
      plane.castShadow = true;
      plane.receiveShadow = true;

      plane.geometry.computeVertexNormals();
      //plane.geometry.mergeVertices();


      let zOffset = 0; // (zMin - zLevel);
      let xOffset = ((1 + colMid) / 2) * cellSize + ((colMin - 1) * cellSize) - this.gridSize / 2;
      let yOffset = this.gridSize / 2 - ((1 + rowMid) / 2) * cellSize - ((rowMin - 1) * cellSize);
      plane.position.set(xOffset, yOffset, zOffset);

      //plane.rotation.x = Math.PI / 2;

      this.scene.add(plane);

      this.drSurfCount += 1;
    },
    async build2DProfOne_bkp(arrData, zLevel, cType, NumColor, iCell, jCell) {
      vm = this;

      let colMin = this.getMin(arrData, 'dcol');
      let colMax = this.getMax(arrData, 'dcol');
      let colMid = colMax - colMin;

      let rowMin = this.getMin(arrData, 'drow');
      let rowMax = this.getMax(arrData, 'drow');
      let rowMid = rowMax - rowMin;


      let zMin = this.dCommonZLevel; //this.getMin(arrData, 'zCoord');
      let zMax = this.getMax(arrData, 'zCoord');
      let zMid = zMax - zMin;
      let scaleZKoef = -0.1; // 100 / zMid;
      let cellSize = 2 * this.gridSize / (this.gridCount * 2);
      let gridHalf = this.gridSize / 2;
      //  let zOffset = (zMin - zLevel);
      let arrLinePoints = [];
      let oneStep = this.gridSize / arrData.length;


      arrData.forEach(element => {
        let sd = (zMin - element.zCoord) * scaleZKoef * 10;

        if (cType === 'vert') {
          let curVector = new THREE.Vector3(-gridHalf + cellSize * iCell - (cellSize / 2), gridHalf - oneStep * element.dcol, sd);
          arrLinePoints.push(curVector);
        } else {
          let curVector = new THREE.Vector3(-gridHalf + oneStep * element.drow, gridHalf - cellSize * jCell + (cellSize / 2), sd);
          arrLinePoints.push(curVector);
        }

      });
      let curve = new THREE.CatmullRomCurve3(arrLinePoints);

      let geometry = new THREE.TubeGeometry(curve, 10, 0.2, 8, false);

      if (cType === 'vert') {
        let tumaterial = new THREE.MeshBasicMaterial({
          color: this.getColorHSL(NumColor * 40), //color: 0xff0000
          //color: 0x0000ff
        });
      } else {
        let tumaterial = new THREE.MeshBasicMaterial({
          color: this.getColorHSL(NumColor * 40), //color: 0xff0000
          //color: 0xff0000
        });
      }

      if (cType === 'vert') {
        let xOffset = 0;
        let yOffset = cellSize / 2;
      } else {
        let xOffset = -cellSize / 2;
        let yOffset = 0;

      }
      let zOffset = 0;
      (zMin - zLevel);

      let plane = new THREE.Mesh(geometry, tumaterial);

      plane.position.set(xOffset, yOffset, zOffset);
      this.scene.add(plane);
    },
    async builSurfObj(dCol, dRow, NumColor, objType) {
      vm = this;

      let zMinLevel = this.dCommonZLevel;

      let cellSize = 2 * this.gridSize / (this.gridCount * 2);
      let gridHalf = this.gridSize / 2;
      let xOffset = -gridHalf + cellSize * dCol - (cellSize / 2);
      let yOffset = gridHalf - cellSize * dRow + (cellSize / 2);
      let scaleZKoef = 0.3;

      // let fbxloader = new THREE.GLTFLoader();
      //let loader = new GLTFLoader();
      /*
            fbxloader.load('tube.json', function (geometry) {
              this.scene.add(geometry);
              // if the model is loaded successfully, add it to your scene here
            }, undefined, function (err) {
              console.error(err);
            });
      */
      let curve = new THREE.CatmullRomCurve3([
        new THREE.Vector3(1, 1, 20),
        new THREE.Vector3(1, 1, -20)
      ]);
      let geometry = new THREE.TubeGeometry(curve, 5, 1.1, 8, false);
      let tumaterial = new THREE.MeshBasicMaterial({
        //color: this.getColorHSL(NumColor * 40)
        // color: this.setHexColor(0xff7043)
      });
      tumaterial.color.setHex(0xff2190);
      let plane = new THREE.Mesh(geometry, tumaterial);
      let zOffset = 0;
      plane.position.set(xOffset, yOffset, zOffset);
      this.scene.add(plane);


    },
    async buildWell(dCol, dRow, NumColor, drilldeep, tappedLevels) {
      vm = this;

      let zMinLevel = this.dCommonZLevel;

      let cellSize = 2 * this.gridSize / (this.gridCount * 2);
      let gridHalf = this.gridSize / 2;
      let xOffset = -gridHalf + cellSize * dCol - (cellSize / 2);
      let yOffset = gridHalf - cellSize * dRow + (cellSize / 4);
      let scaleZKoef = 0.3;
      var geometry
      var plane

      if (tappedLevels.length === 0) {
        let curve = new THREE.CatmullRomCurve3([
          new THREE.Vector3(1, 1, this.dSurfTop),
          new THREE.Vector3(1, 1, -20)
        ]);
        geometry = new THREE.TubeGeometry(curve, 5, 1.1, 8, false);
        let tumaterial = new THREE.MeshBasicMaterial({
          //color: this.getColorHSL(NumColor * 40)
          // color: this.setHexColor(0xff7043)
        });
        tumaterial.color.setHex(0xff7043);
        plane = new THREE.Mesh(geometry, tumaterial);
        let zOffset = 0;
        plane.position.set(xOffset, yOffset, zOffset);
        this.scene.add(plane);
      } else {
        let levDeep = -10;
        let levDeepPrev = -10;
        let sTTw = 0;
        for (let il = 0; il < tappedLevels.length; il++) {
          sTTw = (-1) * zMinLevel - tappedLevels[il].yLevelMetr;

          levDeep = sTTw * scaleZKoef;
          let curve = new THREE.CatmullRomCurve3([
            new THREE.Vector3(1, 1, levDeep),
            new THREE.Vector3(1, 1, levDeepPrev)
          ]);
          geometry = new THREE.TubeGeometry(curve, 5, 1.1, 8, false);
          let tumaterial = new THREE.MeshBasicMaterial({
            //  color: this.getColorHSL((il+1) * 40)
          });
          tumaterial.color.setHex(0xff7043);

          plane = new THREE.Mesh(geometry, tumaterial);
          let zOffset = 0;
          plane.position.set(xOffset, yOffset, zOffset);
          this.scene.add(plane);

          geometry = new THREE.PlaneGeometry(8, 8, 1);
          let material = new THREE.MeshBasicMaterial({
            // color:this.getColorHSL((il+1) * 4),
            side: THREE.DoubleSide
          });
          material.color.setHex(0x494f54);
          //#494f54

          plane = new THREE.Mesh(geometry, material);
          let zLevelOffset = levDeep;
          plane.position.set(xOffset, yOffset, zLevelOffset);
          this.scene.add(plane);

          levDeepPrev = levDeep;
        }

        let curve = new THREE.CatmullRomCurve3([
          new THREE.Vector3(1, 1, 120),
          new THREE.Vector3(1, 1, levDeepPrev)
        ]);
        geometry = new THREE.TubeGeometry(curve, 5, 1.1, 8, false);
        let tumaterial = new THREE.MeshBasicMaterial({
          //color: this.getColorHSL(tappedLevels.length+4)
        });
        tumaterial.color.setHex(0xff7043);
        plane = new THREE.Mesh(geometry, tumaterial);
        let zOffset = 0;
        plane.position.set(xOffset, yOffset, zOffset);
        this.scene.add(plane);

      }
    },
    async buildWell5(dCol, dRow, NumColor, drilldeep, tappedLevels, wellitem) {
      vm = this;

      let zMinLevel = this.dCommonZLevel;

      let cellSize = 2 * this.gridSize / (this.gridCount * 2);
      let gridHalf = this.gridSize / 2;
      let xOffset = -gridHalf + cellSize * dCol - (cellSize / 2);
      let xOffset2 = xOffset;

      let zOffset = 0;
      let zVert = 2100 - (-1) * wellitem.drilldeep

      if (wellitem !== null && wellitem.toeI) {
        xOffset2 = cellSize * (wellitem.toeI - dCol);
      }
      //let yOffset = gridHalf - cellSize * dRow + (cellSize / 2);
      let yOffset = gridHalf - cellSize * dRow + (cellSize / 4);
      let yOffset2 = yOffset;
      if (wellitem !== null && wellitem.toeI) {
        yOffset2 = cellSize * (dRow - wellitem.toeJ);
      }
      let scaleZKoef = 0.3;

      if (tappedLevels.length === 0) {
        let curve1 = new THREE.CatmullRomCurve3([
          new THREE.Vector3(1, 1, this.dSurfTop),
          new THREE.Vector3(1, 1, zVert)
        ]);
        let geometry = new THREE.TubeGeometry(curve1, 5, 1.1, 8, false);
        let tumaterial = new THREE.MeshBasicMaterial({
          //color: this.getColorHSL(NumColor * 40)
          // color: this.setHexColor(0xff7043)
        });
        tumaterial.color.setHex(0xff7043);
        let plane1 = new THREE.Mesh(geometry, tumaterial);

        plane1.position.set(xOffset, yOffset, zOffset);
        this.scene.add(plane1);

        let curve2 = new THREE.CatmullRomCurve3([
          new THREE.Vector3(1, 1, zVert),
          new THREE.Vector3(xOffset2, yOffset2, 5)
        ]);
        let geometry2 = new THREE.TubeGeometry(curve2, 5, 1.1, 8, false);
        let tumaterial2 = new THREE.MeshBasicMaterial({
          //color: this.getColorHSL(NumColor * 40)
          // color: this.setHexColor(0xff7043)
        });
        tumaterial2.color.setHex(0xE99046);
        let plane2 = new THREE.Mesh(geometry2, tumaterial2);
        plane2.position.set(xOffset, yOffset, zOffset);
        this.scene.add(plane2);


      } else {
        let levDeep = -10;
        let levDeepPrev = -10;
        let sTTw = 0;

        var geometry
        var plane

        for (let il = 0; il < tappedLevels.length; il++) {
          sTTw = (-1) * zMinLevel - tappedLevels[il].yLevelMetr;
          levDeep = sTTw * scaleZKoef;
          let curve = new THREE.CatmullRomCurve3([
            new THREE.Vector3(1, 1, levDeep),
            new THREE.Vector3(1, 1, levDeepPrev)
          ]);
          geometry = new THREE.TubeGeometry(curve, 5, 1.1, 8, false);
          let tumaterial = new THREE.MeshBasicMaterial({
            //  color: this.getColorHSL((il+1) * 40)
          });
          tumaterial.color.setHex(0xff7043);

          let zOffset = 0;

          plane = new THREE.Mesh(geometry, tumaterial);
          plane.position.set(xOffset, yOffset, zOffset);
          this.scene.add(plane);

          geometry = new THREE.PlaneGeometry(8, 8, 1);
          let material = new THREE.MeshBasicMaterial({
            // color:this.getColorHSL((il+1) * 4),
            side: THREE.DoubleSide
          });
          material.color.setHex(0x494f54);

          plane = new THREE.Mesh(geometry, material);
          let zLevelOffset = levDeep;
          plane.position.set(xOffset, yOffset, zLevelOffset);
          this.scene.add(plane);

          levDeepPrev = levDeep;
        }

        let curve = new THREE.CatmullRomCurve3([
          new THREE.Vector3(1, 1, 120),
          new THREE.Vector3(1, 1, levDeepPrev)
        ]);
        geometry = new THREE.TubeGeometry(curve, 5, 1.1, 8, false);
        let tumaterial = new THREE.MeshBasicMaterial({
          //color: this.getColorHSL(tappedLevels.length+4)
        });
        tumaterial.color.setHex(0xff7043);
        plane = new THREE.Mesh(geometry, tumaterial);
        let zOffset = 0;
        plane.position.set(xOffset, yOffset, zOffset);
        this.scene.add(plane);
      }
    },
    getMin: function (thisdata, sParam) {
      return thisdata.reduce((min, p) => p[sParam] < min ? p[sParam] : min, thisdata[0][sParam]);
    },
    getMax: function (thisdata, sParam) {
      return thisdata.reduce((max, p) => p[sParam] > max ? p[sParam] : max, thisdata[0][sParam]);
    },
    makeTextSprite(message, parameters) {
      if (parameters === undefined) parameters = {};

      let fontface = parameters.hasOwnProperty("fontface") ?
        parameters["fontface"] : "Arial";

      let fontsize = parameters.hasOwnProperty("fontsize") ?
        parameters["fontsize"] : 14;

      let borderThickness = parameters.hasOwnProperty("borderThickness") ?
        parameters["borderThickness"] : 1;

      let borderColor = parameters.hasOwnProperty("borderColor") ?
        parameters["borderColor"] : {
          r: 0,
          g: 0,
          b: 0,
          a: 1.0
        };

      let backgroundColor = parameters.hasOwnProperty("backgroundColor") ?
        parameters["backgroundColor"] : {
          r: 255,
          g: 255,
          b: 255,
          a: 1.0
        };
      //let spriteAlignment = THREE.SpriteAlignment.topLeft;

      let canvas = document.createElement('canvas');
      let context = canvas.getContext('2d');
      context.font = "Bold " + fontsize + "px " + fontface;

      // get size data (height depends only on font size)
      let metrics = context.measureText(message);
      let textWidth = metrics.width;

      // background color
      context.fillStyle = "rgba(" + backgroundColor.r + "," + backgroundColor.g + "," +
        backgroundColor.b + "," + backgroundColor.a + ")";
      // border color
      context.strokeStyle = "rgba(" + borderColor.r + "," + borderColor.g + "," +
        borderColor.b + "," + borderColor.a + ")";

      context.lineWidth = borderThickness;
      this.roundRect(context, borderThickness / 2, borderThickness / 2, textWidth + borderThickness, fontsize * 1.4 + borderThickness, 6);
      // 1.4 is extra height factor for text below baseline: g,j,p,q.

      // text color
      context.fillStyle = "rgba(0, 0, 0, 1.0)";
      context.fillText(message, borderThickness, fontsize + borderThickness);

      // canvas contents will be used for a texture
      let texture = new THREE.Texture(canvas)
      texture.needsUpdate = true;
      let spriteMaterial = new THREE.SpriteMaterial({
        map: texture,
        useScreenCoordinates: true,
      });
      let sprite = new THREE.Sprite(spriteMaterial);
      sprite.scale.set(100, 50, 1.0);
      return sprite;
    },
    roundRect(ctx, x, y, w, h, r) {
      ctx.beginPath();
      ctx.moveTo(x + r, y);
      ctx.lineTo(x + w - r, y);
      ctx.quadraticCurveTo(x + w, y, x + w, y + r);
      ctx.lineTo(x + w, y + h - r);
      ctx.quadraticCurveTo(x + w, y + h, x + w - r, y + h);
      ctx.lineTo(x + r, y + h);
      ctx.quadraticCurveTo(x, y + h, x, y + h - r);
      ctx.lineTo(x, y + r);
      ctx.quadraticCurveTo(x, y, x + r, y);
      ctx.closePath();
      ctx.fill();
      ctx.stroke();
    },
    // prepareSceneMin() {
    //   vm = this;
    //   // scene, camera, renderer
    //   this.curObj = this.obj;
    //   let container, renderer, controls;
    //
    //   this.scene = new THREE.Scene();
    //   let SCREEN_WIDTH = 1200,
    //     SCREEN_HEIGHT = 800;
    //
    //   // let axes = new THREE.AxisHelper(100);
    //   // this.scene.add(axes);
    //   this.camera = new THREE.PerspectiveCamera(45, SCREEN_WIDTH / SCREEN_HEIGHT, 0.1, 1000);
    //   this.camera.position.set(180, 120, 100);
    //   //camera.position.set( 15, 10, - 15 );
    //
    //
    //   renderer = new THREE.WebGLRenderer({
    //     antialias: true
    //   });
    //   renderer.setSize(SCREEN_WIDTH, SCREEN_HEIGHT);
    //   container = document.getElementById('stage');
    //
    //   let positions = [
    //     [1, 1, 1],
    //     [-1, -1, 1],
    //     [-1, 1, 1],
    //     [1, -1, 1]
    //   ];
    //   //  this.scene.add(new THREE.AmbientLight(0x111111));
    //   for (let i = 0; i < 4; i++) {
    //     let light = new THREE.DirectionalLight(0xdddddd);
    //     light.position.set(positions[i][0], positions[i][1], 0.4 * positions[i][2]);
    //     this.scene.add(light);
    //   }
    //
    //   renderer.setClearColor(0xEEEEEE, 1.0);
    //   // Append to DOM
    //   orbitControls = new OrbitControls(this.camera, renderer.domElement);
    //   this.$refs.canvasContainer.appendChild(renderer.domElement);
    //   this.$refs.canvasContainer.addEventListener('dblclick', this.onMouseClick, false);
    //   //draw scene grid
    //
    //
    //   this.scene.rotation.x = -Math.PI / 2;
    //   /*
    //         let spritey = this.makeTextSprite( " i 24 j 1 ", { fontsize: 10,   } );
    //         spritey.position.set(150,100,-20);
    //         this.scene.add( spritey );
    //   */
    //   //arrow nord
    //   let radius = 2;
    //   let height = 18.0;
    //   let radialSegments = 21;
    //   let geometry = new THREE.ConeBufferGeometry(radius, height, radialSegments);
    //
    //   let tumaterial = new THREE.MeshBasicMaterial({
    //     color: 0x0000ff
    //   });
    //   let plane = new THREE.Mesh(geometry, tumaterial);
    //   plane.position.set(105, 5, 0);
    //   //  this.scene.add(plane);
    //   // this.projector = new THREE.Projector();
    //   let animate = function () {
    //     requestAnimationFrame(animate);
    //     renderer.render(vm.scene, vm.camera);
    //
    //
    //   };
    //   animate();
    //
    //   this.bShowScene = true;
    // },
    prepareScene() {
      vm = this;
      // scene, camera, renderer
      this.curObj = this.obj;
      let container, renderer, controls;

      this.scene = new THREE.Scene();
      let SCREEN_WIDTH = 1200,
        SCREEN_HEIGHT = 800;

      let axes = new THREE.AxisHelper(100);
      this.scene.add(axes);
      this.camera = new THREE.PerspectiveCamera(45, SCREEN_WIDTH / SCREEN_HEIGHT, 0.1, 1000);
      this.camera.position.set(180, 120, 100);
      //camera.position.set( 15, 10, - 15 );


      renderer = new THREE.WebGLRenderer({
        antialias: true
      });
      renderer.setSize(SCREEN_WIDTH, SCREEN_HEIGHT);
      container = document.getElementById('stage');

      let positions = [
        [1, 1, 1],
        [-1, -1, 1],
        [-1, 1, 1],
        [1, -1, 1]
      ];
      //  this.scene.add(new THREE.AmbientLight(0x111111));
      for (let i = 0; i < 4; i++) {
        let light = new THREE.DirectionalLight(0xdddddd);
        light.position.set(positions[i][0], positions[i][1], 0.4 * positions[i][2]);
        this.scene.add(light);
      }

      renderer.setClearColor(0xEEEEEE, 1.0);
      // Append to DOM
      orbitControls = new OrbitControls(this.camera, renderer.domElement);
      this.$refs.canvasContainer.appendChild(renderer.domElement);
      this.$refs.canvasContainer.addEventListener('dblclick', this.onMouseClick, false);
      //draw scene grid

      let gridHelper = new GridHelper(this.gridSize, this.gridCount, 0x999999, 0xbbbbbb);
      //let gridHelper = new THREE.GridHelper(10, 4);
      gridHelper.rotation.x = -Math.PI / 2;

      this.scene.add(gridHelper);
      this.scene.rotation.x = -Math.PI / 2;
      let pmremGenerator = new THREE.PMREMGenerator(renderer);
      pmremGenerator.compileEquirectangularShader();

      //arrow nord
      let radius = 2;
      let height = 18.0;
      let radialSegments = 21;
      let geometry = new THREE.ConeBufferGeometry(radius, height, radialSegments);

      let tumaterial = new THREE.MeshBasicMaterial({
        color: 0x0000ff
      });
      let plane = new THREE.Mesh(geometry, tumaterial);
      plane.position.set(105, 5, 0);
      this.scene.add(plane);
      // this.projector = new THREE.Projector();
      let animate = function () {
        requestAnimationFrame(animate);
        renderer.render(vm.scene, vm.camera);


      };
      animate();

      this.bShowScene = true;
    },
    // setHexColor: function (hexcolor) {
    //   let color = new THREE.Color();
    //   color.setColor(hexcolor);
    //   return color;
    // },
    getColorHSL: function (i) {
      let hue = i * 1.2 / 360;
      let color = new THREE.Color();
      color.setHSL(hue, 1, .5);
      return color;
    },
    uniqueLayers(inpArray) {
      return inpArray.reduce((seed, current) => {
        return Object.assign(seed, {
          [current.sLayerName]: current
        });
      }, {});
    },
    // async LoadDBData2Storage() {
    //   try {
    //     this.megaObj = this.secLS.get('ocData');
    //     let curUserid = this.megaObj.userid || -1;
    //     console.log(curUserid)
    //     if (curUserid > -1) {
    //       await PortalApi.DoLoadCase();
    //     } else {
    //
    //     }
    //   } catch (e) {
    //     console.log('LoadDBData2Storage error:' + e)
    //   }
    // },
    async doSendUserScreen(sdsd) {

      await PortalApi.SendScreenShot(sdsd).then((resp) => {
        return resp;
      })
    },
    async getArrDictParams() {
      await PortalApi.GetDictValues().then((response) => {
        if (response.status === 200) {
          this.arrDictParams = response.data;
          this.secLS.set('ocDictList', this.arrDictParams)
        }
      });

    },
    async doCheckDictHash() {
      try {
        await PortalApi.getDictHash(0).then(async resp => {
          if (resp.status == 200) {
            let localHash = localStorage.getItem("ocDictHash");
            if (localHash !== resp.data || !this.secLS.ls["ocDictList"]) {
              await this.getArrDictParams().then(r => {
                return "doCheckDictHash";
              });

              localStorage.setItem("ocDictHash", resp.data);
            } else {
              this.arrDictParams = this.secLS.get('ocDictList');
            }

          } else {
            return 'doCheckDictHash error';
          }

        }).catch(err => {
          return 'doCheckDictHash error ' + err;
        })


      } catch (err) {
        return 'getProfileHash error ' + err;
      }
    },
    async SaveStorage2DBData(iGameStep) {
      // Увеличивать шаг на сервере
      await PortalApi.DoSaveCase(iGameStep);
    },

    async doLoadCaseData() {
      return
      if (this.secLS === null || this.secLS.ls['ocData'] === null) {
        return;
      }
      this.megaObj = this.secLS.get('ocData');
      if (this.megaObj !== null) {
        if (this.megaObj.arrDrillsList !== null && this.megaObj.arrDrillsList.length > 0) {
          this.arrDrillsList = JSON.parse(this.megaObj.arrDrillsList);
        }
        if (this.megaObj.arr2DProfileList !== null && this.megaObj.arr2DProfileList.length > 0) {
          this.arr2DProfileList = JSON.parse(this.megaObj.arr2DProfileList);
        }
        if (this.megaObj.arr3DProfileList !== null && this.megaObj.arr3DProfileList.length > 0) {
          this.arr3DProfileList = JSON.parse(this.megaObj.arr3DProfileList);
        }
        if (this.megaObj.arrSurfObjList !== null && this.megaObj.arrSurfObjList.length > 0) {
          this.arrSurfObjList = JSON.parse(this.megaObj.arrSurfObjList);
        }
        if (this.megaObj.arrCommon !== null && typeof this.megaObj.arrCommon === 'string'.startsWith('{')) {
          this.arrCommon = JSON.parse(this.megaObj.arrCommon);
        } else {
          if (typeof this.megaObj.arrCommon === "object") {
            this.arrCommon = this.megaObj.arrCommon;
          }
        }
      }

      if (this.arrDrillsList.length > 0 || this.arr2DProfileList.length > 0 || this.arr3DProfileList.length > 0) {
        this.bStorageDataEmpty = false;
        this.dCommonZLevel = -2850;
        this.dCommonZLevelSeism1 = -2850;
        this.dCommonZLevelSeism2 = -2550;
      } else {
        this.bStorageDataEmpty = true;
      }
    },
  }
}
