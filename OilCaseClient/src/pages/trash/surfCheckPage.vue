<template>
  <q-page padding class="row justify">
    <q-inner-loading :visible="apiloadvisible">
      <q-spinner-gears size="50px" color="primary"></q-spinner-gears>
    </q-inner-loading>
    <div class="doc-container" style="margin-top: -40px !important; width: 95%">
      <h3>surfCheckPage</h3>
      <div class="row">
        <div class="col-2">
          Входной поток
          <q-input v-model="inpFlow.F" stack-label="Расход" />
          <q-input v-model="inpFlow.P" stack-label="Давление" />
          <q-input v-model="inpFlow.T" stack-label="Тепература" />
          <q-input v-model="inpFlow.Pl" stack-label="Плотность" />
          <q-input v-model="inpFlow.H2O" stack-label="Содержание воды" />
          <q-input v-model="inpFlow.DNp" stack-label="Давление насыщенных паров" />
          <q-input v-model="outFlow.GF" stack-label="Газовый фактор" />
        </div>
        <div class="col-8">
          <div>Перечень оборудования</div>
          <div class="q-ma-md" v-for="(item, idx) in this.surfInstalledArr" :key="`${item.uid}`">
            <q-btn label="Добавить перед" outline @click="addApparat(idx)" />
             <div class="row" v-if="item.outFlow!=null">
                <div class="col-11">
                  {{ idx + 1 }} <b>{{ item.apprt.name }}</b>
                </div>
                <div class="col-1">
                  <q-btn  @click.prevent="doViewAppInfo(item.type)" color="blue"  size="sm" outline  icon="info" />

                </div>
            </div>
            <div>
              <div class="row" v-if="item.outFlow!=null">
                <div class="col">
                <p> Fluid</p>
                <div v-if="item.outFlow.Fluid!=null">  {{ item.outFlow.Fluid }}</div>
                </div>
                <div class="col">
                <p> Gas</p>
                 <div v-if="item.outFlow.Gas!=null">  {{ item.outFlow.Gas }}</div>
                </div>
                <div class="col">
                <p> Oil</p>
                 <div v-if="item.outFlow.Oil!=null">  {{ item.outFlow.Oil }}</div>
                </div>
              </div>
              <hr/>
              {{ item }}
            </div>
          </div>
          <q-btn label="Добавить в конец" outline @click="addApparat(-1)" />
          <div class="q-mt-md">
            {{ calcFlows }}
          </div>
            <!-- <div v-for="item in this.calcFlows" :key="`${item.id}`">
              {{item}}
            </div> -->
        </div>
        <div class="col-2">
          Выходной поток
          <q-input v-model="outFlow.F" stack-label="Расход" />
          <q-input v-model="outFlow.P" stack-label="Давление" />
          <q-input v-model="outFlow.T" stack-label="Тепература" />
          <q-input v-model="outFlow.Pl" stack-label="Плотность" />
          <q-input v-model="outFlow.H2O" stack-label="Содержание воды" />
          <q-input
            v-model="outFlow.DNp"
            stack-label="Давление насыщенных паров"
          />
        </div>
      </div>
      <q-modal v-model="bViewAppInfo" :content-css="{margin: '80px'}">
      <div style="padding: 20px" v-if="curSurfObject !== null">
          <div class="row">
            <div class="col-11 q-display-3">
              {{curSurfObject.name}}</div>
            <div class="col-1">
              <q-btn color="primary" class="float-right" outline flat @click="bViewAppInfo = false" icon="close" /></div>
          </div>
           <div class="row">
            <div class="col-7">
              <p style="white-space: pre-line;" v-html="curSurfObject.description.text"></p>
            </div>
             <div class="col-5">
               <p class="float-right"><img :src="`statics/surfObjPic/${curSurfObject.description.image}`"/></p>
             </div>
            </div>

      </div>
    </q-modal>
   curSurfObject:  {{curSurfObject==null}}
   bViewAppInfo {{bViewAppInfo}}
    </div>
  </q-page>
</template>

<style>
</style>

<script>
// import { QSpinnerGrid, QSpinnerGears } from 'quasar';
// import store from 'src/store';
import { OutSideMixin } from 'src/scriptslibs/scenescripts.js';
import surfDictions from 'assets/SurfDictions'
let vm = {};
export default {
  name: 'surfCheckPage',
  mixins: [OutSideMixin],
  data() {
    return {
      apiloadvisible: false,
      selApp: null,
      inpFlow: { F: 126, P: 265, T: 26, Pl: 0.25, H2O: 0.05, DNp: 27, GF: 0.02 },
      outFlow: { F: 0, P: 0, T: 0, H2O: 0 },
      deltaP: 1.5,//Зависит от перепада давления. Сейчас в расчет возьму 1,5 атм
      surfInstalledArr: [],
      bViewAppInfo: false,
      dictSurfObjects: null,
      curSurfObject: null,
      arrParams: [
        { name: 'Расход', sign: 'F' },
        { name: 'Давление', sign: 'P' },
        { name: 'Температура', sign: 'T' },
        { name: 'Плотность', sign: 'Pl' },
        { name: 'Содержание воды', sign: 'H2O' },
        { name: 'Давление насыщенных паров', sign: 'DNp' },
        { name: 'Газовый фактор', sign: 'GF' }],
      varcheck: {},
      arrSurfApparats: [
        {
          name: 'Двухфазный сепаратор',
          type: 'sep2',
          calc: {},
          description:{}
        }, {
          name: 'Трехфазный сепаратор',
          type: 'sep3',
          calc: {},
        },
        {
          name: 'Отстойник',
          type: 'otstoy',
          calc: {},
        },
        {
          name: 'Электродегидратор',
          type: 'eldeg',
          calc: {},
        },
        {
          name: 'Концевые сепарационные установки (КСУ)',
          type: 'ksu',
          calc: {},
        },
        {
          name: 'Резервуар вертикальный стальной (РВС)',
          type: 'rvs',
          calc: {},
        },
      ]
    };
  },
  beforeDestroy () {},
  created () {},
  computed: {
    calcFlows() {
      let resflow = { ...this.inpFlow };
      for (let idx = 0; idx < this.surfInstalledArr.length; idx++) {
        let element = this.surfInstalledArr[idx];
        resflow = this.doCalcStep(resflow, element.type);
        resflow.id = Date.now()
        element.outFlow = { ...resflow };
      }
      this.outFlow = resflow;
      return resflow;
    },
    getSurfItems () {
      return this.arrSurfApparats.map(item => {
        return {
          label: item.name,
          value: item.type
        }
      })
    },
  },
  methods: {
    doViewAppInfo(appId) {
      this.curSurfObject = this.dictSurfObjects[0];
      this.bViewAppInfo = true;
    },
    doCalcStep(inpFlow, apptype) {
      let resFlow ={}
      let resGlow ={}
      let resOillow ={}
      let resAllFlow ={ 'Fluid':resFlow, 'Gas':resGlow, 'Oil':resOillow }
      Object.assign(resFlow, inpFlow)
      if (apptype === 'sep2') {
        resFlow.F = 0.97 * inpFlow.F - 0.001;
        resFlow.P = inpFlow.P - this.deltaP; //Зависит от перепада давления. Сейчас в расчет возьму 1,5 атм
        resFlow.T = inpFlow.T - 3;

        resFlow.Pl = inpFlow.Pl;
        resFlow.H2O = inpFlow.H2O * (inpFlow.F/resFlow.F);
        resFlow.DNp = inpFlow.DNp * 0.048 + 79.86;

        resGlow.F = 0.97 * inpFlow.GF - 0.001;
        resGlow.P = inpFlow.P - this.deltaP;
        resGlow.T = inpFlow.T - 3;
      } else if (apptype === 'sep3') {
        resFlow.F = inpFlow.F * inpFlow.H2O * 0.995;
        resFlow.P = inpFlow.P;
        resFlow.T = inpFlow.T - 5;

        resGlow.F = 0.97 * inpFlow.GF - 0.001;
        resGlow.P = inpFlow.P - this.deltaP;
        resGlow.T = inpFlow.T - 3;

        resOillow.F = inpFlow.F - inpFlow.F * inpFlow.H2O;
        resOillow.P = inpFlow.P;
        resOillow.T = inpFlow.T - 5;

        resOillow.Pl = inpFlow.Pl;
        resOillow.H2O = 0.5;
        resOillow.DNp = inpFlow.DNp;

      } else if (apptype === 'otstoy') {
        resFlow.P = inpFlow.P;
        resFlow.F = inpFlow.F * inpFlow.H2O * 0.995;
        resFlow.T = inpFlow.T - 5;


        resOillow.P = inpFlow.P;
        resOillow.F = inpFlow.F - inpFlow.F * inpFlow.H2O;
        resOillow.T = inpFlow.T - 5;

        resOillow.Pl = inpFlow.Pl;
        resOillow.H2O = 0.5;
        resOillow.DNp = inpFlow.DNp;
      }
      else if (apptype === 'eldeg') {
        resFlow.P = inpFlow.P;
        resFlow.F = inpFlow.F * inpFlow.H2O * 0.995;
        resFlow.T = inpFlow.T - 5;

        resOillow.P = inpFlow.P;
        resOillow.F = inpFlow.F - inpFlow.F * inpFlow.H2O;
        resOillow.T = inpFlow.T - 5;

        resOillow.Pl = inpFlow.Pl;
        resOillow.H2O = 0.5;
        resOillow.DNp = inpFlow.DNp;
      }
      else if (apptype === 'ksu') {
        resFlow.F = 0.97 * inpFlow.F - 0.001;
        resFlow.P = inpFlow.P - this.deltaP;
        resFlow.T = inpFlow.T - 3;

        resFlow.Pl = inpFlow.Pl;
        resFlow.H2O = inpFlow.H2O * (inpFlow.F/resFlow.F);
        resFlow.DNp = 62; //Любое число от 60 до 66 кПа, ибо правильно установленна КСУ решает вопрос по давлению насыщенных паров.

        resGlow.F = 0.97 * inpFlow.GF - 0.001;
        resGlow.P = inpFlow.P - this.deltaP;
        resGlow.T = inpFlow.T - 3;
      }
      else if (apptype === 'rvs') {
        resFlow.P = 1;
        resFlow.F = inpFlow.F * inpFlow.H2O * 0.995;
        resFlow.T = inpFlow.T - 5;

        resOillow.P = inpFlow.P;
        resOillow.F = inpFlow.F - inpFlow.F * inpFlow.H2O;
        resOillow.T = inpFlow.T - 5;

        resOillow.Pl = inpFlow.Pl;
        resOillow.H2O = 0.5;
        resOillow.DNp = inpFlow.DNp;
      }
      return resAllFlow;
    },
    async addApparat(idx) {
      this.$q
        .dialog({
          title: 'Добаление аппарата',
          message: 'Укажите тип аппарата',
          ok: 'Добавить',
          cancel: 'Отменить',
          options: {
            type: 'radio',
            model: null,
            items: this.getSurfItems
          },
        })
        .then((data) => {
          let selApp = this.arrSurfApparats.find((x) => x.type === data);

          // if (idx > -1) {
          //   previd = this.surfInstalledArr[idx].uid;
          // }
          if (selApp !== null) {
            let previd =idx > -1? this.surfInstalledArr[idx].uid:-1;
            let newItem = {
              type: data,
              uid: Date.now(),
              uiprev: previd,
              apprt: selApp,
            };
            //this.surfInstalledArr.push({type:data, uid:Date.now(), uiprev:122,apprt:selApp});
            if(idx > -1){
              this.surfInstalledArr = [
                ...this.surfInstalledArr.slice(0, idx), // first half
                newItem, // items to be inserted
                ...this.surfInstalledArr.slice(idx), // second half
              ];
            }else{
              this.surfInstalledArr.push(newItem);
            }
            //переписат parent id
            for (let index = 0; index < this.surfInstalledArr.length; index++) {
              if(index==0){
                this.surfInstalledArr[index].uiprev=-1;
              }else{
                this.surfInstalledArr[index].uiprev = this.surfInstalledArr[index - 1].uid;
                //this.surfInstalledArr[index] = { ...this.surfInstalledArr[index].uiprev=index };
              }

            }
          }
        })
        .catch(() => {});
    },
  },
  mounted () {
    this.dictSurfObjects = surfDictions.dictSurfApparats
  },
  components: {}
};
</script>
