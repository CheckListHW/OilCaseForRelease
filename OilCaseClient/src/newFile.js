export default (await import('vue')).defineComponent({
  name: 'App',
  created() {
    var loggedIn = this.$cookie.get('oilcase_cookie');
    var loggedId = this.$cookie.get('geocases_suserid');
    if (loggedIn !== "undefined" && loggedIn !== null && loggedId !== "undefined" && loggedId !== null) {
      this.$store.dispatch('SET_LOGGED', true);
      this.$store.state.bLogged = true;
      this.$store.state.sUserName = localStorage.getItem('username');
      this.$store.state.suserid = localStorage.getItem('geocases_suserid');
    } else {
      this.$store.dispatch('SET_LOGGED', false);
    }
  }
});
