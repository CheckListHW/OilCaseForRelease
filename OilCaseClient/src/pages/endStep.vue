<template>
  <q-page padding class="row justify">
    <div class="fixed-center text-center">
      <h5>Ход {{ iCurGameStep }} завершен</h5>
    </div>
  </q-page>
</template>

<script>

import SecureLS from "secure-ls";
export default {
  name: 'endStep',
  data() {
    return {
      iCurGameStep: -1
    }
  },
  computed: {},
  created() {
    let secLS = new SecureLS();
    if (secLS.ls['ocData'] === null) {
      return;
    }

    let megaObj = secLS.get('ocData');
    this.iCurGameStep = megaObj.iCurGameStep;
  }
}
</script>
