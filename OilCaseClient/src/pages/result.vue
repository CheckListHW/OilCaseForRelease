<template>
  <q-page padding class="row justify">
    <q-inner-loading :visible="apiloadvisible">
      <q-spinner-gears size="50px" color="primary"></q-spinner-gears>
    </q-inner-loading>
    <div class="doc-container" style="margin-top:-20px !important;width:95%">
      <q-tabs animated inverted swipeable align="justify">
        <q-tab name="tabWell" slot="title" label="Скважины"/>
        <q-tab default name="tab2d" slot="title" label="2D профили"/>
        <q-tab-pane name="tabWell">

          <!--          <q-list v-show="false" highlight inset-separator v-for="(item) in this.arrDrillsList"-->
          <!--                  :key="`${item.x}-${item.y}`">-->
          <!--            <q-item>-->
          <!--              <q-item-main>-->
          <!--                <div class="row">-->
          <!--                  <div class="col-8">-->
          <!--                    <p class="q-mt-lg" v-for="itemissl in item.oilprops" :key="`${itemissl.isslNum}`"-->
          <!--                       v-show="(itemissl.onGameStep !== null && itemissl.onGameStep <= iCurGameStep) || itemissl.onGameStep === null">-->
          <!--                      <b>{{ itemissl.isslText }}</b>-->

          <!--                      в интервале от {{ itemissl.isslInterval.min }} до {{ itemissl.isslInterval.max }}-->
          <!--                      <a class="q-pl-lg" v-show="itemissl.isslType === 'gisParam10'" :href="sLinktoMetod + 'GetOneWellDataZip21/' +-->
          <!--                      item.name + '/' + item.iCell + '/' + item.jCell + '/' + (item.drilldeep) + '/' + iUserID">-->
          <!--                        <q-btn class="q-ma-sm" color="positive" label=" получить фото керна"/>-->
          <!--                      </a>-->
          <!--                    </p>-->
          <!--                    <q-modal v-model="item.bEditModel" :content-css="{ minWidth: '50vw', minHeight: '99vh' }">-->
          <!--                      <q-modal-layout>-->
          <!--                        <q-toolbar slot="header">-->
          <!--                          <q-toolbar-title>-->
          <!--                            <div>Отбивка уровней для скважины {{ item.name }}</div>-->
          <!--                          </q-toolbar-title>-->
          <!--                        </q-toolbar>-->
          <!--                        <div style="margin:20px">-->
          <!--                          <showGis :chartInp="{ curUserID: iUserID, wellname: item.name, wellid: item.id, iCell: item.iCell, jCell: item.jCell, drillMin: 1, drillMax: item.drilldeep, wellIssl: item.oilprops, curGameStep: iCurGameStep, ModelMode: iModelMode }"-->
          <!--                            :chartSettings="comp2dchrtSettings"/>-->
          <!--                        </div>-->
          <!--                        <q-btn class="q-mr-lg q-mb-md float-right" color="teal" @click.prevent="doSaveWellParams(item)">-->
          <!--                          Сохранить-->
          <!--                        </q-btn>-->
          <!--                        <q-btn class="q-ml-lg q-mb-md" color="orange" @click.prevent="item.bEditModel = false">Отмена-->
          <!--                        </q-btn>-->
          <!--                      </q-modal-layout>-->
          <!--                    </q-modal>-->
          <!--                  </div>-->

          <!--                  <div-->
          <!--                    v-show="item.oilprops.length > 0 && ((item.oilprops[0].onGameStep !== null && item.oilprops[0].onGameStep <= iCurGameStep) || item.oilprops[0].onGameStep === null)"-->
          <!--                    class="col-4 vertical-top">-->
          <!--                    кол-во уровней-->
          <!--                    <q-chip square v-bind:color="getTappedLevelCount(item) > 0 ? 'green-4' : 'red-4'">-->
          <!--                      {{ getTappedLevelCount(item) }}-->
          <!--                    </q-chip>-->
          <!--                    <q-btn class="float-right q-mr-md" color="indigo" outline icon="waves"-->
          <!--                           @click.prevent="doOpenWellParams(item)">-->
          <!--                      <q-tooltip>Отбивка уровней</q-tooltip>-->
          <!--                    </q-btn>-->
          <!--                    <br/>-->
          <!--                    <br/>-->
          <!--                    <div v-for="itemLevels in item.tappedLevels" :key="itemLevels.idx">-->
          <!--                      <span class="q-title">-->
          <!--                        <b>{{ itemLevels.lname }}</b>-->
          <!--                        {{ itemLevels.yLevelMetr }}-->
          <!--                      </span>-->
          <!--                      <br/>-->
          <!--                      <span class="q-ml-md" v-for="itemXarr in itemLevels.xValArr" :key="itemXarr.skey">-->
          <!--                        {{itemXarr.skey}} {{ itemXarr.xVal }}</span>-->
          <!--                    </div>-->
          <!--                  </div>-->
          <!--                </div>-->
          <!--              </q-item-main>-->
          <!--            </q-item>-->
          <!--          </q-list>-->

          <q-list v-if="boreholes" highlight inset-separator v-for="(item) in boreholes" :key="`${item.x}-${item.y}`">
            <q-item>
              <q-item-main>
                <div>
                  <div class="col-8 ">
                    {{ item.name.toUpperCase() }} ячейка X:<b>{{ item.x }}</b> - Y:<b>{{ item.y }}</b> Глубина бурения
                    {{ item.zMin }}
                    <q-btn class="q-ma-sm" color="primary" label="выгрузить каротажи" @click="getWellCarotags(item)"/>
                    кол-во уровней
                    <q-chip square v-bind:color="getTappedLevelCount(item) > 0 ? 'green-4' : 'red-4'">
                      {{ getTappedLevelCount(item) }}
                    </q-chip>
                    <q-btn class="float-right q-mr-md" color="indigo" outline icon="waves"
                           @click.prevent="doOpenWellParams(item)">
                      <q-tooltip>Отбивка уровней</q-tooltip>
                    </q-btn>
                  </div>
                </div>
                <div>
                  <div class="col-8">
                    <p class="q-mt-lg" v-for="itemResearch in item.gis" :key="`${itemResearch.name}`">
                      <b>{{ itemResearch.name.toUpperCase() }} - {{ itemResearch.description }} </b>
                      <q-btn v-show="itemResearch.name === 'facies'" class="q-ma-sm" color="positive"
                             label=" получить фото керна"/>
                    </p>
                  </div>

                  <q-modal v-model="item.bEditModel" :content-css="{ minWidth: '50vw', minHeight: '99vh' }">
                    <q-modal-layout>
                      <q-toolbar slot="header">
                        <q-toolbar-title>
                          <div>Отбивка уровней для скважины {{ item.name }}</div>
                        </q-toolbar-title>
                      </q-toolbar>
                      <div style="margin:20px">
                        <showGis :chartInp="{ wellname: item.name, wellid: item.id,
                        iCell: item.iCell, jCell: item.jCell,
                         drillMin: 1, drillMax: item.drilldeep, wellIssl: item.oilprops,
                         curGameStep: iCurGameStep, ModelMode: iModelMode }"
                                 :chartSettings="comp2dchrtSettings"
                                 :boreholeName="item.name"/>
                      </div>
                      <q-btn class="q-mr-lg q-mb-md float-right" color="teal" @click.prevent="doSaveWellParams(item)">
                        Сохранить
                      </q-btn>
                      <q-btn class="q-ml-lg q-mb-md" color="orange" @click.prevent="item.bEditModel = false">Отмена
                      </q-btn>
                    </q-modal-layout>
                  </q-modal>

                </div>
              </q-item-main>
            </q-item>
          </q-list>

          <!--          <q-modal v-model="bLoadLAS" :content-css="{ minWidth: '600px' }">-->
          <!--            <q-modal-layout>-->
          <!--              <q-toolbar slot="header">-->
          <!--                <q-toolbar-title>-->
          <!--                  <div>Загузка LAS файла</div>-->
          <!--                </q-toolbar-title>-->
          <!--              </q-toolbar>-->
          <!--              <div class="layout-padding" style="padding:2em!important">-->
          <!--                <h6 style="margin-top:10px;margin-bottom:15px;">Загузка LAS файла</h6>-->
          <!--                <p>Пожалуйста, укажите запасы нефти всего лицензионного участка в пластовых условиях,</p>-->
          <!--                <input type="file" id="fileLAS" ref="fileLAS" @change="lasFileLoaded">-->
          <!--              </div>-->

          <!--              <q-toolbar slot="footer" class="justify-between">-->
          <!--                <q-btn class="q-mr-lg q-mb-lg q-my-sm" color="orange" @click.prevent="bLoadLAS = false">Отмена</q-btn>-->
          <!--                <q-btn class="q-ml-lg q-my-sm " v-if="lasFile !== null" color="positive" @click="doLoadWellCarotag()">-->
          <!--                  Загрузить-->
          <!--                </q-btn>-->
          <!--              </q-toolbar>-->

          <!--            </q-modal-layout>-->
          <!--          </q-modal>-->

        </q-tab-pane>
        <q-tab-pane name="tab2d">
          <q-list v-if="seismic" highlight inset-separator v-for="(item) in seismic"
                  :key="`${item.startX}-${item.startY}-${item.endX}-${item.endY}`">
            <q-item>
              <q-item-main>
                <div class="q-title">Сейсмика с координатами:
                  x:<b>{{ item.startX }}</b>-<b>{{ item.endX }}</b>, y:<b>{{ item.startY }}</b>-<b>{{ item.endY }}</b>
                </div>
                <!--                <div class="q-title">id: {{ item.id }} name: {{ item.name }}-->
                <!--                  profType: {{ item.profType }} i {{ item.iCell }} j {{ item.jCell }}-->
                <!--                </div>-->
                <!--                <q-chip v-show="!Array.isArray(item.seismType)" square color="red">Неверный профиль!</q-chip>-->
                <!--                <div v-if="Array.isArray(item.seismType)">-->
                <div>
                  <div class="q-ml-lg  q-mt-md" v-for="(sit) in item.seismType" :key="sit">
                    <div class="row">
                      <div class="col-10">
                        <b> {{ sit }}</b>
                        <b> {{ getSeismTypeText(sit) }}</b>
                      </div>
                      <!--                      <div class="col-2">-->
                      <!--                        <a v-if="sit === 'seismR'"-->
                      <!--                           v-show="(item.onGameStep !== null && parseInt(item.onGameStep) < iCurGameStep)"-->
                      <!--                           :href="sLinktoMetod2 + 'Get2dTimeImages/' + item.name + '/' + item.profType + '/' + item.iCell + '/' + item.jCell">-->
                      <!--                          <q-btn class="q-ma-sm" size="sm" color="positive" label="получить временной разрез"/>-->
                      <!--                        </a>-->
                      <!--                        <a v-if="sit !== 'seismR'"-->
                      <!--                           v-show="(item.onGameStep !== null && parseInt(item.onGameStep) < iCurGameStep)"-->
                      <!--                           :href="sLinktoMetod + 'GetOne2dProfDataZip21/' + item.name + '/' + item.profType + '/' + item.iCell + '/' + item.jCell + '/' + sit + '/' + iModelMode">-->
                      <!--                          <q-btn class="q-ma-sm" size="sm" color="positive" label="получить данные интерпретации"/>-->
                      <!--                        </a>-->
                      <!--                      </div>-->
                    </div>
                    <compgraph2d v-if=" sit === 'seismR'"
                                 :seismic="item"
                                 :chartInp="{ iCell: item.iCell, jCell: item.jCell, profType: item.profType, seismType: sit }"
                                 :chartSettings="comp2dchrtSettings"/>
                  </div>
                </div>
              </q-item-main>
            </q-item>
          </q-list>
        </q-tab-pane>
      </q-tabs>
    </div>
  </q-page>
</template>

<style>
</style>

<script>
import {QSpinnerGrid, QSpinnerGears} from "quasar";

import EventBus from "src/event-bus";
import store from "src/store";
import OilcaseApi from "src/api/OilCaseApi";
import {ObjectCreator} from "src/data/ObjectTemplates/ObjectCreator";
import showGis from "src/components/showGis";
import Properties from "src/api/Properties";

import compgraph2d from "src/components/compgraph2d.vue";
import compgraph3d from "src/components/compgraph3d.vue";
import {OutSideMixin} from "src/scriptslibs/scenescripts.js";

let vm = {};
export default {
  name: "result",

  mixins: [OutSideMixin],

  data() {
    return {
      drillDeep: Properties.MinDrillDeep,
      boreholes: null,
      seismic: [],
      GamesStep: 0,

      iCurGameStep: 1000,
      iUserID: "",
      secLS: null,
      iModelMode: 1,
      bEditModel: false,
      apiloadvisible: false,
      arrDrillsList: [],
      arr2DProfileList: [],
      arr3DProfileList: [],
      sLinktoMetod: "",
      sLinktoMetod2: "",
      bLoadLAS: false,
      objCurWell: {},
      lasFile: {},
      megaObj: {},
      comp2dchrtSettings: {
        gsdhjdf: "s34df",
        fgsddf: "s34df",
        dfsddf: 45
      },
      comp2dData: [
        {
          profType: "vert",
          iCell: 8,
          jCell: 16
        }
      ]
    };
  },

  beforeDestroy() {
    EventBus.$off("CaseDataLoaded");
    EventBus.$off("doLoadedWellsData");
  },

  mounted() {
    // this.doLoadCaseData();
    // this.LoadData2Storage();
  },

  created() {
    OilcaseApi.GetBoreholeLog().then(resp => {
      console.log('OilcaseApi.GetBoreholeLog()')
      console.log(resp)
      this.boreholes = resp
    })

    OilcaseApi.GetInfo().then(resp => {
      this.GamesStep = resp.gameStep
    })

    // EventBus.$on("CaseDataLoaded", todoItem => {
    //   this.doLoadCaseData();
    // });
    // EventBus.$on("doLoadedWellsData", todoItem => {
    //   this.doLoadedWellsData(todoItem);
    // });

    OilcaseApi.GetDataSeismic().then(resp => {
      this.seismic = []
      resp.forEach(item => {
        item.seismType = ['seismR']
        this.seismic.push(item)
      })
    })
  },

  computed: {
    StoreWellTappedLevels() {
      return store.state.curWellTappedLevels;
    },
    if3DlistLength() {
      return this.arr3DProfileList.length > 0;
    },
    if2DlistLength() {
      return this.arr2DProfileList.length > 0;
    },
    if1DlistLength() {
      return this.arrDrillsList.length > 0;
    }
  },

  methods: {
    addBorehole(CellX, CellY, name, modelWell, gameStep, toeI, toeJ, toeK) {
      let boreholeId = this.getMaxIdx(this.arrDrillsList)
      this.arrDrillsList.push(ObjectCreator.Borehole(
        boreholeId,
        this.getXForDraw(CellX), this.getYForDraw(CellY),
        CellX, CellY,
        name, gameStep,
        this.drillDeep, modelWell, toeI, toeJ, toeK
      ))
    },

    async lasFileLoaded() {
      if (this.$refs.fileLAS.files.length > 0) {
        this.lasFile = this.$refs.fileLAS.files[0];
        if (this.lasFile.size > (1 * 1024 * 1024)) {
          this.$q.notify({
            color: "negative",
            icon: "warning",
            message:
              "Внимание! Размер файла не должен превышать 1 МБ",
            position: "top",
            timeout: 5000
          });
        }

      }
    },

    async doLoadWellCarotag() {
      let formData = new FormData();
      formData.append('file', this.lasFile, this.lasFile.name);
      formData.append('userid', 235);
      await PortalApi.loadWellLogging(formData).then((resp) => {
        return resp;
      })

    },

    async doOpenWellCarotag(item) {
      this.bLoadLAS = true;
      this.lasFile = null;
      this.objCurWell = item;

    },

    SpinnerShow(options) {
      this.$q.loading.show(options);
    },

    getSWRusNanmes(engName) {
      if (engName === "dry") {
        return "сухо";
      } else if (engName === "water") {
        return "вода";
      } else if (engName === "oil") {
        return "нефть";
      } else if (engName === "wateroil") {
        return "вода+нефть";
      } else if (engName === "oilwater") {
        return "нефть+вода";
      } else if (engName === "notclear") {
        return "неясно";
      }
    },

    async getWellCarotags(item) {
      var filename = "well_WellLogging_" + item.name + ".txt";
      var obj =
        "#\tLAS\tformat\tlog\tfile\tfrom\tPETREL\r\n" +
        "#\tProject\tunits\tare\tspecified\tas\tdepth\tunits\r\n" +
        "#==================================================================\r\n" +
        "~Version\tinformation\r\n" +
        "VERS.\t2.0:\r\n" +
        "WRAP.\tNO:\r\n" +
        "#==================================================================\r\n";

      obj +=
        "~Well\r\n" +
        "STRT\t.m\t2600.0000000\r\n" +
        "STOP\t.m\t" +
        item.drilldeep +
        "\r\n" +
        "STEP\t.m\t0.20000000\r\n" +
        "NULL\t.\t-99990000\r\n" +
        "COMP.\t:\tCOMPANY\r\n";

      obj +=
        "WELL.\tNew\twell\t:\tWELL\r\n" +
        "FLD.\t:\tFIELD\r\n" +
        "LOC.\t:\tLOCATION\r\n" +
        "SRVC.\t:\tSERVICE\tCOMPANY\r\n";
      "DATE.\t31.07.2019\t14:56:54\t:\tLog\tExport\tDate\t{yyyy-MM-dd	HH:mm:ss}\r\n" +
      "PROV.\t:\tPROVINCE\r\n";
      ("UWI.\t:\tUNIQUE\tWELL\tID\r\n");
      ("API.\t:\tAPI\tNUMBER\r\n");
      ("#==================================================================\r\n");

      obj +=
        "~Curve\r\n" +
        "DEPT\t.m\t:\tDEPTH\t1\r\n" +
        "Lith\t._\t:\tLith\t2\r\n" +
        "GR\t.gAPI\t:\tGR\t3\r\n" +
        "PS\t.mV\t:\tSP\t4\r\n" +
        "NPHI\t.mSm/m\t:\tIK\t5\r\n" +
        "RDEP\t.us/m\t:\tAK\t6\r\n" +
        "GGKP\t.g/cm3\t:\tGGKP\t7\r\n" +
        "PORO_CORE\t    \t:\tgrPor\t8\r\n" +
        "SW_CORE\t    \t:\tgrPN\t9\r\n" +
        "PERM_core\tmD\t:\tgrPerm\t10\r\n" +
        "PORO_RIGIS\tmD\t:\tgrPerm\t11\r\n" +
        "PERM_RIGIS\tmD\t:\tgrPerm\t12\r\n" +
        "LITH_RIGIS\tmD\t:\tgrPerm\t13\r\n" +
        "SW_RIGIS\tmD\t:\tgrPerm\t14\r\n" +
        "OPROBOVANIE\t \t:\toprob\t14\r\n" +
        "~Parameter\r\n" +
        "#==================================================================\r\n" +
        "~Ascii\r\n";
      obj += "\r\n\r\n\r\n\r\n";
      //await PortalApi.GetWellDataDeep(item.iCell, item.jCell, item.drilldeep)
      await PortalApi.GetWellDataDeep21(
        item.iCell,
        item.jCell,
        -2940,
        this.iModelMode
      ).then(response => {
        var strTempFromApi = response;
        strTempFromApi.forEach(oneite => {
          //1 DEPT
          obj += "\r\n\t" + oneite.zCoord;

          //2 Lith
          if (
            item.oilprops.filter(oilIte => oilIte.isslType === "gisParam7")
              .length > 0
          ) {
            obj += "\t" + oneite.facies;
          } else {
            obj += "\t--9999";
          }

          //3 GR
          if (
            item.oilprops.filter(oilIte => oilIte.isslType === "gisParam2")
              .length > 0
          ) {
            obj += "\t" + oneite.gisParam2;
          } else {
            obj += "\t--9999";
          }

          //4 PS
          if (
            item.oilprops.filter(oilIte => oilIte.isslType === "gisParam1")
              .length > 0
          ) {
            obj += "\t" + oneite.gisParam1;
          } else {
            obj += "\t--9999";
          }

          //5 NPHI
          if (
            item.oilprops.filter(oilIte => oilIte.isslType === "gisParam4")
              .length > 0
          ) {
            obj += "\t" + oneite.gisParam4;
          } else {
            obj += "\t--9999";
          }

          //6 RDEP
          if (
            item.oilprops.filter(oilIte => oilIte.isslType === "gisParam5")
              .length > 0
          ) {
            obj += "\t" + oneite.gisParam5;
          } else {
            obj += "\t--9999";
          }

          //7 GGKP
          if (
            item.oilprops.filter(oilIte => oilIte.isslType === "gisParam6")
              .length > 0
          ) {
            obj += "\t" + oneite.gisParam6;
          } else {
            obj += "\t--9999";
          }

          //8 PORO_CORE
          if (
            item.oilprops.filter(oilIte => oilIte.isslType === "gisParam8")
              .length > 0
          ) {
            obj += "\t" + oneite.gisParam8;
          } else {
            obj += "\t--9999";
          }

          //9 SW_CORE
          if (
            item.oilprops.filter(oilIte => oilIte.isslType === "gisParam9")
              .length > 0
          ) {
            obj += "\t" + oneite.gisParam9;
          } else {
            obj += "\t--9999";
          }

          //10 PERM_core
          if (
            item.oilprops.filter(oilIte => oilIte.isslType === "gisParam20")
              .length > 0
          ) {
            obj += "\t" + oneite.gisParam20;
          } else {
            obj += "\t--9999";
          }

          //11
          if (
            item.oilprops.filter(oilIte => oilIte.isslType === "rigisParam1")
              .length > 0
          ) {
            obj += "\t" + oneite.rigisParam1;
          } else {
            obj += "\t--9999";
          }

          //12
          if (
            item.oilprops.filter(oilIte => oilIte.isslType === "rigisParam2")
              .length > 0
          ) {
            obj += "\t" + oneite.rigisParam2;
          } else {
            obj += "\t--9999";
          }

          //13
          if (
            item.oilprops.filter(oilIte => oilIte.isslType === "rigisParam3")
              .length > 0
          ) {
            obj += "\t" + oneite.rigisParam3;
          } else {
            obj += "\t--9999";
          }

          //14
          if (
            item.oilprops.filter(oilIte => oilIte.isslType === "rigisParam4")
              .length > 0
          ) {
            obj += "\t" + oneite.rigisParam4;
          } else {
            obj += "\t--9999";
          }

          //15
          if (
            item.oilprops.filter(oilIte => oilIte.isslType === "gisParam21")
              .length > 0
          ) {
            obj += "\t" + oneite.gisParam21;
          } else {
            obj += "\t--9999";
          }
        });

        var blob = new Blob([obj], {type: "text/plain"});
        if (window.navigator && window.navigator.msSaveOrOpenBlob) {
          window.navigator.msSaveOrOpenBlob(blob, filename);
        } else {
          var e = document.createEvent("MouseEvents"),
            a = document.createElement("a");
          a.download = filename;
          a.href = window.URL.createObjectURL(blob);
          a.dataset.downloadurl = ["text/plain", a.download, a.href].join(":");
          e.initEvent(
            "click",
            true,
            false,
            window,
            0,
            0,
            0,
            0,
            0,
            false,
            false,
            false,
            false,
            0,
            null
          );
          a.dispatchEvent(e);
        }
      });
    },

    async getWellCords(arWellsList) {
      var filename = "wells_heads_all.txt";
      var obj =
        "#\tPetrel\twell\thead\r\n" +
        "#\tUnit\tin\tX\tand\tY\tdirection:\tm\r\n" +
        "#\tUnit\tin\tdepth:\tm\r\n" +
        "VERSION\t1\r\n" +
        "BEGIN\tHEADER\r\n";
      obj +=
        "Name\t\t\t\r\n" +
        "UWI\r\n" +
        "Well\tsymbol\t\t\t3\r\n" +
        "Surface\tX\t\t\t\r\n" +
        "Surface\tY\t\t\t\r\n";
      obj += "Bottom\thole\tX\t\t\r\n" + "Bottom\thole\tY\t\t\r\n";
      obj += "END\tHEADER\r\n";

      for (var OneWell of arWellsList) {
        //mvv for actual coords
        //if(OneWell.xCoord==null || OneWell.yCoord==null){
        var xCoord = 0;
        var yCoord = 0;
        await PortalApi.GetWellKoord(
          OneWell.iCell,
          OneWell.jCell,
          this.iModelMode
        ).then(response => {
          if (response !== null && response.length === 2) {
            OneWell.xCoord = response[0];
            OneWell.yCoord = response[1];
          }
        });
        //}
        obj +=
          // OneWell.name + "\t" + OneWell.xCoord + "\t" + OneWell.yCoord + "\r\n";
          OneWell.name + "\t" + OneWell.iCell + "\t" + OneWell.jCell + "\r\n";
      }
      var blob = new Blob([obj], {type: "text/plain"});
      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(blob, filename);
      } else {
        var e = document.createEvent("MouseEvents"),
          a = document.createElement("a");
        a.download = filename;
        a.href = window.URL.createObjectURL(blob);
        a.dataset.downloadurl = ["text/plain", a.download, a.href].join(":");
        e.initEvent(
          "click",
          true,
          false,
          window,
          0,
          0,
          0,
          0,
          0,
          false,
          false,
          false,
          false,
          0,
          null
        );
        a.dispatchEvent(e);
      }
    },

    async clickExtract3D() {
      this.SpinnerShow({
        spinner: QSpinnerGrid,
        spinnerColor: "blue-9",
        spinnerSize: 200,
        message: "Выполняется обработка данных...",
        messageColor: "warning"
      });

      if (this.arr3DProfileList.length > 0) {
        var filename = "Profiles3D.txt";
        var obj = "Profiles3D\r\n";
        for (const element of this.arr3DProfileList) {
          obj +=
            "id: " +
            element.id +
            " name: " +
            element.name +
            " profType: " +
            element.profType +
            " i from  " +
            element.iCelll +
            " to " +
            element.iCellr +
            " j from " +
            element.jCellb +
            " to " +
            element.jCellt +
            "\r\n";
          var strTempFromApi = [];
          strTempFromApi = await PortalApi.GetAllSurfacesRange(
            element.iCelll,
            element.iCellr,
            element.jCellb,
            element.jCellt
          );

          strTempFromApi.forEach(poelem => {
            //obj += "sLayerName: " + poelem.sLayerName + "x: " + poelem.x_coord +"   y_coord: " + poelem.y_coord + "   zCoord: " + poelem.zCoord + "\r\n";
            obj +=
              poelem.x_coord +
              "\t" +
              poelem.y_coord +
              "\t" +
              poelem.zCoord +
              "\r\n";
          });
        }
        var blob = new Blob([obj], {type: "text/plain"});
        if (window.navigator && window.navigator.msSaveOrOpenBlob) {
          window.navigator.msSaveOrOpenBlob(blob, filename);
        } else {
          var e = document.createEvent("MouseEvents"),
            a = document.createElement("a");
          a.download = filename;
          a.href = window.URL.createObjectURL(blob);
          a.dataset.downloadurl = ["text/plain", a.download, a.href].join(":");
          e.initEvent(
            "click",
            true,
            false,
            window,
            0,
            0,
            0,
            0,
            0,
            false,
            false,
            false,
            false,
            0,
            null
          );
          a.dispatchEvent(e);
        }
      }
      this.$q.loading.hide();
    },

    async clickGetZip() {
      await PortalApi.GetWellDataZip(3)
        .then(response => {
          var blob = new Blob([response], {type: "application/octet-stream"});
          if (window.navigator && window.navigator.msSaveOrOpenBlob) {
            window.navigator.msSaveOrOpenBlob(blob, filename);
          } else {
            var e = document.createEvent("MouseEvents"),
              a = document.createElement("a");
            a.download = "wellsData.zip";
            a.href = window.URL.createObjectURL(blob);
            a.dataset.downloadurl = [
              "application/octet-stream",
              a.download,
              a.href
            ].join(":");
            e.initEvent(
              "click",
              true,
              false,
              window,
              0,
              0,
              0,
              0,
              0,
              false,
              false,
              false,
              false,
              0,
              null
            );
            a.dispatchEvent(e);
          }
        })
        .catch(error => {
          console.error(error);
        });

      var sdf = 234;
    },

    async clickExtract2D() {
      this.SpinnerShow({
        spinner: QSpinnerGrid,
        spinnerColor: "blue-9",
        spinnerSize: 200,
        message: "Выполняется обработка данных...",
        messageColor: "warning"
      });

      if (this.arr2DProfileList.length > 0) {
        var filename = "Profiles2D.txt";
        var obj = "Profiles2D\r\n";
        for (const element of this.arr2DProfileList) {
          obj +=
            "id: " +
            element.id +
            " name: " +
            element.name +
            " profType: " +
            element.profType +
            " i " +
            element.iCell +
            " j " +
            element.jCell +
            "\r\n";
          var strTempFromApi = [];
          if (element.profType === "vert") {
            strTempFromApi = await PortalApi.Get2DLayer("i", element.iCell);
          } else {
            strTempFromApi = await PortalApi.Get2DLayer("j", element.jCell);
          }


          strTempFromApi.forEach(poelem => {
            //obj += "sLayerName: " + poelem.sLayerName + "x: " + poelem.x_coord +"   y_coord: " + poelem.y_coord + "   zCoord: " + poelem.zCoord + "\r\n";
            obj +=
              poelem.x_coord +
              "\t" +
              poelem.y_coord +
              "\t" +
              poelem.zCoord +
              "\r\n";
          });
        }
        var blob = new Blob([obj], {type: "text/plain"});
        if (window.navigator && window.navigator.msSaveOrOpenBlob) {
          window.navigator.msSaveOrOpenBlob(blob, filename);
        } else {
          var e = document.createEvent("MouseEvents"),
            a = document.createElement("a");
          a.download = filename;
          a.href = window.URL.createObjectURL(blob);
          a.dataset.downloadurl = ["text/plain", a.download, a.href].join(":");
          e.initEvent(
            "click",
            true,
            false,
            window,
            0,
            0,
            0,
            0,
            0,
            false,
            false,
            false,
            false,
            0,
            null
          );
          a.dispatchEvent(e);
        }
      }

      this.$q.loading.hide();
    },

    getTappedLevelCount(item) {
      return 0
    },

    doSaveWellParams(thiItem) {
      thiItem.tappedLevels = store.state.curWellTappedLevels;
      thiItem.bEditModel = false;

      var ld1 = localStorage.getItem("storDrills") || "[]";
      var ld2 = JSON.stringify(this.arrDrillsList);
      if (ld2 !== ld1) {
        localStorage.setItem("storDrills", JSON.stringify(this.arrDrillsList));
        this.SaveStorage2DBData(-1);
      }
    },

    doLoadedWellsData(todoItem) {
      todoItem.bEditModel = true;
      this.apiloadvisible = false;
    },

    doOpenWellParams(item) {
      this.apiloadvisible = true;
      item.bEditModel = true;
      console.log(item)
      EventBus.$emit("doOpenWellLevels", item)
    },

    async get2DTimesImages(idx, stype) {
      var sdd = null;
      await PortalApi.Get2dTimeImages(idx, stype).then(response => {
        sdd = "data:image/png;charset=utf-8;base64," + response;
        return sdd;
      });
    },

    async LoadData2Storage() {
      this.apiloadvisible = true;
      if (this.secLS.ls['ocData'] === null) {
        this.apiloadvisible = false;
        return;
      }
      this.megaObj = this.secLS.get('ocData');
      this.iCurGameStep = this.megaObj.iCurGameStep;
      try {
        if (this.megaObj.arrDrillsList !== null) {
          this.arrDrillsList = JSON.parse(this.megaObj.arrDrillsList);
        }
        if (this.megaObj.arr2DProfileList !== null) {
          this.arr2DProfileList = JSON.parse(this.megaObj.arr2DProfileList);
        }
        if (this.megaObj.arr3DProfileList !== null) {
          this.arr3DProfileList = JSON.parse(this.megaObj.arr3DProfileList);
        }
      } catch {
      }
      this.apiloadvisible = false;
    }
  },

  components: {
    compgraph2d,
    compgraph3d,
    showGis
  }
};
</script>
