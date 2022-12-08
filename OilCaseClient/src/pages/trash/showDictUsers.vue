<template>
   <q-page padding class="row justify">
      <div class="doc-container vertical-top" style="margin-top:-20px !important;width:95%">

          <q-table title="Участники"
           :pagination.sync="pagination"
           :separator="separator" :filter="tblfilter"
           :visible-columns="tblvisibleColumns" :data="arrDictUsersList"
            :columns="columns" row-key="id"
            binary-state-sort
        rows-per-page-label="Записей на странице"
        no-data-label="Нет данных"
        no-results-label="Поиск не дал результатов" >

      <template slot="top-right" slot-scope="props">
      <q-btn class="q-mr-md float-left" color="secondary" outline @click="openEmplViewPage('new')">Добавить</q-btn>
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
          <q-select
            class="q-mx-md"
            outlined
            dense
            v-model="separator"
            v-show="cellLines && mode === 'list'"
            :options="separatorOptions"
            emit-value
            map-options
            hide-underline
          >
            <q-tooltip>Отображение границ таблицы</q-tooltip>
          </q-select>
      </template>
      <q-tr slot="body" slot-scope="props" :props="props" @click="doSelAll(0)">
        <q-context-menu>
                      <q-list link separator style="min-width: 300px; max-height: 300px;">
                        <q-item v-close-overlay @click.native="openEmplViewPage(props.row.id)">
                          <q-item-main label="Редактирование" sublabel="Редактирование сведений" />
                                    <q-item-section avatar>
                          <q-icon name="perm_contact_calendar" color="blue"/>
                          </q-item-section>
                        </q-item>
                        <q-item v-close-overlay @click.native="showOtherToast()">
                          <q-item-main label="Отчетная форма" />
                              <q-item-section avatar>
                          <q-icon name="assignment" color="orange"/>
                          </q-item-section>
                        </q-item>
                      </q-list>
                    </q-context-menu>
                  <q-td key="colid" :props="props" style="width:50px">
                <!-- <q-icon class="text-teal-5" name="fas fa-key" style="font-size: 16px;">
                    <q-tooltip>Пароль для тестирования сформирован</q-tooltip>
                </q-icon> -->
                    <div v-bind:style=" props.row.status ==-1 ? 'width:50px;text-decoration:line-through':'width:50px;' ">{{ props.row.id }}
                    <q-tooltip>Правый клик</q-tooltip>
               </div>
            </q-td>

        <q-td key="cfirstName" :props="props">
          {{ props.row.firstName }}

        </q-td>
        <q-td key="clastName" :props="props">
          {{ props.row.lastName }}
        </q-td>
        <q-td  key="cusername" :props="props">{{ props.row.username }}</q-td>
        <q-td  key="corgname" :props="props">{{ props.row.orgname }}</q-td>
        <q-td key="crolename" :props="props" >
          {{ props.row.rolename }}
           </q-td>
      </q-tr>
    </q-table>
    <!-- {{arrDictUsersList}} -->
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
//import moment from 'moment'
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
         rowsPerPage: 50
      },
      strMessage:'',
      tblfilter:'',
      arrDictionaryList: [],
      arrDictUsersList: [],
      tblvisibleColumns:['colid','cusername','corgname','crolename'],
       mode: "list",
      cellLines: true,
       separator: "cell",
      separatorOptions: [
        { label: "Горизонтально", value: "horizontal" },
        { label: "Вертикально", value: "vertical" },
        { label: "Ячейка", value: "cell" },
        { label: "Нет", value: "none" }
      ],
       columns: [{name: 'colid',label: '#',align: 'left', field: 'id',requierd:true, sortable: true},
        { name: 'cfirstName',label: 'Фамилия',align: 'left', field: 'firstName',sortable: true},
        {name: 'clastName', label: 'Имя', align: 'left', field: 'lastName', sortable: true},
        { name: 'cusername', label: 'Логин', field: 'username', align: 'left', sortable: true },
       { name: 'corgname', label: 'Команда', field: 'orgname',  align: 'left', sortable: true },
        { name: 'crolename', label: 'Роль', field: 'rolename', align: 'left', sortable: true }
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
        openEmplViewPage: function (emplid) {
         let route = this.$router.resolve({path: '/editUserPage'});
         window.open(route.href+"/"+emplid, "_blank");
          //window.open("../#/OneSortPage/"+emplid, "_blank");
      },
     dosave(prop){
        //window.alert(prop.id+" "+prop.name+" "+prop.value);
       PortalApi.SetDictValue(prop);
        //this.strMessage = prop.id+" "+prop.name+" "+prop.value;
     }
  },
  computed: {
  },
  async mounted() {
     vm=this;
    vm.loadProc=true;
      // await PortalApi.GetDictValues().then((response) => {
      //     if (response.status === 200) {
      //       this.arrDictionaryList = response.data;
      // }})
      await PortalApi.DoGetAllUsers().then((response) => {
          if (response !== null) {
            this.arrDictUsersList = response;
             vm.loadProc=false;
      }})
       //this.arrDictUsersList = await PortalApi.DoGetAllUsers();
  },
  filters: {

  }
}
</script>

<style lang="stylus">
</style>
