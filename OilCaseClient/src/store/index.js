import Vue from 'vue'
import Vuex from 'vuex'
Vue.use(Vuex)


export default new Vuex.Store({
  state: {
    notes: [],
    count: 13,
    bLogged:false,
    UserRole:'user',
    modelMode:1,
    sUserName: 'неавторизован',
    suserid: '-33',
    curWellTappedLevels:[]
  },
  getters: {
    BLOGGED: state => {
      return state.bLogged;
    },
    SUSEROLE: state => {
      return state.UserRole;
    },
    MODELMODE: state => {
      return state.modelMode;
    },

    // Compute derived state based on the current state. More like computed property.
  },
  mutations: {
    SET_LOGGED: (state, payload) => {
      state.bLogged = payload;
    },
    // Mutate the current state
  },
  actions: {
    SET_LOGGED: (context, payload)=> {
      context.commit('SET_LOGGED', payload);
    },
    // Get data from server and send that to mutations to mutate the current state
  }
})
