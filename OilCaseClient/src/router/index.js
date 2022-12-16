import Vue from 'vue'
import VueRouter from 'vue-router'
import routes from './routes'
import VueKonva from 'vue-konva'
import Vuex from 'vuex'
import VueCookie from 'vue-cookie'


Vue.use(VueCookie)
Vue.use(Vuex)
Vue.use(VueRouter);
Vue.use(VueKonva);

export default function () {
  const Router = new VueRouter({
    routes,
    scrollBehavior: () => ({y: 0}),
    mode: 'history',
    base: process.env.VUE_ROUTER_BASE,
  });


  Router.beforeEach((to, from, next) => {
    const authPages = ['/LoginPage', '/'];
    if (!Vue.cookie.get('oilcase_cookie')){
      return authPages.includes(to.path) ? next() : next('/');
    }
    return authPages.includes(to.path) ? next('/Legend') : next();
  })

  return Router
}
