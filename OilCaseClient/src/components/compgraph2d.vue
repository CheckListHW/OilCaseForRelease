<template lang="html">
  <div>
    <ve-line :data="chartDataDin" :settings="chartSettings2" :legend-visible="false"></ve-line>
  </div>
</template>

<script>
import PortalApi from "src/api/OilcaseApi.js";
import VeLine from 'v-charts/lib/line.common'
import SecureLS from "secure-ls";
import OilcaseApi from "src/api/OilcaseApi.js";

const width = 750; //window.innerWidth * 0.75; // 900;
const height = 750; // window.innerHeight * 0.75;
let vm = {};

export default {
  props: ['chartInp', 'chartSettings', 'seismic'],
  name: 'compgraph2d',
  data() {
    return {
      loadProc: false,
      chartSettings2: {
        min: [-2890, -2890],
        max: [-2740, -2740],
        lineTension: 0.5
      },
      chartDataDin: {
        columns: ['istep', 'num1', 'num2'],
        rows: [{
          'num2': -2749.120,
          'num1': -2748.320,
          'istep': '1'
        },
          {
            'num2': -2749.520,
            'num1': -2798.620,
            'istep': '2'
          },
          {
            'num2': -2745.120,
            'num1': -2881.620,
            'istep': '6'
          }
        ]
      }
    }
  },
  mounted() {
    this.chartDataDin = {
      columns: [],
      rows: []
    }
    console.log("sad")
    OilcaseApi.GetLithologicalModelInfo().then(resp => {
      this.chartSettings2 = {
        min: [resp.minZ],
        max: [resp.maxZ],
        lineTension: 1
      }
    })

    this.chartDataDin.columns.push('x')
    for (var surfaceName in this.seismic.values) {
      this.chartDataDin.columns.push(surfaceName)
    }


    for (var i = 0; i < this.seismic.position.length; i++) {
      let value = {}
      value['x'] = i
      for (var surfaceName in this.seismic.values) {
        value[surfaceName] = -this.seismic.values[surfaceName][i]
      }
      this.chartDataDin.rows.push(value)

    }
    console.log(this.chartDataDin)
    // this.loadProc = true;
    // if (this.chartInp !== null && this.chartInp.profType !== 'undefined')
    //    this.clickGetSurf(this.chartInp.iCell, this.chartInp.jCell, this.chartInp.profType, this.chartInp.seismType);
    //
    // this.loadProc = false;
  },
  methods: {
    async clickGetSurf(iCell, jCell, cType, seismType) {
      try {
        vm = this;
        let secLS = new SecureLS();
        let megaObj = secLS.get('ocData');
        var username = megaObj.userid || -1;
        if (cType === 'vert') {
          this.strTempFromApi = await PortalApi.GetSeismic2D("i", iCell, seismType, username);
        } else {
          this.strTempFromApi = await PortalApi.GetSeismic2D("j", jCell, seismType, username);
        }

        if (this.strTempFromApi === "not yet saved") {
          return;
        }
        var max = this.strTempFromApi.reduce(function (prev, current) {
          return (prev.seisVal > current.seisVal) ? prev : current
        });
        this.chartSettings2.max[0] = max.seisVal;
        this.chartSettings2.max[1] = max.seisVal;

        var min = this.strTempFromApi.reduce(function (prev, current) {
          return (prev.seisVal < current.seisVal) ? prev : current
        });
        this.chartSettings2.min[0] = min.seisVal;
        this.chartSettings2.min[1] = min.seisVal;

        var tmpGrData = this.groupBy2(this.strTempFromApi, pet => pet.iSurf);

        var tmpAllDatas = [];
        var tmpAllNames = [];
        this.chartDataDin.columns = ['istep'];
        this.chartDataDin.rows = [];

        var Idx = 0;
        for (let [k, v] of tmpGrData) {
          tmpAllNames.push("layer" + (Idx + 1));
          this.chartDataDin.columns.push("layer" + (Idx + 1));
          tmpAllDatas.push(v);
          Idx++;
        }
        tmpGrData = null;
        for (var i = 0; i < tmpAllDatas[0].length; i++) {
          var OneObj = {};
          OneObj["istep"] = (i);
          for (var j = 0; j < tmpAllNames.length; j++) {
            OneObj["layer" + (j + 1)] = tmpAllDatas[j][i].seisVal;
          }
          this.chartDataDin.rows.push(OneObj);
        }
      } catch (error) {
        console.log(error);
      }
    },
    groupBy2: function (list, keyGetter) {
      const map = new Map();
      list.forEach((item) => {
        const key = keyGetter(item);
        if (!map.has(key)) {
          map.set(key, [item]);
        } else {
          map.get(key).push(item);
        }
      });
      return map;
    },
  },

  components: {
    VeLine
  }
}
</script>
