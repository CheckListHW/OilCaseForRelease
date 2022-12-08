<template>
  <div style="overflow-x:scroll;width:1200px;">
    <q-toggle v-model="bModeOtbiv" label="установка уровней"/>
    <div>
      <v-stage ref="stage" :config="configKonva" @mousedown="handleStageMouseDown" @mousemove="handleMouseMove">
        <v-layer>
          <v-rect v-for="item in FaciesRects" :key="item.rnum" :config="item"/>
        </v-layer>
        <v-layer ref="layerCellLines1">
          <v-rect
            :config="{ x: 60, y: curMouseYPos, width: getGraphWidth, height: 1, fill: 'black', visible: bModeOtbiv === true && curMouseYPos > getTopPadding }"/>
        </v-layer>
        <v-layer ref="layerCellLines3">
          <v-line v-for="item in arrYScale" :key="item.idx" :config="item"></v-line>
          <v-text v-for="item in arrYText" :key="item.idx" :config="item"/>
          <v-rect :config="{ x: 0, y: getTopPadding, width: getGraphWidth, height: 1, fill: 'black' }"/>
        </v-layer>
        <v-layer ref="layerAxisTopLabel">
          <v-text v-for="item1 in ArffPoints4Graph" :config="item1.scaleTopTitle1" :key="item1.scaleTopTitle1.idx">
          </v-text>
          <v-text v-for="item2 in ArffPoints4Graph" :config="item2.scaleTopTitle2" :key="item2.scaleTopTitle2.idx">
          </v-text>
        </v-layer>
        <v-layer>
          <v-line v-for="item in ArffPoints4Graph"
                  :config="{ x: item.xLeft, y: getTopPadding,
                    points: item.graphLinePoints, tension: 0.7,
                    stroke: item.lineColor, strokeWidth: 1.8 }"
                  :key="item.idx"></v-line>
        </v-layer>
        <v-layer ref="layerCellLines4">
          <v-rect v-for="item in arrTappedLevelsList" :key="item.DisplayConfig.idx" :config="item.DisplayConfig"/>
          <v-text v-for="item in arrTappedLevelsList" :key="item.DisplayLabel.idx" :config="item.DisplayLabel"
                  @mousedown="handleLevelTextMouseDown"/>
        </v-layer>
      </v-stage>
    </div>

    <q-dialog v-model="levelDialogModel" prevent-close>
      <span slot="title">{{ sModalTitle }}</span>
      <span slot="message">Название уровня для отметки <b>{{ tmpAddLvelItem.yLevelMetr }}</b>
            <p class="q-pt-md q-mb-sm"><b>Значения</b></p>
            <span v-for="item1 in tmpAddLvelItem.xValArr" :key="`${item1.skey}`">
               {{ item1.skey }} - {{ item1.xVal }}<br/>
            </span>
         </span>
      <div slot="body">
        <q-field label="Название уровня" :label-width="4">
          <q-input v-model="CurLevelName"/>
        </q-field>
      </div>
      <template slot="buttons" slot-scope="props">
        <q-btn v-show="sModalTitle === 'Параметры уровня'" color="negative" label="удалить"
               @click="delLevel(props.ok)"/>
        <q-btn flat label="отмена" @click="levelDialogModel = false"/>
        <q-btn color="primary" label="сохранить" @click="choose(props.ok)"/>
      </template>
    </q-dialog>
    <q-inner-loading :visible="loadProc">
      <q-spinner-gears size="50px" color="primary"></q-spinner-gears>
    </q-inner-loading>

  </div>
</template>

<script>
import EventBus from 'src/event-bus';
import store from "src/store";
import {UserActions} from "src/data/ObjectTemplates/Dialogs";
import OilcaseApi from "src/api/OilCaseApi.js";

const width = window.innerWidth * 2;
const height = window.innerHeight * 0.75;
const TopPadding = 80;
const yAxisStep = 70;
const xAxisWidth = 150;
const arrColors = ['Indigo', 'green', 'Maroon', 'blue', 'purple', 'orange', 'BlueViolet', '#DC143C'];

let vm = {};

export default {
  name: 'showGis',
  props: ['chartInp', 'chartSettings', 'boreholeName'],

  data() {
    return {
      // boreholeName: null,
      yMin: null,
      yMax: null,
      configKonva: {
        width: width,
        height: height
      },
      arrTappedLevelsList: [],
      percentPaddingGisOnPlot: 0.05,
      levelDialogModel: false,
      FaciesRects: [],
      imodelmode: 1,
      sModalTitle: '',
      strTempFromApi: [],
      tmpAddLvelItem: {},
      arrCellsList: [],
      arrYScale: [],
      arrYText: [],
      curMouseYPos: -1,
      getGraphWidth: 0,
      loadProc: false,
      grdataPoro: [{
        zcoord: -2749.90014648438,
        facies: 0,
        dporo: 0.0,
        gisParam1: 0.0,
        gisParam2: 0.0,
        gisParam3: 0.0,
        gisParam4: 0.0,
        gisParam5: 0.0,
        gisParam8: 0.0,
        gisParam20: 0.0,
        rigisParam1: 0.0,
        rigisParam2: 0.0,
        rigisParam3: 0.0,
        rigisParam4: 0.0,
      }],
      ArffPoints4Graph: [],
      CurLevelName: '',
      bModeOtbiv: false,
      DictWellIssls: [],
      selGraphYpes: ['gisParam1', 'gisParam2', 'gisParam4', 'gisParam5', 'gisParam6', 'gisParam8', 'gisParam9', 'gisParam20',
        'rigisParam1', 'rigisParam2', 'rigisParam3', 'rigisParam4'],
      curWellIssl: [],
      msgData: 'msgData',
      count: 0,

    }
  },


  created() {
    EventBus.$on('doOpenWellLevels', item => {
      if (this.boreholeName === item.name) {
        let gisData = new Map()
        console.log(item)
        item.boreholeLog.forEach(item => {
          gisData.set(item.name, {
            'values': item.values,
            'positions': item.positions
          })
        })
        this.doOpenWellLevels(gisData);
      }
    });
  },

  destroyed() {
    EventBus.$off('doOpenWellLevels');
  },

  methods: {
    async doOpenWellLevels(item) {
      this.loadProc = true;
      this.buildChart(item);
      this.apiloadvisible = true;
      this.loadProc = false;
      EventBus.$emit('doLoadedWellsData', item)
    },

    buildChart(tempData) {
      vm = this;
      vm.ArffPoints4Graph = [];
      let idxMax = 0;
      let fixedLongValue = (val) => Math.abs(val) < 100 ? val.toFixed(2) : Math.round(val);

      // Установка шапок столбцов
      Array.from(tempData.keys()).forEach(key => {
        let arr = tempData.get(key).values
        let xMin = Math.min(...arr)
        let xMax = Math.max(...arr)

        let xDif = Math.abs(xMax - xMin)

        //percentPaddingGisOnPlot - процент отступа при рисовании линий графика
        idxMax += 1;
        vm.ArffPoints4Graph.push({
          idx: vm.ArffPoints4Graph.length,
          skey: key,
          xMin: xMin - xDif * vm.percentPaddingGisOnPlot,
          xMax: xMax + xDif * vm.percentPaddingGisOnPlot,
          xDiff: xMax - xMin + xDif * vm.percentPaddingGisOnPlot * 2,
          xLeft: 100 + xAxisWidth * (vm.ArffPoints4Graph.length),
          graphLinePoints: [],
          lineColor: arrColors[idxMax % arrColors.length],
          scaleTopTitle1: {
            idx: 'tk1' + vm.ArffPoints4Graph.length,
            x: 110 + xAxisWidth * (vm.ArffPoints4Graph.length), y: 35,
            text: key.toUpperCase(),
            fontSize: 15
          },
          scaleTopTitle2: {
            idx: 'tk2' + vm.ArffPoints4Graph.length,
            x: 110 + xAxisWidth * (vm.ArffPoints4Graph.length), y: 65,
            text: `${fixedLongValue(xMin)}  ${key}Val  ${fixedLongValue(xMax)}`,
            fontSize: 12
          }
        });

        this.arrYScale.push({
          idx: this.arrYScale.length,
          points: [100 + xAxisWidth * (vm.ArffPoints4Graph.length),
            0, 100 + xAxisWidth * (vm.ArffPoints4Graph.length), height
          ],
          stroke: 'black',
          strokeWidth: 0.5
        });
      })

      this.yMin = Math.min(...Array.from(tempData.values()).map(item => Math.min(...item.positions)))
      this.yMax = Math.max(...Array.from(tempData.values()).map(item => Math.max(...item.positions)))
      this.vdiff = this.yMax - this.yMin;

      this.arrYText = [];
      var delXP = (height - TopPadding) / yAxisStep;

      //Формирование столбца глубины

      for (let n = 0; n <= yAxisStep; n++) {
        if ((n < yAxisStep) && (n > 4) && n % 5 === 0) {
          this.arrYScale.push({
            idx: this.arrYScale.length,
            points: [55, TopPadding + n * delXP, 70, TopPadding + n * delXP],
            stroke: 'black',
            strokeWidth: 0.7
          });

          this.arrYText.push({
            idx: 'sctxt' + n,
            x: 10, y: TopPadding + n * delXP - 5,
            text: fixedLongValue(this.yMax - this.vdiff / yAxisStep * n),
            fontSize: 14
          });
        } else {
          this.arrYScale.push({
            idx: this.arrYScale.length,
            points: [65, TopPadding + n * delXP, 70, TopPadding + n * delXP],
            stroke: 'black',
            strokeWidth: 0.7
          });
        }
      }

      // линии для столбца литологии

      [[70, TopPadding, 70, height], [90, 0, 90, height]].forEach(point => {
        vm.arrYScale.push({
          idx: vm.arrYScale.length,
          points: point,
          stroke: 'black',
          strokeWidth: 0.7
        })
      })

      // заполнение значений

      this.getGraphWidth = 100 + xAxisWidth * (vm.ArffPoints4Graph.length)
      this.FaciesRects = [];
      let grPointYPrev = TopPadding;
      let Lff = tempData.get('facies') != null ? (height - TopPadding) / tempData.get('facies').values.length : null



      vm.ArffPoints4Graph.forEach(scVal => {
        let points = tempData.get(scVal.skey)
        for (let i = 0; i < points.values.length; i++) {
          let xPoint = Math.floor(scVal.xDiff !== 0 ? (points.values[i] - scVal.xMin) * xAxisWidth / scVal.xDiff : 0);
          let yPoint = Math.floor(((height - TopPadding) * (Math.abs(points.positions[i] - vm.yMax))) / vm.vdiff);
          scVal.graphLinePoints[scVal.graphLinePoints.length] = xPoint
          scVal.graphLinePoints[scVal.graphLinePoints.length] = yPoint
          if (scVal.skey === 'facies' && Lff !== null) {
            let fVal = points.values[i]
            vm.FaciesRects.push({
              rnum: vm.FaciesRects.length,
              x: 70,
              width: 20,
              fill: fVal === 0 ? '#868e96' : '#ffc107',
              y: grPointYPrev,
              height: fVal === 0 ? 0.5 : Lff
            });
            grPointYPrev = Lff + grPointYPrev;
          }


        }
      })
    },

    getNearetsItems: function (list, needle) {
      var closest = list.reduce(function (prev, curr) {
        return (Math.abs(curr.zcoord - needle) < Math.abs(prev.zcoord - needle) ? curr : prev);
      });

      if (list.indexOf(closest) === -1) {
        return {
          nearRItem: null,
          nearLItem: null
        };
      } else {
        var nearR = null;
        var nearL = null;
        if (Math.abs(closest.zcoord) > Math.abs(needle)) {
          nearR = closest;
          nearL = (list[list.indexOf(closest) - 1] !== undefined) && list[list.indexOf(closest) - 1];
        } else {
          nearR = (list[list.indexOf(closest) + 1] !== undefined) && list[list.indexOf(closest) + 1];
          nearL = closest;
        }

        return {
          nearRItem: nearR,
          nearLItem: nearL
        }
      }
    },

    handleMouseMove(event) {
      if (this.bModeOtbiv) {
        this.curMouseYPos = event.currentTarget.pointerPos.y;
      }
    },

    indexWhere(array, conditionFn) {
      const item = array.find(conditionFn)
      return array.indexOf(item)
    },

    editLevelRecord(item) {
      this.editLevelItem = item;
      this.dCurTapLevel = item.yLevelMetr;
      this.CurLevelName = item.lname;
      this.sModalTitle = 'Переименование уровня';
      this.levelDialogModel = true;
    },

    handleLevelTextMouseDown(event) {
      //event.target.className
      if (event.target.className === "Text") {
        this.tmpAddLvelItem = this.arrTappedLevelsList.find(d => d.num === event.target.attrs.id);
        this.CurLevelName = this.tmpAddLvelItem.lname;

        this.sModalTitle = 'Параметры уровня';
        this.levelDialogModel = true;
      }
    },

    handleStageMouseDown(event) {
      return
      if (this.bModeOtbiv && this.getTopPadding < event.currentTarget.pointerPos.y && (this.getGraphWidth + 40) > event.currentTarget.pointerPos.x) {
        this.dCurYPos = event.currentTarget.pointerPos.y;
        this.dCurTapLevel = (Math.abs(this.yMax) + (this.vdiff * (this.dCurYPos - TopPadding)) / (height - TopPadding)).toFixed(2);

        this.idxMax = 1;
        if (vm.arrTappedLevelsList !== null && vm.arrTappedLevelsList.length > 0) {
          var itmeMax = vm.arrTappedLevelsList.reduce(function (prev, current) {
            return (prev.num > current.num) ? prev : current
          });
          this.idxMax = itmeMax.num + 1;
        }
        this.CurLevelName = 'Top-' + this.idxMax;
        this.sModalTitle = 'Добавление уровня';

        this.tmpAddLvelItem = {};

        var iColIdx = this.idxMax % arrColors.length;
        var arrSelX = [];
        var ddF = this.getNearetsItems(this.grdataPart1, this.dCurTapLevel * -1);

        var ddX = (Math.abs(ddF.nearRItem.dporo) - Math.abs(ddF.nearLItem.dporo)) *
          (Math.abs(ddF.nearRItem.zcoord) - Math.abs(this.dCurTapLevel)) / (Math.abs(ddF.nearRItem.zcoord) - Math.abs(ddF.nearLItem.zcoord));
        var ddfx = (ddX - Math.abs(ddF.nearRItem.dporo)) * -1;
        var sValDimens = arrGraphDimenSs.find(d => d.sgrKey === 'dporo');
        arrSelX.push({
          skey: sValDimens.sgrLbl,
          xVal: ddfx.toFixed(2)
        });
        for (var key in Object.keys(ddF.nearRItem)) {
          var sSkey = Object.keys(ddF.nearRItem)[key];
          if (sSkey.includes('gisParam') && this.curWellIssl.includes(sSkey)) {
            var ddX = (Math.abs(ddF.nearRItem[sSkey]) - Math.abs(ddF.nearLItem[sSkey])) *
              (Math.abs(ddF.nearRItem.zcoord) - Math.abs(this.dCurTapLevel)) / (Math.abs(ddF.nearRItem.zcoord) - Math.abs(ddF.nearLItem.zcoord));
            var ddfx = (ddX - Math.abs(ddF.nearRItem[sSkey])) * -1;
            var sValDimens = arrGraphDimenSs.find(d => d.sgrKey === sSkey);
            arrSelX.push({
              skey: sValDimens.sgrLbl,
              xVal: ddfx.toFixed(2)
            });
          }
        }

        this.tmpAddLvelItem.num = this.idxMax;
        this.tmpAddLvelItem.lname = this.CurLevelName;
        this.tmpAddLvelItem.yPos = this.dCurYPos;
        this.tmpAddLvelItem.yLevelMetr = this.dCurTapLevel;
        this.tmpAddLvelItem.xVal = ddfx.toFixed(2);
        this.tmpAddLvelItem.xValArr = arrSelX;
        this.tmpAddLvelItem.text = this.CurLevelName;
        this.tmpAddLvelItem.fill = arrColors[iColIdx];
        this.levelDialogModel = true;
      }
    },

    async delLevel(okFn) {
      await okFn()
      this.$q.dialog({
        title: 'Подтверждение',
        message: 'Удалить уровень ' + this.tmpAddLvelItem.lname + ' ' + this.tmpAddLvelItem.yLevelMetr + ' ?',
        ok: 'Удалить',
        cancel: 'Отменить'
      }).then(() => {
        this.doCaseLogAction('Удаление уровня в скважине', 'Скважина: ' + this.chartInp.wellname + '  название уровня: ' + this.tmpAddLvelItem.lname + '   отметка уровня: ' + this.tmpAddLvelItem.yLevelMetr);
        var remIndex = this.indexWhere(this.arrTappedLevelsList, arritem => arritem.num === this.tmpAddLvelItem.num);
        this.arrTappedLevelsList.splice(remIndex, 1);

      }).catch(() => {
      });
    },

    async choose(okFn) {
      if (this.CurLevelName.length === 0) {
        this.$q.dialog({
          title: 'Предупреждение',
          message: `Необходимо указать название уровня`
        }).catch(() => {
        });
      } else {
        await okFn()
        var obj1 = null;
        if (vm.arrTappedLevelsList !== null) {
          obj1 = vm.arrTappedLevelsList.find(d => d.lname === this.CurLevelName);
        }
        if (obj1 !== null) {
          this.$q.dialog({
            title: 'Предупреждение',
            message: 'Уровень с названием ' + this.CurLevelName + ' уже существует. '
          }).catch(() => {
          });
          return;
        }
        var iLevIdx = 0;
        if (vm.arrTappedLevelsList !== null) {
          iLevIdx = vm.arrTappedLevelsList.length;
        }
        if (this.sModalTitle === 'Добавление уровня') {
          var dNObj = {
            idx: iLevIdx,
            num: this.tmpAddLvelItem.num,
            lname: this.tmpAddLvelItem.lname,
            yPos: this.tmpAddLvelItem.yPos,
            yLevelMetr: this.tmpAddLvelItem.yLevelMetr,
            xVal: this.tmpAddLvelItem.xVal,
            xValArr: this.tmpAddLvelItem.xValArr,
            wellname: this.chartInp.wellname,
            wellid: this.chartInp.wellid,
            DisplayConfig: {
              idx: 'ldc' + this.tmpAddLvelItem.num,
              x: 58,
              y: this.tmpAddLvelItem.yPos,
              width: vm.getGraphWidth + 40,
              height: 2,
              fill: this.tmpAddLvelItem.fill
            },
            DisplayLabel: {
              idx: 'ldl' + this.tmpAddLvelItem.num,
              id: this.tmpAddLvelItem.num,
              x: vm.getGraphWidth + 45,
              y: this.tmpAddLvelItem.yPos - 15,
              text: this.tmpAddLvelItem.lname,
              fontSize: 14,
              fill: this.tmpAddLvelItem.fill
            }
          };
          vm.arrTappedLevelsList.push(dNObj);
          this.addUserActionToLogAsync(UserActions.AddLevelAtBorehole(this.chartInp.wellname, dNObj.lname, dNObj.yLevelMetr))

          this.$q.notify({
            type: 'positive',
            message: `Добавлен уровень ${this.CurLevelName}`
          });
        } else {
          this.tmpAddLvelItem.lname = this.CurLevelName;
          this.tmpAddLvelItem.DisplayLabel.text = this.CurLevelName;
        }
        store.state.curWellTappedLevels = vm.arrTappedLevelsList;
      }
    },

    async addUserActionToLogAsync(action) {
      await OilcaseApi.DoLogCase(this.iUserID, action.ActionType, action.ActionData)
    },

    async doCaseLogAction(sActionType, sActionData) {
      await PortalApi.DoLogCase(this.chartInp.curUserID, sActionType, sActionData);
    },
  },

  computed: {
    getTopPadding() {
      return TopPadding;
    },

    getarrChooseGraphs() {
      var dfdf = this.DictWellIssls.slice(1);
      return dfdf.map((item) => {
        return {
          label: item.sgrLbl,
          value: item.sgrKey
        };
      });
    },
  },
}
</script>
