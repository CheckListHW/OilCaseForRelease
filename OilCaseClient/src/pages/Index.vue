<template>
  <q-page padding class="row justify">
    <div class="row" v-if="false">
      <div class="col-12">
        <img alt="Quasar logo" src="~assets/app-logo.png">
      </div>
    </div>

    <div class="fixed-center text-center">
      <h5 class="q-pa-lg">Для работы с платформой необходимо авторизоваться </h5>
      <p>
        <router-link class=" q-ml-lg q-pl-lg" to="/LoginPage">переход к странице авторизации</router-link>
      </p>
    </div>

    <p v-if="false">
      <router-link class=" q-ml-lg q-pl-lg" to="/make3dScene">3D Scene</router-link>
    </p>

  </q-page>
</template>

<style>
</style>

<script>

export default {
  name: 'PageIndex',

  computed: {
    StoreLogged() {
      return this.$store.getters.BLOGGED;
    },
    StoreMODELMODE() {
      return this.$store.getters.MODELMODE;
    },

  }
}
</script>
