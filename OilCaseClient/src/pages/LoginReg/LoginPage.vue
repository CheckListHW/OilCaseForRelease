<template>
  <q-page padding class="doc-container">
    <div class="fixed-center text-center">
      <h5>Добро пожаловать в платформу Online Practice!</h5>
      <h6>Вход</h6>

      <q-input v-model="username" stack-label="Имя пользователя"/>
      <q-input class="q-mt-md" v-model="password" type="password" stack-label="Пароль"/>

      <br/>
      <q-btn class="q-ma-lg" color="primary" size="lg" label="войти" @click="clickLogin()"/>

      <br> {{ smessage }}
      <br>

    </div>
    <q-btn class="q-ma-lg" v-if="false" size="sm" color="deep-orange" @click="showResetDialog=true">
      <q-icon left name="settings"/>
      <div class="q-ml-md text-center">
        Сброс кэша
      </div>
      <q-tooltip>Сброс данных, сохраненных в браузере</q-tooltip>
    </q-btn>
    <q-modal v-model="showResetDialog" :content-css="{minWidth: '600px', minHeight: '350px'}">
      <q-modal-layout>
        <q-toolbar slot="header">
          <q-toolbar-title>
            <div>Подтверждение сброса данных</div>
          </q-toolbar-title>
        </q-toolbar>
        <div class="layout-padding" style="padding:2em!important">
          <h6 style="margin-top:10px;margin-bottom:15px;">Сбросить данные, сохраненные в браузере?</h6>
          <p>При этом данные, сохраненные при завершении предыдущего хода остануться в БД</p>
          <q-btn class="q-ml-lg q-my-sm " v-if="false" color="positive" @click.prevent="resetCash()">Да, сбросить
          </q-btn>
          <q-btn class="q-mr-lg q-my-sm float-right" color="orange" @click.prevent="showResetDialog=false">Нет</q-btn>
        </div>
      </q-modal-layout>
    </q-modal>
  </q-page>
</template>

<script>

import PortalApi from "src/api/OilcaseApi.js";
import EventBus from 'src/event-bus';
import {QSpinnerGrid} from 'quasar';
import {OutSideMixin} from "src/scriptslibs/scenescripts.js";
import Vue from "vue";

export default {
  name: 'PageLogin',

  mixins: [OutSideMixin],

  data() {
    return {
      showResetDialog: false,
      smessage: '',
      loggingIn: false,
      username: '',
      password: '',
      megaObj: null,
      submitted: false
    };
  },

  created() {
  },

  methods: {
    SpinnerShow(options) {
      this.$q.loading.show(options);
    },

    async clickLogin() {
      this.SpinnerShow({
        spinner: QSpinnerGrid,
        spinnerColor: 'blue-9',
        spinnerSize: 200,
        message: 'Выполняется обработка данных...',
        messageColor: 'warning'
      });
      try {
        let result = await PortalApi.DoLogin(this.username, this.password);

        this.smessage = result;
        this.$q.loading.hide();
        this.$router.push(result === true ? '/legend' : '');
      } catch (error) {
        this.smessage = "ошибка " + error
      }
    }
  }
}
</script>
