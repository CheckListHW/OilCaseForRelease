<template lang="html">
    <div>
       <q-toggle v-model="bModeOtbiv" label="установка уровней" />
         <div style="border: 1px solid #96979e">
            <div class="row q-pt-md gutter-md">
            <div class="col-10">
           <v-stage ref="stage" :config="configKonva" @mousedown="handleStageMouseDown" @mousemove="handleMouseMove">
                     <v-layer>
                        <v-rect v-for="item in FaciesRects" :key="item.rnum" :config="item" />
                     </v-layer>
                     <v-layer ref="layerCellLines1">
                        <v-rect :config="{x: 60,y: curMouseYPos,width: getGraphWidth,height: 1, fill: 'black',visible: bModeOtbiv === true && curMouseYPos>getTopPadding  }" />
                     </v-layer>
                     <v-layer ref="layerCellLines3">
                        <v-line v-for="item in arrYScale" :key="item.id" :config="item"></v-line>
                        <v-text v-for="item in arrYText" :key="item.id" :config="item" />
                        <v-rect :config="{x: 0,y: getTopPadding,width: getGraphWidth+40,height: 1, fill: 'black'}" />
                     </v-layer>
                     <v-layer ref="layerAxisTopLabel">
                        <v-text v-for="item1 in ArffPoints4Graph" :config="item1.scaleTopTitle1" :key="item1.scaleTopTitle1.idx"></v-text>
                        <v-text v-for="item2 in ArffPoints4Graph" :config="item2.scaleTopTitle2" :key="item2.scaleTopTitle2.idx"></v-text>
                     </v-layer>
                     <v-layer>
                        <v-line v-for="item in ArffPoints4Graph" :config="{x: item.xLeft, y: getTopPadding, points: item.graphLinePoints, tension: 0.2, stroke: item.lineColor, strokeWidth:1.8}" :key="item.idx"></v-line>
                     </v-layer>
                     <v-layer ref="layerCellLines4">
                        <v-rect v-for="item in arrTappedLevelsList" :key="item.num" :config="item.DisplayConfig" />
                        <v-text v-for="item in arrTappedLevelsList" :key="item.id" :config="item.DisplayLabel" />
                     </v-layer>
                  </v-stage>

          </div>
            <div class="col-2">
               Отмеченные уровни<br/>
          <span style="font-size:10px" v-for="item in arrTappedLevelsList" :key="`${item.num}`">
   {{item.lname}} ({{item.yLevelMetr}})
   <q-btn class="q-pa-sm"  flat  color="green-4" icon="title" @click.prevent="editLevelRecord(item)" >
   <q-tooltip>Переименовать уровень</q-tooltip></q-btn>
   <q-btn class="q-pa-sm" flat  color="red-4" icon="clear" @click.prevent="delLevelRecord(item)">
   <q-tooltip>Удалить уровень</q-tooltip>
   </q-btn>
   <span v-for="item1 in item.xValArr" :key="`${item1.skey}`">
   <br/>{{item1.skey}} - {{item1.xVal}}
   </span>
               <br/>
               </span>
          </div>
         </div>
               </div>

                  <q-dialog v-model="levelDialogModel" prevent-close>
            <!-- This or use "title" prop on <q-dialog> -->
            <span slot="title">{{sModalTitle}}</span>
            <!-- This or use "message" prop on <q-dialog> -->
            <span slot="message">Название уровня для отметки <b>{{dCurTapLevel}}</b>  </span>
            <div slot="body">
               <q-field helper="Укажите название уровня" label="Название уровня" :label-width="3">
                  <q-input v-model="CurLevelName" />
               </q-field>
            </div>
            <template slot="buttons" slot-scope="props">
                                                <q-btn flat label="отмена" @click="levelDialogModel=false" />
                                                <q-btn color="primary" label="сохранить" @click="choose(props.ok)" />
</template>
      </q-dialog>
    </div>
</template>
<script>
 const width = 1300; //window.innerWidth * 0.75; // 900;
   const height = 750; // window.innerHeight * 0.75;
   const TopPadding = 100; // window.innerHeight * 0.75;
   const yAxisStep = 100; // window.innerHeight * 0.75;
   const xAxisWidth = 150; // window.innerHeight * 0.75;
    const arrColors = ['Indigo', 'green', 'Maroon', 'blue', 'purple', 'orange', 'BlueViolet', '#DC143C'];
    const arrGraphDimenSs = [{
         sgrKey: 'dporo',
         sgrLbl: 'Porosity',
         sgrText: 'Пористость',
         sgrEdinc: 'por'
      },
      {
         sgrKey: 'gisParam1',
         sgrLbl: 'GR',
         sgrText: 'Гамма-каротаж',
         sgrEdinc: 'grVal'
      },
      {
         sgrKey: 'gisParam2',
         sgrLbl: 'PS',
         sgrText: 'самопроизвольной поляризации',
         sgrEdinc: 'grPS'
      },
      {
         sgrKey: 'gisParam3',
         sgrLbl: 'NPHI',
         sgrText: 'Нейтронный каротаж',
         sgrEdinc: 'grNK'
      },
      {
         sgrKey: 'gisParam4',
         sgrLbl: 'RDEP',
         sgrText: 'Удельное электрическое сопротивление',
         sgrEdinc: 'grIK'
      }
   ];
    let vm = {};
export default {
     props: ['grdataPoro','chartSettings'],
       name: 'WellGraphComponent',
    data () {
        return {
            FaciesRects:[],
            configKonva: {
               width: width,
               height: height
            },
            bModeOtbiv: false,
              sModalTitle: '',
            dCurTapLevel: 1.1,
            CurLevelName: '',
            dCurYPos: 0,
            levelDialogModel: false,
            editLevelItem: {},
            selCell2Di: 6,
            yMax: {},
            yMin: {},
            vdiff: 0.0,
            getGraphWidth: 0,
            GraphPoints: [],
            curMouseYPos: -1,
            arrTappedLevelsList: [],
            arrCellsList: [],
            arrYScale: [],
            arrYText: [],
            ArffPoints4Graph:[]
        }
    },
    mounted () {
    this.buildChart()
  },

 computed: {
         getTopPadding() {
            return TopPadding;
         }
      },
  watch: {
    'chartData' (to, from) {
      this.render()
    }
  },
    methods: {
         buildChart() {
            vm = this;
            vm.ArffPoints4Graph = [];
            // Подготовка данных
            this.yMin = this.grdataPoro.reduce(function(prev, current) {
               return (prev.zcoord < current.zcoord) ? prev : current
            });
            this.yMax = this.grdataPoro.reduce(function(prev, current) {
               return (prev.zcoord > current.zcoord) ? prev : current
            });

            var xMin = this.grdataPoro.reduce(function(prev, current) {
               return (prev.dporo < current.dporo) ? prev : current
            });
            var xMax = this.grdataPoro.reduce(function(prev, current) {
               return (prev.dporo > current.dporo) ? prev : current
            });
            var idxMax = 0;
            var sValDimens = arrGraphDimenSs.find(d => d.sgrKey === 'dporo');

            this.ArffPoints4Graph.push({
               idx: 0,
               skey: 'dporo',
               xMin: xMin.dporo,
               xMax: xMax.dporo,
               xDiff: xMax.dporo - xMin.dporo,
               xLeft: 100,
               graphLinePoints: [],
               lineColor: arrColors[idxMax % arrColors.length],
               scaleTopTitle1: {
                  idx: 'tk10',
                  x: 130,
                  y: 55,
                  text: sValDimens.sgrLbl,
                  fontSize: 15
               },
               scaleTopTitle2: {
                  idx: 'tk20',
                  x: 95,
                  y: 85,
                  text: xMin.dporo.toFixed(2) + '  ' + sValDimens.sgrEdinc + '  ' + xMax.dporo.toFixed(2),
                  fontSize: 12
               }
            });

            this.arrYScale.push({
               points: [100 + xAxisWidth + 10, 0, 100 + xAxisWidth + 10, height],
               stroke: 'black',
               strokeWidth: 0.5
            });

            for (var key in Object.keys(this.grdataPoro[0])) {
               var sSkey = Object.keys(vm.grdataPoro[0])[key];
               if (sSkey.startsWith('gisParam')) {

                  var xMin = vm.grdataPoro.reduce(function(prev, current) {
                     return (prev[sSkey] < current[sSkey]) ? prev : current
                  });
                  var xMax = vm.grdataPoro.reduce(function(prev, current) {
                     return (prev[sSkey] > current[sSkey]) ? prev : current
                  });
                  var sValDimens = arrGraphDimenSs.find(d => d.sgrKey === sSkey);
                  idxMax = idxMax + 1;
                  vm.ArffPoints4Graph.push({
                     idx: vm.ArffPoints4Graph.length,
                     skey: sSkey,
                     xMin: xMin[sSkey],
                     xMax: xMax[sSkey],
                     xDiff: xMax[sSkey] - xMin[sSkey],
                     xLeft: 100 + xAxisWidth * (vm.ArffPoints4Graph.length) + 20 * (vm.ArffPoints4Graph.length),
                     graphLinePoints: [],
                     lineColor: arrColors[idxMax % arrColors.length],
                     scaleTopTitle1: {
                        idx: 'tk1' + vm.ArffPoints4Graph.length,
                        x: 100 + xAxisWidth * (vm.ArffPoints4Graph.length) + 25 * (vm.ArffPoints4Graph.length),
                        y: 55,
                        text: sValDimens.sgrLbl,
                        fontSize: 15
                     },
                     scaleTopTitle2: {
                        idx: 'tk2' + vm.ArffPoints4Graph.length,
                        x: 100 + xAxisWidth * (vm.ArffPoints4Graph.length) - 5 + 20 * (vm.ArffPoints4Graph.length),
                        y: 85,
                        text: xMin[sSkey].toFixed(2) + '  ' + sValDimens.sgrEdinc + '  ' + xMax[sSkey].toFixed(2),
                        fontSize: 12
                     }
                  });

                  // правая граница
                  this.arrYScale.push({
                     points: [100 + xAxisWidth * (vm.ArffPoints4Graph.length) - 10 + 20 * (vm.ArffPoints4Graph.length),
                        0, 100 + xAxisWidth * (vm.ArffPoints4Graph.length) - 10 + 20 * (vm.ArffPoints4Graph.length), height
                     ],
                     stroke: 'black',
                     strokeWidth: 0.5
                  });
               }
            }

            this.grdataPart1 = [];
            this.grdataPart1.push(vm.grdataPoro[0]);

            for (let n = 1; n < this.grdataPoro.length; n++) {
               vm.grdataPart1.push(vm.grdataPoro[n]);
            };

            this.arrYText = [];
            // Построение графиков
            var xMin = this.grdataPoro.reduce(function(prev, current) {
               return (prev.dporo < current.dporo) ? prev : current
            });

            var xMax = this.grdataPoro.reduce(function(prev, current) {
               return (prev.dporo > current.dporo) ? prev : current
            });
            var xValDiff = xMax.dporo - xMin.dporo;
            this.vdiff = this.yMax.zcoord - this.yMin.zcoord;
            var delXP = (height - TopPadding) / yAxisStep;

            //заполнение шкалы глубин
            for (let n = 0; n <= yAxisStep; n++) {
               if ((n < yAxisStep) && (n > 4) && n % 5 === 0) {
                  //if (n % 5 === 0) {
                  this.arrYScale.push({
                     points: [55, TopPadding + n * delXP, 70, TopPadding + n * delXP],
                     stroke: 'black',
                     strokeWidth: 0.7
                  });
                  var xtxt = this.yMax.zcoord - this.vdiff / yAxisStep * n;

                  this.arrYText.push({
                     id: 'sctxt' + n,
                     x: 10,
                     y: TopPadding + n * delXP - 5,
                     text: xtxt.toFixed(2),
                     fontSize: 10
                  });
               } else {
                  this.arrYScale.push({
                     points: [65, TopPadding + n * delXP, 70, TopPadding + n * delXP],
                     stroke: 'black',
                     strokeWidth: 0.7
                  });
               }
            }
            this.getGraphWidth = 100 + xAxisWidth * (vm.ArffPoints4Graph.length) + 10 * (vm.ArffPoints4Graph.length);

            this.arrYScale.push({
               points: [70, TopPadding, 70, height],
               stroke: 'black',
               strokeWidth: 0.7
            });
            this.arrYScale.push({
               points: [90, 0, 90, height],
               stroke: 'black',
               strokeWidth: 0.7
            });

            // заполнение значений
            this.FaciesRects = [];
            this.ArffPoints = [];

            var frPrevYy = 0;
            var frDiffYy = 0;
            var grPointYPrev = 0;
            var frPrevFacies = -1;

            for (var n = 0; n < this.grdataPart1.length; n++) {
               var grPointY = Math.floor(((height - TopPadding) * (Math.abs(vm.grdataPart1[n].zcoord - vm.yMax.zcoord))) / vm.vdiff);

               vm.ArffPoints4Graph.forEach(function(scVal, scKey) {
                  var Xval = (vm.grdataPart1[n][scVal.skey] - scVal.xMin) * xAxisWidth / scVal.xDiff;
                  scVal.graphLinePoints[scVal.graphLinePoints.length] = Math.floor(Xval);
                  scVal.graphLinePoints[scVal.graphLinePoints.length] = grPointY;
               });

               //заполнение шкалы фаций
               if (n === 0) {
                  var Lff = ((height - TopPadding) * (Math.abs(vm.grdataPart1[1].zcoord - vm.yMax.zcoord))) / vm.vdiff;
                  //frDiffYy = Math.floor(grPointY);
                  frPrevYy = TopPadding + Math.floor(Lff / 2);

                  vm.FaciesRects.push({
                     rnum: vm.FaciesRects.length,
                     x: 70,
                     width: 20,
                     faciesVal: vm.grdataPart1[0].facies,
                     fill: vm.grdataPart1[0].facies === 0 ? '#868e96' : '#ffc107',
                     y: TopPadding,
                     height: Math.floor(Lff / 2)
                  });
                  grPointYPrev = Lff;
               } else {
                  if (grPointY > grPointYPrev) {
                     frDiffYy = grPointY - grPointYPrev;
                  } else {
                     frDiffYy = grPointY;
                  }
                  vm.FaciesRects.push({
                     rnum: vm.FaciesRects.length,
                     x: 70,
                     width: 20,
                     faciesVal: vm.grdataPart1[n].facies,
                     fill: vm.grdataPart1[n].facies === 0 ? '#868e96' : '#ffc107',
                     y: frPrevYy,
                     height: Math.floor(frDiffYy)
                  });
                  //frPrevYy = TopPadding + Math.floor(frDiffYy / 2);
                  frPrevYy = frPrevYy + Math.floor(frDiffYy);
                  grPointYPrev = Math.floor(grPointY);
               }
            }
         },
        render () {
            vm = this;
             this.chartData.forEach(function(valueY, keyY) {
                  vm.FaciesRects.push({
                     x: valueY.ckx,
                    y: valueY.cky,
                     width: 70,
                     height: 50,
                     fill: valueY.ckD === 0 ? '#868e96' : '#ffc107'
                  });
               });

        },
        add () {
            this.chartData[0].ckx+=20 ;
        },
        getNearetsItems: function(list, needle) {
            var closest = list.reduce(function(prev, curr) {
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
         delLevelRecord(item) {
            this.$q.dialog({
               title: 'Подтверждение',
               message: 'Удалить уровень ' + item.lname + ' ' + item.yLevelMetr + ' ?',
               ok: 'Удалить',
               cancel: 'Отменить'
            }).then(() => {
               var remIndex = this.indexWhere(this.arrTappedLevelsList, arritem => arritem.num === item.num);
               this.arrTappedLevelsList.splice(remIndex, 1);
            }).catch(() => {});
         },
         async choose(okFn) {
            if (this.CurLevelName.length === 0) {
               this.$q.dialog({
                  title: 'Предупреждение',
                  message: `Необходимо указать название уровня`
               }).catch(() => {});
            } else {
               await okFn()
               //проверка существует название
               var obj1 = vm.arrTappedLevelsList.find(d => d.lname === this.CurLevelName);
               if (obj1 !== null) {
                  this.$q.dialog({
                     title: 'Предупреждение',
                     message: 'Уровень с названием ' + this.CurLevelName + ' уже существует. '
                  }).catch(() => {});
                  return;
               }

               if (this.sModalTitle === 'Добавление уровня') {
                  var iColIdx = this.idxMax % arrColors.length;
                  var arrSelX = [];
                  //Получение значения Х
                  var ddF = this.getNearetsItems(this.grdataPart1, this.dCurTapLevel * -1);

                  var ddX = (Math.abs(ddF.nearRItem.dporo) - Math.abs(ddF.nearLItem.dporo)) *
                     (Math.abs(ddF.nearRItem.zcoord) - Math.abs(this.dCurTapLevel)) / (Math.abs(ddF.nearRItem.zcoord) - Math.abs(ddF.nearLItem.zcoord));
                  var ddfx = (ddX - Math.abs(ddF.nearRItem.dporo)) * -1;
                  arrSelX.push({
                     skey: 'dporo',
                     xVal: ddfx.toFixed(2)
                  });
                  for (var key in Object.keys(ddF.nearRItem)) {
                     var sSkey = Object.keys(ddF.nearRItem)[key];
                     if (sSkey.startsWith('gisParam')) {
                        var ddX = (Math.abs(ddF.nearRItem[sSkey]) - Math.abs(ddF.nearLItem[sSkey])) *
                           (Math.abs(ddF.nearRItem.zcoord) - Math.abs(this.dCurTapLevel)) / (Math.abs(ddF.nearRItem.zcoord) - Math.abs(ddF.nearLItem.zcoord));
                        var ddfx = (ddX - Math.abs(ddF.nearRItem[sSkey])) * -1;
                        arrSelX.push({
                           skey: sSkey,
                           xVal: ddfx.toFixed(2)
                        });
                     }
                  }

                  var dNObj = {
                     num: this.idxMax,
                     lname: this.CurLevelName,
                     yPos: this.dCurYPos,
                     yLevelMetr: this.dCurTapLevel,
                     xVal: ddfx.toFixed(2),
                     xValArr: arrSelX,
                     DisplayConfig: {
                        id: this.idxMax,
                        x: 58,
                        y: this.dCurYPos,
                        width: vm.getGraphWidth + 20,
                        height: 1,
                        fill: arrColors[iColIdx]
                     },
                     DisplayLabel: {
                        id: this.idxMax,
                        x: vm.getGraphWidth + 45,
                        y: this.dCurYPos - 10,
                        text: this.CurLevelName ,
                        fontSize: 10,
                        fill: arrColors[iColIdx]
                     }
                  };
                  vm.arrTappedLevelsList.push(dNObj);
                  this.$q.notify({
                     type: 'positive',
                     message: `Добавлен уровень ${this.CurLevelName}`
                  });
               } else {
                  //переименование
                  var obj1 = vm.arrTappedLevelsList.find(d => d.lname === this.editLevelItem.lname);
                  obj1.lname = this.CurLevelName;
                  obj1.DisplayLabel.text = this.CurLevelName + ' (' + obj1.yLevelMetr + ')';
               }
            }
         },
         handleStageMouseDown(event) {
            if (this.bModeOtbiv && this.getTopPadding < event.currentTarget.pointerPos.y) {
               this.dCurYPos = event.currentTarget.pointerPos.y;
               this.dCurTapLevel = (Math.abs(this.yMax.zcoord) + (this.vdiff * (this.dCurYPos - TopPadding)) / (height - TopPadding)).toFixed(2);

               this.idxMax = 1;
               if (vm.arrTappedLevelsList.length > 0) {
                  var itmeMax = vm.arrTappedLevelsList.reduce(function(prev, current) {
                     return (prev.num > current.num) ? prev : current
                  });
                  this.idxMax = itmeMax.num + 1;
               }
               this.CurLevelName = 'Top-' + this.idxMax;
               this.sModalTitle = 'Добавление уровня';
               this.levelDialogModel = true;
            }
         }
    }
}
</script>
