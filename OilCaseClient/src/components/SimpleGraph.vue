<template lang="html">
  <div><p class="bg-positive">chartData: {{ chartData }}
    <br/>chartSettings: {{ chartSettings }}</p>
    <button @click="add()">Increment</button>
    <div style="border: 1px solid #96979e; width:400px">
      <v-stage ref="stage" :config="configKonva">
        <v-layer>
          <v-rect v-for="item in FaciesRects" :key="item.rnum" :config="item"/>
        </v-layer>
      </v-stage>
    </div>
  </div>
</template>
<script>

const width = 400; //window.innerWidth * 0.75; // 900;
const height = 750; // window.innerHeight * 0.75;
let vm = {};
export default {
  props: ["chartData", "chartSettings"],
  name: "SimpleGraph",
  data() {
    return {
      msgData: "msgData",
      count: 0,
      FaciesRects: [],
      configKonva: {
        width: width,
        height: height
      }
    };
  },
  mounted() {
    this.render();
  },
  watch: {
    chartData() {
      this.render();
    }
  },
  methods: {
    render() {
      vm = this;
      this.chartData.forEach(function (valueY) {
        vm.FaciesRects.push({
          x: valueY.ckx,
          y: valueY.cky,
          width: 70,
          height: 50,
          fill: valueY.ckD === 0 ? "#868e96" : "#ffc107"
        });
      });
    },
    add() {
      this.FaciesRects[0].x += 20;
    }
  }
};
</script>
