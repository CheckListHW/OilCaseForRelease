<template>
  <q-page padding class="row justify">
    <div class="doc-container vertical-top" style="margin-top:-20px !important;width:95%">
      <q-inner-loading :visible="loadProc">
        <q-spinner-gears size="150px" color="primary"></q-spinner-gears>
      </q-inner-loading>
      <div class="q-my-md q-title">Сведения об участнике
        <span v-if="megaObj.status"> удалены ранее</span>
      </div>

      <div v-show="!loadProc" class="row gutter-sm">
        <div class="col-5">
          <q-input v-model="megaObj.username" stack-label="Логин" />
          <q-input v-model="megaObj.firstName" stack-label="Фамилия" />
          <q-input v-model="megaObj.lastName" stack-label="Имя" />
          <q-input v-model="megaObj.sDopInfo" stack-label="Пароль" />
          <q-btn class="q-mt-md float-left" outline color="primary" @click="doSaveData(-1)">Удалить
          </q-btn>
        </div>
        <div class="col-5  .offset-md-1">
          <q-input v-model="megaObj.orgname" borderless stack-label="Команда" />
          <q-input v-model="megaObj.rolename" borderless stack-label="Роль" />
          <q-input v-model="megaObj.modelMode" borderless type="number" stack-label="Тип модели" />
          <q-input v-model="megaObj.email" borderless stack-label="email" />
          <q-btn class="q-mt-md float-right" color="primary" @click="doSaveData(1)"> Сохранить изменения</q-btn>
        </div>
      </div>
      <div v-if="megaObj.iCurGameStep !== null && megaObj.iCurGameStep > -1 && !loadProc">
        <q-chip class="q-mt-xl" color="blue-4" square>Игровые данные</q-chip>
        <div class="q-my-md text-h5"><span>Начало <b>{{ getFormattedDate(megaObj.firstdate) }}</b> </span>
          <span> Последних ход <b>{{ getFormattedDate(megaObj.laststdate) }}</b> </span>

          <span> Кол-во ходов <b>{{ megaObj.iCurGameStep }}</b></span>
        </div>
        <div v-if="true">
          Общие затраты: финансовые
          <b class="text-primary"> {{ sumZatrFin | formatFinance }} </b> из {{mnyAllMax}} OilCoin
          <div v-if="false" class="q-mx-md" style="width:60px; background: #ff5566">
            check color
          </div>
          <span class="q-ml-md"> временные</span> <b class="text-primary"> {{ sumZatrSut | formatSut }}</b>из 1095 сут.
          (3 года)
        </div>
      </div>

      <div class="row q-col-gutter-x-md">
        <div class="col-12">
          <q-chip class="q-mt-md" v-show="if1MapLength" square color="blue-grey">Карты</q-chip>
          <q-list v-show="if1MapLength" highlight inset-separator v-for="item in this.arrMapList" :key="`${item.sKey}`">
            <div class="row">
              <div class="col-9">
                <span class="q-mx-md">{{ item.sText }}</span>
              </div>
              <div class="col-3">
                <span class="q-mx-md"> {{ item.sZatMoney | formatFinance }}</span>
                <span>{{ item.sZatSut | formatSut }}</span>
              </div>
            </div>
          </q-list>

          <q-chip class="q-mt-md" square color="deep-orange-5" v-show="if1DlistLength">Скважины</q-chip>
          <q-list v-show="if1DlistLength" highlight inset-separator v-for="item in this.arrDrillsList"
            :key="`${item.id}`">
            <q-item>
              <q-item-main>
                <div class="row">
                  <div class="col-5">
                    <b>ход {{ item.onGameStep }} </b>
                    {{ item.name }} ячейка i {{ item.iCell }} j
                    {{ item.jCell }} Глубина бурения {{ item.drilldeep }}

                    <p class="q-mt-md q-ml-md">
                      Финансовые затраты
                      <q-chip square small color="primary">{{
                          item.onGameStep > 0
                            ? getWellMoneyAll(item.drilldeep) +
                            sumZatrIssl(item)
                            : 0 | formatFinance
                      }}</q-chip>

                      <br />
                      <span v-show="item.onGameStep > 0">
                        базовая:
                        <b>{{ mnyWellBase | formatFinance }}</b> + бурения:
                        <b>{{
                            getWellMoneyDrill(-1 * item.drilldeep) | formatFinance
                        }}</b>
                        +
                      </span>
                      исследования:
                      <b>{{
                          (item.selWellIssl.length * mnyWellIssl +
                            item.selWellIssl2.length * mnyWellIssl * 2 +
                            item.selWellIssl3.length * 100)
                          | formatFinance
                      }}</b>

                      <br />Временные затраты
                      <q-chip square small color="secondary">{{
                          item.onGameStep > 0
                            ? -1 * sutWellMetr * item.drilldeep + sutWellBase
                            : (0 + item.oilprops.length * sutWellIssl) | formatSut
                      }}</q-chip>

                      <br />
                      <span v-show="item.onGameStep > 0">
                        базовые:
                        <b>{{ sutWellBase }}</b>суток + дополнительные:
                        <b>{{
                            (-1 * item.drilldeep * sutWellMetr) | formatSut
                        }}</b>
                        +
                      </span>
                      исследования:
                      <b>{{
                          (item.oilprops.length * sutWellIssl) | formatSut
                      }}</b>
                    </p>
                  </div>
                  <div class="col-5"> <b v-show="item.oilprops.length > 0">Исследования</b>
                    <br />
                    <span class="q-mt-lg" v-for="itemissl in item.oilprops" :key="`${itemissl.isslNum}`">
                      {{ itemissl.isslText }}
                      <br />
                      <!-- {{item.oilprops}} -->
                      <!-- в интервале от {{ itemissl.isslInterval.min }} до {{ itemissl.isslInterval.max }}-->
                    </span>
                  </div>
                </div>

              </q-item-main>
            </q-item>
          </q-list>

          <q-chip square color="green-4  q-mt-md" v-show="if2DlistLength">2D профили</q-chip>
          <q-list v-show="if2DlistLength" highlight inset-separator v-for="item in this.arr2DProfileList"
            :key="`${item.id}`">
            <q-item>
              <q-item-main>
                <b>ход {{ item.onGameStep }} </b>
                {{ item.name }} профиль i {{ item.iCell }} j
                {{ item.jCell }} сейсмические исследования:
                <b> {{ getSeismTypeText(item.seismType) }} </b>
                <br />Финансовые затраты
                <q-chip square small color="primary">{{
                    getSeismicTypeMoney(item.seismType)
                }}</q-chip>
                Временные затраты
                <q-chip small square color="secondary">{{
                    sut2D | formatSut
                }}</q-chip>
              </q-item-main>
            </q-item>
          </q-list>

          <q-chip square color="deep-purple-3 q-mt-md" v-show="if3DlistLength">3D сейсмика</q-chip>
          <q-list v-show="if3DlistLength" highlight inset-separator v-for="item in this.arr3DProfileList"
            :key="`${item.id}`">
            <q-item>
              <q-item-main> <b>ход {{ item.onGameStep }} </b>
                {{ item.name }} куб il {{ item.iCelll }} ir {{ item.iCellr }} jt
                {{ item.jCellt }} jb {{ item.jCellb }} сейсмические исследования:
                <b> {{ getSeismTypeText(item.seismType) }} </b>
                <br />Финансовые затраты
                <q-chip square small color="primary">{{
                    getSeismTypeMoney(item.seismType)
                }}</q-chip>
                Временные затраты
                <q-chip square small color="secondary">{{
                    sut3D | formatSut
                }}</q-chip>
              </q-item-main>
            </q-item>
          </q-list>

          <q-chip square color="brown q-mt-md" v-show="if1SurfObjLength">Обустройство</q-chip>
          <q-list v-show="if1SurfObjLength" highlight inset-separator v-for="item in this.arrSurfObjList"
            :key="`${item.id}`">
            <q-item>
              <q-item-main> <b>ход {{ item.onGameStep }} </b>
                {{ item.name }} - <b>{{ item.description }}</b> участок I
                {{ item.iCell }} J {{ item.jCell }} ячейка i
                {{ item.selCelliPo }} j {{ item.selCelljPo }}
                <q-btn color="red-4" outline icon="delete" @click.prevent="delRecord(item)" class="float-right"
                  v-show="item.onGameStep === iCurGameStep"></q-btn>
                <br />Финансовые затраты
                <q-chip square small color="primary">{{
                    item.objModel.sZatMoney | formatFinance
                }}</q-chip>
                Временные затраты
                <q-chip square small color="secondary">{{
                    item.objModel.sZatSut | formatSut
                }}</q-chip>
              </q-item-main>
            </q-item>
          </q-list>

          <q-chip square color="cyan-6 q-mt-md" v-show="if1LogsLength">Логи решения кейса</q-chip>
          <q-timeline v-show="if1LogsLength" responsive color="secondary" style="padding: 0 24px;">
            <q-timeline-entry v-for="item in strTempFromApi" :key="`${item.id}`"
              v-bind:title="item.actionDATE | dateDateTime" v-bind:subtitle="item.actionType"
              v-bind:side="getSide(item.id)">
              <p v-html="item.actionData"></p>
            </q-timeline-entry>
          </q-timeline>

        </div>
      </div>
    </div>
    <!-- {{megaObj}} -->
    <!-- {{strTempFromApi}} -->
  </q-page>
</template>

<script>
//https://plot.ly/javascript/contour-plots/#styling-color-bar-ticks-for-contour-plots
//https://konvajs.github.io/docs/vue/
import PortalApi from "src/api/OilcaseApi.js";
import EventBus from 'src/event-bus';
import { OutSideMixin } from "src/scriptslibs/scenescripts.js";
import SecureLS from "secure-ls";
import spravDictions from 'assets/SpravDictions'
import moment from 'moment'
//import VueKonva from 'vue-konva'
const width = 750; //window.innerWidth * 0.75; // 900;
const height = 750; // window.innerHeight * 0.75;
let vm = {};
export default {
  name: 'editUserPage',
  mixins: [OutSideMixin],
  data() {
    return {
      loadProc: false,
      sEmplID: -1,
      megaObj: {},
      dictMapsTypes: [],
      if1DlistLength: true,
      if2DlistLength: true,
      if3DlistLength: true,
      if1SurfObjLength: true,
      if1MapLength: true,
      if1LogsLength: true,
      strTempFromApi: []
    };
  },
  created() {
    vm = this;
    EventBus.$once("CaseUserLogLoaded", resp => {
      vm.strTempFromApi = resp;
    });
    EventBus.$on("UserDataSaved", resp => {
      if (resp === "good") {
        if (vm.megaObj.status === -1) {
          vm.loadProc = true;
          vm.if1DlistLength = false;
          vm.if2DlistLength = false;
          vm.if3DlistLength = false;
          vm.if1SurfObjLength = false;
          vm.if1MapLength = false;
          vm.if1LogsLength = false;
          vm.$q.notify({ type: 'warning', message: `Сведения об участнике удалены`, timeout: 1000 });
          return false;
        }
        vm.$q.notify({ type: 'positive', message: `Сведения успешно сохранены`, timeout: 1000 });
      } else {
        vm.$q.notify({ type: 'warning', message: `Ошибка сохранения сведений`, timeout: 1000 });
      }

    });
    EventBus.$once("UserDataLoaded", resp => {
      if (resp !== null) {
        vm.megaObj = resp;
        //this.megaObj.status
        vm.loadProc = true;
        if (vm.megaObj.status === -1) {
          vm.if1DlistLength = false;
          vm.if2DlistLength = false;
          vm.if3DlistLength = false;
          vm.if1SurfObjLength = false;
          vm.if1MapLength = false;
          vm.if1LogsLength = false;
          vm.$q.notify({ type: 'warning', message: `Сведения об участнике удалены`, timeout: 1000 });
          return false;
        }

        if (vm.megaObj.arrWELLLIST !== null && vm.megaObj.arrWELLLIST.length > 0) {
          vm.arrDrillsList = JSON.parse(vm.megaObj.arrWELLLIST);
        }
        if (vm.megaObj.arr2DPROFLIST !== null && vm.megaObj.arr2DPROFLIST.length > 0) {
          vm.arr2DProfileList = JSON.parse(vm.megaObj.arr2DPROFLIST);
        }
        if (vm.megaObj.arr3DCUBELIST !== null && vm.megaObj.arr3DCUBELIST.length > 0) {
          vm.arr3DProfileList = JSON.parse(vm.megaObj.arr3DCUBELIST);
        }
        if (vm.megaObj.arrSURFOBJLIST !== null && vm.megaObj.arrSURFOBJLIST.length > 0) {
          vm.arrSurfObjList = JSON.parse(vm.megaObj.arrSURFOBJLIST);
        }
        if (vm.megaObj.arrCommon !== null && typeof vm.megaObj.arrCommon === 'string' && vm.megaObj.arrCommon.startsWith('{')) {
          vm.arrCommon = JSON.parse(vm.megaObj.arrCommon);
          const maparr = vm.arrCommon.buymap.split(',')
          if (maparr.length > 0) {
            const df = this.dictMapsTypes.filter(x => maparr.includes(x.sKey))
            df.forEach(item => {
              this.arrMapList.push(item)
            })
          }

          vm.$q.notify({ type: 'positive', message: `Сведения загружены`, timeout: 1000 });
          vm.loadProc = false;
        } else {
          vm.$q.notify({ type: 'warning', message: `Ошибка загрузки сведений`, timeout: 1000 });
        }

        vm.if1DlistLength = true;
        vm.if2DlistLength = true;
        vm.if3DlistLength = true;
        vm.if1SurfObjLength = true;
        vm.if1LogsLength = true;
        vm.loadProc = false;
      }
    });
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      const secLS = new SecureLS();
      var megaObj = secLS.get("ocData");
      if (megaObj.userrole !== "admin") {
        vm.$router.push("/LoginPage");
      }
    })
  },
  beforeDestroy() {
    EventBus.$off("UserDataLoaded");
    EventBus.$off("UserDataSaved");
    EventBus.$off("CaseUserLogLoaded");
  },
  methods: {

    getSide: function (param1) {
      if (param1 % 2 === 0) {
        return 'left'
      } else {
        return 'right'
      }
    },
    async doSaveData(status) {
      this.$q.dialog({
        title: 'Подтверждение',
        message: 'Сохранить изменения?',
        ok: 'Сохранить',
        cancel: 'Нет'
      })
        .then(() => {
          var postdata = {
            id: this.megaObj.id, FirstName: this.megaObj.firstName, LastName: this.megaObj.lastName, Username: this.megaObj.username,
            Orgname: this.megaObj.orgname, Rolename: this.megaObj.rolename, Tasksame: this.megaObj.tasksame, email: this.megaObj.email, Password: this.megaObj.password,
            Status: this.megaObj.status, sDopInfo: this.megaObj.sDopInfo, modelMode: this.megaObj.modelMode
          };

          if (this.sEmplID === 'new') {
            postdata.id = -1
          }
          if (status === -1) {
            postdata.status = -1;
          }

          PortalApi.DoSaveUserData(postdata);
        })
        .catch(() => {
          this.$q.notify('отменено')
        })
      //window.alert(prop.id+" "+prop.name+" "+prop.value);
      //  PortalApi.SetDictValue(prop);
      //this.strMessage = prop.id+" "+prop.name+" "+prop.value;
    },

  },
  computed: {
  },
  async mounted() {
    vm = this;
    this.sEmplID = this.$route.params.emplid;
    vm.loadProc = true;
    if (this.sEmplID !== 'new') {
      this.dictMapsTypes = spravDictions.dictMapsTypes;
      let bProf = await PortalApi.DoLoadUserInfo1(this.sEmplID);
      //this.strTempFromApi = await PortalApi.GetCaseLogEvents(this.sEmplID);
      let dd = await PortalApi.GetCaseLogEvents(4647);
    } else {
      vm.if1DlistLength = false;
      vm.if2DlistLength = false;
      vm.if3DlistLength = false;
      vm.if1SurfObjLength = false;
      vm.if1MapLength = false;
      vm.if1LogsLength = false;
      vm.loadProc = false;
      this.megaObj.firstName = ''; this.megaObj.lastName = ''; this.megaObj.username = '';
      this.megaObj.orgname = ''; this.megaObj.rolename = ''; this.megaObj.tasksame = ''; this.megaObj.email = ''; this.megaObj.password = '';
      this.megaObj.status = 1; this.megaObj.sDopInfo = ''; this.megaObj.modelMode = 5;
    }
  },
  filters: {
    formatFinance: function (str) {
      if (!str) {
        return '0 OilCoin'
      }
      return `${str.toLocaleString('ru-RU')} OilCoin`
    },
    formatSut: function (str) {
      if (!str) {
        return '0 сут.'
      }
      return `${str.toLocaleString('ru-RU')} сут.`
    },
    dateDateTime: function (str) {
      if (!str) {
        return "(n/a)";
      }
      str = new Date(str);
      //return ((str.getDate() < 10) ? '0' : '') + str.getDate() + '-' + ((str.getMonth() < 9) ? '0' : '') + (str.getMonth() + 1) + '-' + str.getFullYear();
      return moment(String(str)).format("HH:mm:ss") + ' ' + moment(String(str)).format("DD.MM.YYYY");
    }
  }
}
</script>

<style lang="stylus">
</style>
