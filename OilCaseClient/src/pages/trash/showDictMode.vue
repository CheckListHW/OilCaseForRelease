<template>
   <q-page padding class="row justify">
      <div class="doc-container vertical-top" style="margin-top:-20px !important;width:95%">
         <h5>Настройка </h5>
          <q-table title="Cправочник значений" :hide-bottom="true" :pagination="pagination" :filter="tblfilter" :visible-columns="tblvisibleColumns" :data="arrDictionaryList" :columns="columns" row-key="name">
      <template slot="top-right" slot-scope="props">
           <q-search
          hide-underline
          color="secondary"
          v-model="tblfilter"
          class="col-6"
        />
        <q-table-columns
          color="secondary"
          class="q-mr-sm"
          v-model="tblvisibleColumns"
          :columns="columns"
        />
      </template>
      <q-tr slot="body" slot-scope="props" :props="props">
        <q-td key="cName" :props="props">
          {{ props.row.name }}
          <q-popup-edit @save="dosave(props.row)" v-model="props.row.name" title="Название параметра" buttons>
              <q-input v-model="props.row.name" />
          </q-popup-edit>
        </q-td>
        <q-td key="cDescription" :props="props">
          {{ props.row.description }}
             <q-popup-edit @save="dosave(props.row)" v-model="props.row.description" title="Описание параметра" buttons>
              <q-input v-model="props.row.description" />
          </q-popup-edit>
        </q-td>
        <q-td  key="cStatus" :props="props">{{ props.row.status }}</q-td>
        <q-td key="cValue" :props="props" >
          {{ props.row.value }}
          <q-popup-edit @save="dosave(props.row)" v-model="props.row.value" title="Значение параметра" buttons>
            <q-input type="number" v-model="props.row.value" />
          </q-popup-edit>
           </q-td>
      </q-tr>
    </q-table>
         <q-inner-loading :visible="loadProc">
            <q-spinner-gears size="150px" color="primary"></q-spinner-gears>
         </q-inner-loading>
      </div>
   </q-page>
</template>

<script>
//https://plot.ly/javascript/contour-plots/#styling-color-bar-ticks-for-contour-plots
//https://konvajs.github.io/docs/vue/
import PortalApi from "src/api/OilCaseApi.js";
import EventBus from 'src/event-bus';
import moment from 'moment'
import SecureLS from "secure-ls";
//import VueKonva from 'vue-konva'
const width = 750; //window.innerWidth * 0.75; // 900;
const height = 750; // window.innerHeight * 0.75;
let vm = {};
export default {
  name: 'showDictMode',
  data() {
    return {
      loadProc: false,
      pagination:{
         rowsPerPage: 25
      },
      strMessage:'',
      tblfilter:'',
      tblvisibleColumns:['cDescription','cValue'],
      arrDictionaryList: [],
       columns: [
        {
          name: 'cName',
          required: true,
          label: 'Название',
          align: 'left',
          field: 'name',
          sortable: true
        },
        { name: 'cDescription', label: 'Описание', field: 'description', align: 'left', sortable: true },
       { name: 'cStatus', label: 'Статус', field: 'status', sortable: true },
        { name: 'cValue', label: 'Значение', field: 'value', align: 'left', sortable: true }
      ],
    };
  },
  created() {
    vm = this;
     EventBus.$once("evbusSetDictValue", resp => {
        if(resp === 'good'){
           this.$q.notify({  type: 'positive', message: `Значение успешно сохранено`, timeout:1000});
            localStorage.setItem("ocDictHash", "new");
        }else{
           this.$q.notify({ type: 'warning', message: `Ошибка сохранения`, timeout:1000});
        }
    });
  },
  beforeRouteEnter(to, from, next) {
      next(vm => {
        const secLS = new SecureLS();
        var megaObj = secLS.get("ocData");
        if(megaObj.userrole !== "admin"){
          vm.$router.push("/LoginPage");
        }
    })
  },
  beforeDestroy(){
    EventBus.$off("evbusSetDictValue")
  },
  methods: {
     dosave(prop){
        //window.alert(prop.id+" "+prop.name+" "+prop.value);
       PortalApi.SetDictValue(prop);
        //this.strMessage = prop.id+" "+prop.name+" "+prop.value;
     }
  },
  computed: {
  },
  async mounted() {
      await PortalApi.GetDictValues().then((response) => {
          if (response.status === 200) {
            this.arrDictionaryList = response.data;
      }})

       //this.arrDictionaryList = await PortalApi.GetDictValues();
  },
  filters: {

  }
}
</script>

<style lang="stylus">
</style>
