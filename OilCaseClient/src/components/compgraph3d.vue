<template lang="html">
  <div>
    <q-inner-loading :visible="loadProc">
      <q-spinner-gears size="50px" color="primary"></q-spinner-gears>
    </q-inner-loading>
  </div>
</template>

<script>
import EventBus from 'src/event-bus';
import PortalApi from "src/api/OilcaseApi.js";

let vm = {};

export default {
  props: ['chartInp', 'chartSettings'],
  name: 'compgraph3d',
  data() {
    return {
      loadProc: false,
      strTempFromApi: [],
      chartSettings2: {
        min: [-2890, -2890],
        max: [-2740, -2740]
      },
      grSurfdata: [{
        z: [
          []
        ],
        x: [],
        y: [],
        type: 'contour',
        contours: {
          start: -2834.0,
          end: -2836.0,
          size: 0.5
        },

        dx: 1,
        x0: 37,
        dy: 1,
        y0: 23,
        autocontour: false,
      }],
    }
  },
  created() {
    EventBus.$on('doLoaded3DProf', () => {
      this.clickGetSurf(this.chartInp.iCelll,
        this.chartInp.iCellr, this.chartInp.jCellb, this.chartInp.jCellt, this.chartInp.layerName);
    });
  },
  mounted() {
    this.getDBData();
  },
  methods: {

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
    uniqueCols() {
      return this.strTempFromApi.reduce((seed, current) => {
        return Object.assign(seed, {
          [current.dcol]: current
        });
      }, {});
    },
    uniqueRows() {
      return this.strTempFromApi.reduce((seed, current) => {
        return Object.assign(seed, {
          [current.drow]: current
        });
      }, {});
    },
    async getDBData() {
      try {
        this.strTempFromApi = await PortalApi.GetSurfaceLayer21(this.chartInp.iCelll,
          this.chartInp.iCellr, this.chartInp.jCellb, this.chartInp.jCellt, this.chartInp.layerName, this.chartInp.iModelMode);
        EventBus.$emit('doLoaded3DProf', '');
      } catch (error) {
        console.log(error);
      }
    },
    async clickGetSurf(iCelll, iCellr, jCellb, jCellt, layerName) {
      try {
        vm = this;
        if (this.strTempFromApi.length == 0) {
          return;
        }
        //this.loadProc = true;
        //idx

        var max = this.strTempFromApi.reduce(function (prev, current) {
          return (prev.zCoord > current.zCoord) ? prev : current
        });
        this.contoursZmax = max.zCoord;
        this.grSurfdata[0].contours.end = max.zCoord;

        var min = this.strTempFromApi.reduce(function (prev, current) {
          return (prev.zCoord < current.zCoord) ? prev : current
        });
        this.contoursZmin = min.zCoord;
        this.grSurfdata[0].contours.start = min.zCoord;

        // берем ccol crow и
        var sallCols = this.uniqueCols();
        var sallRows = this.uniqueRows();
        this.grSurfdata[0].x = [];
        this.grSurfdata[0].y = [];
        this.grSurfdata[0].z = [];
        for (var index in sallCols) {
          var attr = sallCols[index];
          this.grSurfdata[0].x.push(parseInt(index, 10));
        }
        for (var index in sallRows) {
          var attr = sallRows[index];
          this.grSurfdata[0].y.push(parseInt(index, 10));
        }

        this.grSurfdata[0].y.forEach(function (valueY, keyY) {
          var tmpXarr = [];
          vm.grSurfdata[0].x.forEach(function (valueX, keyX) {
            var obj1 = vm.strTempFromApi.find(d => d.dcol === valueX && d.drow === valueY);
            tmpXarr.push(obj1.zCoord);
          });
          vm.grSurfdata[0].z.push(tmpXarr);
        });


        var dgrPoss = document.getElementById('grPoss' + this.chartInp.idx);
        while (dgrPoss.firstChild) dgrPoss.removeChild(dgrPoss.firstChild);

        var div = document.createElement('div')
        var graphId = 'graph-' + this.chartInp.idx;
        div.id = graphId;
        div.style = "border: 1px solid #96979e;width: 1000px; height:800px;"
        dgrPoss.appendChild(div)

        Plotly.newPlot(graphId, this.grSurfdata);


      } catch (error) {
        console.log(error);
      }
    },
  },

  components: {}
}
</script>
