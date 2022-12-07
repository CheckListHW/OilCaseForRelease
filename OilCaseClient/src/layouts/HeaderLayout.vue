<template>
  <q-layout view="hHh lpR lFf">
    <q-layout-header class="no-shadow">
      <q-toolbar color="primary">
        <q-btn v-show="StoreAdminLogged" flat dense round @click="leftDrawerOpen = !leftDrawerOpen" aria-label="Menu">
          <q-icon name="menu"/>
        </q-btn>
        <q-toolbar-title>OilCase X Final 2022
          <span style="font-size:10pt" class="text-weight-thin">v20.10|17ps</span>
        </q-toolbar-title>
        <span class="text-weight-thin q-mx-lg">{{ StoreGetUserName }}</span>
        <span class="text-weight-thin q-mx-lg">{{ StoreGetmodelMode }}</span>

        <span class=" mainlink q-mx-lg">|</span>
        <router-link class="mainlink" to="/legend">Легенда</router-link>

        <router-link v-show="StoreLogged" class="mainlink q-ml-lg" to="/tasks">Карта</router-link>
        <router-link v-show="StoreLogged" class="mainlink q-ml-lg" to="/result">Результаты</router-link>
        <router-link v-show="StoreLogged" class="mainlink q-ml-lg" to="/3DScene">3D обзор</router-link>
        <span class=" mainlink q-mx-lg">|</span>
        <router-link v-show="!StoreLogged" class=" q-ml-lg q-pl-lg" to="/LoginPage">login</router-link>
        <q-btn v-show="StoreLogged" class="q-ml-lg" flat dense @click="clickLogout()" aria-label="Menu">
          logout
        </q-btn>


      </q-toolbar>
    </q-layout-header>
    <q-layout-drawer v-show="StoreLogged" v-model="leftDrawerOpen"
                     :content-class="$q.theme === 'mat' ? 'bg-grey-2' : null">

      <q-list no-border link inset-delimiter>
        <q-item to="/showUserLog">
          <q-item-main label="Логи" sublabel="Логи решения кейса"/>
        </q-item>


      </q-list>
    </q-layout-drawer>
    <q-page-container>
      <router-view/>
    </q-page-container>
  </q-layout>
</template>

<script>
import PortalApi from "src/api/OilcaseApi.js";

export default {
  name: 'MyLayout',
  data() {
    return {
      leftDrawerOpen: false,
      imodelmode: -1
    }
  },
  mounted() {
    this.imodelmode = parseInt(localStorage.getItem("geocases_modelmode")) || -1;

    var ldddata = localStorage.getItem('geocases_suserid') || '';
    if (ldddata === '') {
      this.$router.push('/LoginPage');
    }
  },
  methods: {
    clickLogout() {
      PortalApi.DoLogout();
      // this.$store.dispatch('SET_LOGGED', false);
      // this.$cookie.set('oilcase_cookie', '');
      // this.$store.state.bLogged = false;
      // this.$store.state.sUserName = '';
      // this.$store.state.suserid = '';
      // localStorage.setItem('geocases_suserid', '');
      // this.$router.push('/LoginPage');
      // (new SecureLS()).removeAll();
    }
  },
  computed: {
    StoreAdminLogged() {
      return this.$store.getters.SUSEROLE === '' && this.$store.getters.BLOGGED;
    },
    StoreLogged() {
      return this.$store.getters.BLOGGED;
    },
    StoreGetUserName() {
      return this.$store.state.sUserName;
    },
    StoreGetmodelMode() {
      return this.imodelmode;
    },
  }
}
</script>

<style lang="stylus">
.mainlink {
  color: #fff;
  text-decoration-line: none !important;
  font-size: 16px;
  font-weight: 500;
}
</style>
