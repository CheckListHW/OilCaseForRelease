import Vue from 'vue'
import Axios from 'axios'
import VueCookie from 'vue-cookie'
import store from 'src/store'
import Vuex from 'vuex'
import {UserData} from './DataForm'
import {MapObjectOfArrangement} from "./MapObjectOfArrangement";

Vue.use(Vuex)
Vue.use(VueCookie)
localStorage.setItem("baseUrl", process.env.NODE_ENV === 'development' ? 'https://localhost:16745/' : './')
const client = Axios.create({
  baseURL: localStorage.getItem("baseUrl") + 'api/v1',
  json: true
})

Vue.prototype.$http = Axios;

const accessToken = Vue.cookie.get('oilcase_cookie');

if (accessToken) {
  Vue.prototype.$http.defaults.headers.common['Authorization'] = accessToken
}

export default {
  async execute(method, resource, data) {
    if (accessToken) {
      return client({
        method,
        url: resource,
        data,
        headers: {
          Authorization: `Bearer ${accessToken}`
        }
      }).then(req => {
        return req.data
      }).catch(err => {
        return (err)
      });
    } else {
      return client({
        method,
        url: resource,
        data
      }).then(req => {
        return req.data
      }).catch(err => {
        return (err)
      });
    }
  },


  DoLogin: async function (username, password) {
    store.state.bLogged = false;
    return await this.execute('post', `/Login`, {
      username: username,
      password: password
    }).then(resp => {
      if ((typeof resp) === "string") {
        this.GetInfo()
        Vue.cookie.set('oilcase_cookie', resp, "7d")
        return true
      } else {
        return 'login error ' + resp.message;
      }
    }).catch(err => {
      return 'login error ' + err;
    })
  },

  DoLogout: async function () {
    this.execute('get', `/Logout`).then(resp => {
    })
  },

  DoLogCase: async function (_, actionType, actionData) {
    this.execute('post', `/TeamActionLogs/`, {
      "TypeAction": actionType,
      "Description": actionData
    })
      .then(resp => {
        return 'saveLogCase good';
      })
      .catch(err => {
        return 'saveLogCase error ' + err;
      });
  },


  GetInfo: async function (){
    console.log("[Log] GetInfo")
    return this.execute('get', `/Info/`)
      .then(resp => {
        UserData.team = resp.teamInfo.name
        UserData.name = resp.username
        UserData.role = resp.role
        return resp;
      })
      .catch(err => {
        return 'saveLogCase error ' + err;
      });
  },

  GetLithologicalModelInfo: async function (){
    return this.execute('get', `/LithologicalModel/`)
      .then(resp => {
        return resp;
      })
      .catch(err => {
        return 'saveLogCase error ' + err;
      });
  },


  PostMapObjectOfArrangement: async function (objectName, cellX, cellY, subCellX, subCellY) {
    console.log('[Log] PutMapObjectOfArrangement')
    this.execute('post', `/LithologicalData/ObjectOfArrangement/`, {
      "CellX": cellX,
      "CellY": cellY,
      "SubCellX": subCellX,
      "SubCellY": subCellY,
      "key": objectName
    })
      .then(resp => {
        return 'success';
      })
      .catch(err => {
        return 'failed' + err;
      });
  },

  DeleteMapObjectOfArrangement: async function (cellX, cellY, subCellX, subCellY) {
    console.log('[Log] DeleteMapObjectOfArrangement')
    return  this.execute('delete', `/LithologicalData/ObjectOfArrangement/`, {
      "CellX": cellX,
      "CellY": cellY,
      "SubCellX": subCellX,
      "SubCellY": subCellY,
      "ObjectOfArrangement": {
        "key": "None"
      }
    })
      .then(resp => {
        return true;
      })
      .catch(err => {
        return false;
      });
  },

  GetMapObjectOfArrangement: async function () {
    return this.execute('get', `/LithologicalData/ObjectOfArrangement/`)
      .then(resp => {
        console.log(resp)
        return resp
      })
      .catch(err => {
        return 'saveLogCase error ' + err;
      });
  },


  PostSeismic: async function (startCellX, startCellY, endCellX, endCellY) {
    console.log('[Log] PutSeismic')
    return  this.execute('post', `/Purchased/Seismic/`, {
      "StartCellX": startCellX,
      "StartCellY": startCellY,
      "EndCellX": endCellX,
      "EndCellY": endCellY,
    })
      .then(resp => {
        return true;
      })
      .catch(err => {
        console.log(err)
        return false;
      });
  },

  DeleteSeismic: async function (startCellX, startCellY, endCellX, endCellY) {
    console.log('[Log] DeleteSeismic')
    return  this.execute('delete', `/Purchased/Seismic/`, {
      "StartCellX": startCellX,
      "StartCellY": startCellY,
      "EndCellX": endCellX,
      "EndCellY": endCellY,
    })
      .then(resp => {
        return true;
      })
      .catch(err => {
        console.log(err)
        return false;
      });
  },

  GetSeismic: async function () {
    console.log('[Log] GetSeismic')
    return  this.execute('get', `/Purchased/Seismic/`)
      .then(resp => {
        return resp;
      })
      .catch(err => {
        return false;
      });
  },


  PutBoreholeProduction: async function (boreholeStatus, boreholeType, name, trajectoryPoints) {
    console.log('[Log] PutBorehole')
    return  this.execute('put', `/Purchased/BoreholeProduction/`, {
      "BoreholeStatus": boreholeStatus,
      "BoreholeType": boreholeType,
      "Name": name,
      "trajectoryPoints": trajectoryPoints,
    })
      .then(resp => {
        console.log(resp)
        return true;
      })
      .catch(err => {
        console.log(err)
        return false;
      });
  },

  DeleteBoreholeProduction: async function (id) {
    console.log('[Log] DeleteBorehole')
    return  this.execute('delete', `/Purchased/BoreholeProduction/${id}`)
      .then(resp => {
        console.log(resp)
        return true;
      })
      .catch(err => {
        console.log(err)
        return false;
      });
  },

  GetBoreholeProduction: async function () {
    console.log('[Log] GetBorehole')
    return  this.execute('get', `/Purchased/BoreholeProduction/`)
      .then(resp => {
        return resp;
      })
      .catch(err => {
        return false;
      });
  },


  PutBoreholeExploration: async function (boreholeStatus, boreholeType, name, trajectoryPoints) {
    console.log('[Log] PutBorehole')
    return  this.execute('put', `/Purchased/BoreholeExploration/`, {
      "BoreholeStatus": boreholeStatus,
      "BoreholeType": boreholeType,
      "Name": name,
      "trajectoryPoints": trajectoryPoints,
    })
      .then(resp => {
        console.log(resp)
        return true;
      })
      .catch(err => {
        console.log(err)
        return false;
      });
  },

  DeleteBoreholeExploration: async function (id) {
    console.log('[Log] DeleteBorehole')
    return  this.execute('delete', `/Purchased/BoreholeExploration/${id}`)
      .then(resp => {
        console.log(resp)
        return true;
      })
      .catch(err => {
        console.log(err)
        return false;
      });
  },

  GetBoreholeExploration: async function () {
    console.log('[Log] GetBorehole')
    return  this.execute('get', `/Purchased/BoreholeExploration/`)
      .then(resp => {
        return resp;
      })
      .catch(err => {
        return false;
      });
  },


  CompleteMove: async function (){
    console.log('[Log] Post CompleteMove')
    return  this.execute('post', `/CompleteMove/`)
      .then(resp => {
        return resp;
      })
      .catch(err => {
        return false;
      });
  },


  GetTypesOfResearch: async function (){
    console.log('[Log] GetTypesOfResearch')
    return  this.execute('get', `/LogName/`)
      .then(resp => {
        return resp;
      })
      .catch(err => {
        return false;
      });
  },


  // GetBoreholeResearch: async function(){
  //   console.log('[Log] PutBoreholeResearch')
  //   return  this.execute('get', `/PurchasedLogName/`)
  //     .then(resp => {
  //       return resp;
  //     })
  //     .catch(err => {
  //       return false;
  //     });
  // },
  //
  // PutBoreholeResearch: async function(boreholeResearches, cellX, cellY ){
  //   console.log('[Log] PutBoreholeResearch', boreholeResearches)
  //   return  this.execute('put', `/PurchasedLogName/`, {
  //     "CellX": cellX,
  //     "CellY": cellY,
  //     "LogNames": boreholeResearches
  //   })
  //     .then(resp => {
  //       console.log(resp)
  //       return resp;
  //     })
  //     .catch(err => {
  //       return false;
  //     });
  // },

  GetBoreholeLog: async function(){
    console.log('[Log] GetBoreholeLog')
    return  this.execute('get', `/LithologicalData/BoreholeLog/`)
      .then(resp => {
        return resp;
      })
      .catch(err => {
        return false;
      });
  },

  GetDataSeismic: async function(){
    console.log('[Log] Seismic')
    return  this.execute('get', `/LithologicalData/Seismic/`)
      .then(resp => {
        return resp;
      })
      .catch(err => {
        return false;
      });
  },



  // async executeFull(method, resources, data) {
  //   if (accessToken) {
  //     return client({
  //       method,
  //       url: resources,
  //       data,
  //       headers: {
  //         Authorization: `Bearer ${accessToken}`
  //       }
  //     }).then(req => {
  //       return req
  //     }).catch(err => {
  //       return (err)
  //     });
  //   } else {
  //     return client({
  //       method,
  //       url: resources,
  //       data
  //     }).then(req => {
  //       return req.data
  //     }).catch(err => {
  //       return (err)
  //     });
  //   }
  // },

  // async loadWellLogging(formData) {
  //   this.execute('post', '/Portal2Records/loadWellLogging', formData,
  //     {
  //       headers: {
  //         'Content-Type': 'multipart/form-data'
  //       }
  //     })
  //     .then(resp => {
  //       return resp;
  //     })
  //     .catch(err => {
  //       return 'loadWellLogging error ' + err;
  //     });
  // },

  // async GetCaseLogEvents(suserid) {
  //   this.execute('get', '/Portal2Records/getCaseLogEvents/' + suserid)
  //     .then(resp => {
  //       EventBus.$emit('CaseUserLogLoaded', resp);
  //       return resp;
  //     })
  //     .catch(err => {
  //       return 'saveLogcase error ' + err;
  //     });
  // },

  // async DoSaveUserData(postdata) {
  //   this.execute('post', '/users/saveuserinfo1', postdata)
  //     .then(resp => {
  //       if (resp !== "") {
  //         EventBus.$emit('UserDataSaved', resp);
  //         return resp;
  //       }
  //     })
  //     .catch(err => {
  //       // store.state.bLogged = false;
  //       return 'savecase error ' + err;
  //     });
  // },

  // async DoLoadUserInfo1(userid) {
  //   var postdata = {
  //     Id: userid, FirstName: null,
  //     LastName: null, Username: null, Password: null
  //   }
  //   this.execute('post', '/users/loaduserinfo1', postdata)
  //     .then(resp => {
  //       if (resp !== "" && resp !== "[]") {
  //         EventBus.$emit('UserDataLoaded', resp);
  //         return resp;
  //       }
  //     })
  //     .catch(err => {
  //       // store.state.bLogged = false;
  //       return 'savecase error ' + err;
  //     });
  // },

  // async DoLoadCase4lod(suserid) {
  //   this.execute('get', '/Portal2Records/loadCaseLogData/' + suserid)
  //     .then(resp => {
  //       if (resp !== "" && resp !== "[]") {
  //         EventBus.$emit('CaseUserLogLoaded2', resp);
  //         return resp;
  //       }
  //     })
  //     .catch(err => {
  //       return 'savecase error ' + err;
  //     });
  // },

  // async DoLoadCase() {
  //   let secLS = new SecureLS();
  //   var megaObj = secLS.get('ocData');
  //
  //   var suserid = megaObj.userid || 0;
  //
  //   this.execute('get', '/Portal2Records/loadCaseData/' + suserid)
  //     .then(resp => {
  //       if (resp !== "" && resp !== "[]") {
  //         megaObj.arrDrillsList = resp.arrWELLLIST;
  //         megaObj.arr2DProfileList = resp.arr2DPROFLIST;
  //         megaObj.arr3DProfileList = resp.arr3DCUBELIST;
  //         megaObj.arrSurfObjList = resp.arrSURFOBJLIST;
  //         megaObj.arrCommon = JSON.parse(resp.arrCommon);
  //         megaObj.id = resp.id;
  //         megaObj.laststdate = resp.laststdate;
  //         megaObj.iCurGameStep = resp.iCurGameStep;
  //
  //         secLS.set('ocData', megaObj);
  //
  //         EventBus.$emit('CaseDataLoaded', 'dfr');
  //         return 'load good';
  //       } else {
  //         megaObj.arrDrillsList = [];
  //         megaObj.arr2DProfileList = [];
  //         megaObj.arr3DProfileList = [];
  //         megaObj.arrSurfObjList = [];
  //         megaObj.arrCommon = '';
  //
  //
  //         secLS.set('ocData', megaObj);
  //         return 'load bad';
  //       }
  //     })
  //     .catch(err => {
  //       // store.state.bLogged = false;
  //       return 'savecase error ' + err;
  //     });
  // },

  // async DoSaveCase(iGameStep) {
  //   let secLS = new SecureLS();
  //
  //   if (secLS.ls['ocData'] === null) {
  //     this.apiloadvisible = false;
  //     return 'savecase error ' + err;
  //   }
  //
  //   var startDate = new Date();
  //
  //   let megaObj = secLS.get('ocData');
  //   if (megaObj.arrDrillsList.length === 0) {
  //     megaObj.arrDrillsList = '';
  //   }
  //   if (megaObj.arr2DProfileList.length === 0) {
  //     megaObj.arr2DProfileList = '';
  //   }
  //   if (megaObj.arr3DProfileList.length === 0) {
  //     megaObj.arr3DProfileList = '';
  //   }
  //   if (megaObj.arrSurfObjList.length === 0) {
  //     megaObj.arrSurfObjList = '';
  //   }

  // GetWellKoord(cellI, cellJ) {
  //   return this.execute('get', `/Portal2Records/GetWellKoord/${cellI}/${cellJ}`)
  // },

  // SetDictValue(data) {
  //   this.execute('post', `/Portal2Records/SetDictValue`, data).then(resp => {
  //     if (resp.message === 'good') {
  //       EventBus.$emit('evbusSetDictValue', 'good');
  //     } else {
  //       EventBus.$emit('evbusSetDictValue', 'error');
  //     }
  //     return 'SetDictValue good';
  //   }).catch(err => {
  //     EventBus.$emit('evbusSetDictValue', 'error');
  //     return 'SetDictValue error ' + err;
  //   });
  // },
  // GetDictValues() {
  //   return this.executeFull('get', `/Portal2Records/GetDictValues`)
  // },
  // Get2dTimeImages(widx, wtype) {
  //   return this.execute('get', `/Portal2Records/Get2dTimeImages/${widx}/${wtype}`)
  // },
  // GetWellDataZip(wID) {
  //   return this.execute('get', `/Portal2Records/GetWellDataZip/` + wID)
  // },
  // DoGetAllUsers() {
  //   return this.execute('get', `/users/getuserslist`)
  // },
  // GetWellDataIntrv(cI, cJ, cKl, cKt) {
  //   var tmpIntVar = parseInt(localStorage.getItem("geocases_modelmode")) || -1;
  //   return this.execute('get', `/Portal2Records/GetWellDataIntrv/${cI}/${cJ}/${cKl}/${cKt}/${tmpIntVar}`)
  // },
  // GetWellDataIntrv21(cI, cJ, cKl, cKt) {
  //   return this.execute('get', `/Portal2Records/GetWellDataIntrv21/${cI}/${cJ}/${cKl}/${cKt}`)
  // },
  // GetWellDataIntrv22(cI, cJ, cKl, cKt) {
  //   return this.execute('get', `/Portal2Records/GetWellDataIntrv22/${cI}/${cJ}/${cKl}/${cKt}`)
  // },
  // GetWellDataDeep21(cI, cJ, cDeep, iMode) {
  //   return this.execute('get', `/Portal2Records/GetWellDataDeep21/${cI}/${cJ}/${cDeep}/${iMode}`)
  // },
  // GetWellDataDeep(cI, cJ, cDeep, iMode) {
  //   return this.execute('get', `/Portal2Records/GetWellDataDeep/${cI}/${cJ}/${cDeep}/${iMode}`)
  // },
  // Get2DLayer(cType, cLine) {
  //   return this.execute('get', `/Portal2Records/Get2DLayer/${cType}/${cLine}`)
  // },
  // GetSeismic2D(cType, cLine, seismType, userId) {
  //   return this.execute('get', `/Portal2Records/GetSeismic2D/${cType}/${cLine}/${seismType}/${userId}`)
  // },
  // GetTopDep(iCoord, jCoord) {
  //   return this.execute('get', `/Portal2Records/GetTopDep/${iCoord}/${jCoord}`)
  // },
  // GetSurfaceRange(cCol_l, cCol_r, cRow_b, cRow_t) {
  //   return this.execute('get', `/Portal2Records/GetSurfaceRange/${cCol_l}/${cCol_r}/${cRow_b}/${cRow_t}`)
  // },
  // GetSurfaceLayer(cCol_l, cCol_r, cRow_b, cRow_t, sLayer, iModelMode) {
  //   return this.execute('get', `/Portal2Records/GetSurfaceLayer/${cCol_l}/${cCol_r}/${cRow_b}/${cRow_t}/${sLayer}/${iModelMode}`)
  // },
  // GetSurfaceLayer21(cCol_l, cCol_r, cRow_b, cRow_t, sLayer, iModelMode) {
  //   return this.execute('get', `/Portal2Records/GetSurfaceLayer21/${cCol_l}/${cCol_r}/${cRow_b}/${cRow_t}/${sLayer}/${iModelMode}`)
  // },
  // GetAllSurfacesRange(cCol_l, cCol_r, cRow_b, cRow_t) {
  //   return this.execute('get', `/Portal2Records/GetAllSurfacesRange/${cCol_l}/${cCol_r}/${cRow_b}/${cRow_t}`)
  // },
  // GetAllSurfacesRangeSeism21(cCol_l, cCol_r, cRow_b, cRow_t, seismType, userId) {
  //   return this.execute('get', `/Portal2Records/GetAllSurfacesRangeSeism21/${cCol_l}/${cCol_r}/${cRow_b}/${cRow_t}/${seismType}/${userId}`)
  // },
  // SaveTempObject(team, tempObject) {
  //   return this.execute('post', `/Portal2Records/SaveTempObject/${team}/${tempObject}`)
  // },
}
