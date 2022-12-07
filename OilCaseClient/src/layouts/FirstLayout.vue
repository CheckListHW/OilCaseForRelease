<template>
  <q-layout view="hHh lpR lFf">
    <q-layout-header class="no-shadow">
      <q-toolbar color="primary">
        <q-btn v-if="false" v-show="StoreAdminLogged" flat dense round @click="leftDrawerOpen = !leftDrawerOpen"
               aria-label="Menu">
          <q-icon name="menu"/>
        </q-btn>
        <q-toolbar-title>OilCase X Final 2022</q-toolbar-title>
        <span class=" mainlink q-mx-lg">|</span>

        <router-link class="mainlink q-ml-lg q-pl-lg" to="/LoginPage">login</router-link>
      </q-toolbar>
    </q-layout-header>
    <q-layout-drawer v-if="false" v-show="StoreLogged" v-model="leftDrawerOpen"
                     :content-class="$q.theme === 'mat' ? 'bg-grey-2' : null">
    </q-layout-drawer>
    <q-page-container>
      <router-view/>
    </q-page-container>
  </q-layout>
</template>

<script>
export default {
  name: 'MyLayout',
  data() {
    return {
      leftDrawerOpen: false,
      imodelmode: -1
    }
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
