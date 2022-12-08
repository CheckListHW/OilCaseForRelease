<template>
   <q-page padding class="row justify">
      <div class="doc-container vertical-top" style="margin-top:-20px !important;width:95%">
         <h5>Логи решения кейса</h5>
         <div class="row q-pt-md">
            <div class="col-2">
               <q-select v-model="selOneUser" filter float-label="Выбор пользователя" :options="getUsersOptions" />
               <q-btn class="q-ma-lg" color="secondary" size="sm" label="Посмотреть историю"
                  @click="doCaseLogAction()" />
               <q-btn class="q-mr-lg q-my-md" v-show="strTempFromApi.length > 0" v-if="(strTempFromApi !== null)"
                  @click.prevent="csvExport()">export</q-btn>
               <q-btn class="q-mr-lg q-my-md" v-if="false" @click.prevent="TestZtr()">TestZtr</q-btn>
               <div v-if="loadProc2">
                  Общие затраты: <br />финансовые
                  <q-chip square color="primary">{{ sumZatrFin | formatFinance }}</q-chip>

                  <br />временные
                  <q-chip square color="secondary">{{ sumZatrSut | formatSut }}</q-chip>
               </div>

            </div>
            <div class="q-pl-lg col-10">
               <q-timeline responsive color="secondary" style="padding: 0 24px;">
                  <q-timeline-entry v-for="item in strTempFromApi" :key="`${item.id}`"
                     v-bind:title="item.actionDATE | dateDateTime" v-bind:subtitle="item.actionType"
                     v-bind:side="getSide(item.id)">
                     <p v-html="item.actionData"></p>
                  </q-timeline-entry>
               </q-timeline>
            </div>
         </div>

         <q-inner-loading :visible="loadProc">
            <q-spinner-gears size="150px" color="primary"></q-spinner-gears>
         </q-inner-loading>
      </div>
   </q-page>
</template>

<script>
import PortalApi from "src/api/OilCaseApi.js";
import EventBus from 'src/event-bus';
import moment from 'moment'
import SecureLS from "secure-ls";

const width = 750;
const height = 750;

let vm = {};
export default {
   name: 'showUserLog',
   data() {
      return {
         loadProc: false,
         loadProc2: false,
         strTempFromApi: [],
         strTempFromApi2: [],
         selOneUser: {},
         arrUsersList: [],
         arrDrillsList: [],
         arr2DProfileList: [],
         arr3DProfileList: [],
         mnyWellBase: 56020,
         mnyWellMetr: 23,
         mnyWellIssl: 500,
         mnyWellKernFoto: 200,
         sutWellBase: 30.0,
         sutWellMetr: 0.013,
         sutWellIssl: 2.0,
         mny2D: 5000,
         mny3D: 75000,
         sut2D: 90,
         sut3D: 300,
      };
   },
   created() {
      vm = this;
      EventBus.$on('CaseUserLogLoaded', eventMess => {
         // this.doAfterLogin(eventMess);
         this.strTempFromApi = eventMess;
         this.doDrawCaseLogAction();
      });
      EventBus.$on('CaseUserLogLoaded2', resp => {
         if (resp !== "" && resp !== "[]" && resp !== undefined) {
            vm.arrDrillsList = JSON.parse(resp.arrWELLLIST);
            vm.arr2DProfileList = JSON.parse(resp.arr2DPROFLIST);
            vm.arr3DProfileList = JSON.parse(resp.arr3DCUBELIST);
            vm.loadProc2 = true;
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
   methods: {
      async TestZtr() {
         vm = this;
         vm.loadProc2 = false;;
         let df1 = await PortalApi.DoLoadCase4lod(2633);
      },
      csvExport() {
         var universalBOM = "\ufeff";
         //let csvContent = "mime:text/csv;charset: 'UTF-16BE'";

         //let csvContent = "data:text/csv;charset=utf-8,";
         let csvContent = "data:text/csv;charset=utf-16be;base64,";
         //let csvContent = "data:application/vnd.ms-excel,";
         csvContent += [
            Object.keys(this.strTempFromApi[0]).join(";"),
            ... this.strTempFromApi.map(item => Object.values(item).join(";"))
         ]
            .join("\n")
            .replace(/(^\[)|(\]$)/gm, "");

         //const data = encodeURI(universalBOM+csvContent);
         const data = encodeURI(csvContent);
         const link = document.createElement("a");
         link.setAttribute("href", data);
         link.setAttribute("download", "CaseActionsLog.csv");
         link.click();
      },
      getSide: function (param1) {
         if (param1 % 2 === 0) {
            return 'left'
         } else {
            return 'right'
         }
      },
      async LoadData() {
         this.loadProc = true;
         try {
            var ldddata = await PortalApi.DoGetAllUsers();
            if (ldddata !== "[]") {
               this.arrUsersList = ldddata;//.filter(x=>x.id>2245);
            }
         } catch { }
         this.loadProc = false;
      },


      async doCaseLogAction() {
         try {
            vm = this;
            this.loadProc = true;
            this.loadProc2 = false;
            this.strTempFromApi = await PortalApi.GetCaseLogEvents(this.selOneUser);
            let df1 = await PortalApi.DoLoadCase4lod(this.selOneUser);
            //   this.strTempFromApi2 = await PortalApi.DoLoadCase4lod(this.selOneUser);



            this.loadProc = false;
         } catch (error) {
            console.log(error);
         }
      },
      async doDrawCaseLogAction() {
         try {
            vm = this;
            this.loadProc = true;
            // this.strTempFromApi = await PortalApi.GetCaseLogEvents(this.selOneUser);
            this.loadProc = false;
         } catch (error) {
            console.log(error);
         }
      },
      //

   },
   computed: {
      sumZatrFin() {
         vm = this;
         var wellCo = 0;
         if (this.arrDrillsList.length > 0) {
            this.arrDrillsList.forEach(item => {
               if (item.onGameStep > 0) {
                  wellCo =
                     wellCo +
                     -1 * item.drilldeep * vm.mnyWellMetr +
                     vm.mnyWellBase +
                     item.selWellIssl.length * vm.mnyWellIssl +
                     item.selWellIssl2.length * vm.mnyWellIssl * 3 +
                     item.selWellIssl3.length * 200;
                  //   item.oilprops.length * vm.mnyWellIssl;
               } else {

                  wellCo = wellCo + item.oilprops.filter(x => x.isslType !== "gisParam10").length * vm.mnyWellIssl;

                  wellCo = wellCo + item.oilprops.filter(x => x.isslType === "gisParam10").length * vm.mnyWellKernFoto;

               }
            });
         }
         if (this.arr2DProfileList.length > 0) {
            var prof2DCo = 0;
            this.arr2DProfileList.forEach(item => {
               if (Array.isArray(item.seismType)) {
                  item.seismType.forEach(el => {
                     if (el === 'seismR') {
                        prof2DCo += 5000;
                     } else if (el === 'seismM') {
                        prof2DCo += 16000;
                     } else if (el === 'seismT') {
                        prof2DCo += 11000;
                     }
                  });
               } else {
                  prof2DCo += item.seismType === 'seismM' ? 9000 : 5000
               }
               //prof2DCo+=item.seismType=='seismM'?9000:5000
            });
         }


         //  var prof2DCo = this.arr2DProfileList.reduce(
         //  function (acc, curitem) { return acc + curitem.seismType=='seismM'?9000:5000; }, 0   );

         // var prof3DCo = this.arr3DProfileList.reduce(
         //   (acc, curitem) => acc +curitem.seismType=='seismM'?100000:70000,0 );

         var prof3DCo = 0;
         if (this.arr3DProfileList.length > 0) {
            this.arr3DProfileList.forEach(item => {
               if (Array.isArray(item.seismType)) {
                  item.seismType.forEach(el => {
                     if (el === 'seismR') {
                        prof2DCo += 5000;
                     } else if (el === 'seismM') {
                        prof2DCo += 45000;
                     } else if (el === 'seismT') {
                        prof2DCo += 35000;
                     }
                  });
               } else {
                  prof3DCo += item.seismType === 'seismM' ? 35000 : 25000;
               }
               //prof3DCo+=item.seismType=='seismM'?35000:25000;
            });
         }


         return wellCo + prof2DCo + prof3DCo;
      },

      sumZatrSut() {
         vm = this;
         //const sumValues = (obj) => Object.keys(obj).reduce((acc, value) => acc + obj[value], 0);
         //return this.arrDrillsList.length ;
         var sutItog;
         //var wellSut=this.arrDrillsList.reduce((acc, curitem) => acc +  -1*curitem.drilldeep* vm.sutWellMetr+vm.sutWellBase+curitem.oilprops.length* vm.sutWellIssl, 0);
         var wellSut = 0;
         this.arrDrillsList.forEach(item => {
            if (item.onGameStep > 0) {
               wellSut =
                  wellSut +
                  -1 * item.drilldeep * vm.sutWellMetr +
                  vm.sutWellBase +
                  item.oilprops.length * vm.sutWellIssl;
            } else {
               wellSut = wellSut + item.oilprops.length * vm.sutWellIssl;
            }
         });

         var prof2DSut = this.arr2DProfileList.length > 0 ? this.sut2D : 0;
         var prof3DSut = this.arr3DProfileList.length > 0 ? this.sut3D : 0;
         sutItog = prof3DSut > prof2DSut ? prof3DSut : prof2DSut;
         sutItog = wellSut > sutItog ? wellSut : sutItog;
         return sutItog; //this.arrDrillsList.reduce((acc, curitem) => acc +  -1*curitem.drilldeep* vm.sutWellMetr+vm.sutWellBase+curitem.oilprops.length* vm.sutWellIssl, 0);
      },
      getUsersOptions() {
         return this.arrUsersList.map((item) => {
            //"sgrKey": "gisParam1", "sgrLbl": "GR", "sgrText": "Гамма-каротаж",
            return {
               label: item.firstName + ' - ' + item.lastName,
               value: item.id
            };
         });
      }
   },

   mounted() {
      this.LoadData();
   },
   filters: {
      formatFinance: function (str) {
         if (!str) {
            return "0 т.р.";
         }
         // str = new Date(str);
         //return moment(String(str)).format("HH:mm:ss") + ' ' + moment(String(str)).format("DD.MM.YYYY");
         //var sdfd=str.toString().replace( /\B(?=(?:\d{3})+$)/g, ',' )
         //i.toString().replace( /\B(?=(?:\d{3})+$)/g, ',' );

         return `${str.toLocaleString("ru-RU")} т.р.`;
      },
      formatSut: function (str) {
         if (!str) {
            return "0 сут.";
         }
         // str = new Date(str);
         //return moment(String(str)).format("HH:mm:ss") + ' ' + moment(String(str)).format("DD.MM.YYYY");

         return `${str.toLocaleString("ru-RU")} сут.`;
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
