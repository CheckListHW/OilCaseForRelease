<template>
  <q-layout view="hHh LpR lFf">
    <q-layout-header class="no-shadow">
      <q-toolbar color="primary">
        <q-btn v-show="StoreAdminLogged"
               flat
               dense
               round
               @click="leftDrawerOpen = !leftDrawerOpen"
               aria-label="Menu">
          <q-icon name="menu"/>
        </q-btn>
        <q-toolbar-title>OilCase X Final 2022</q-toolbar-title>

        <span style="font-size: 9pt"
              v-if="bCoordRecieve"
              class="text-weight-thin">{{ pointerPosition }}</span>
        <span class="text-weight-thin q-mx-lg">{{ StoreGetUserName }}</span>
        <span class="mainlink q-mx-lg">|</span>

        <router-link class="mainlink" to="/legend">Легенда</router-link>
        <router-link v-show="StoreLogged" class="mainlink q-ml-lg" to="/tasks">
          Карта
        </router-link>
        <router-link v-show="StoreLogged"
                     class="mainlink q-ml-lg"
                     to="/result">
          Результаты
        </router-link>
        <router-link v-show="StoreLogged"
                     class="mainlink q-ml-lg"
                     to="/ThreeDScene">
          3D обзор
        </router-link>

        <span class="mainlink q-mx-lg">|</span>

        <div v-show="StoreLogged" @click="doScreenShot()">
          <q-icon name="photo_camera"/>
          <q-tooltip class="bg-blue-grey-6 text-body2" style="height: 50px">
            Отправка скриншота экрана в службу поддержки
          </q-tooltip>
        </div>

        <div v-if="false"
             v-show="StoreLogged"
             class="q-mx-lg"
             @click="bOpenSetting = true">
          <q-icon name="gesture"/>
          <q-tooltip class="bg-blue-grey-6 text-body2">
            Настройка совместной работы<br/>команды - передача и прием
            указателя мыши
          </q-tooltip>
        </div>

        <div v-if="false">
          <q-icon name="report"/>
          <q-tooltip class="bg-blue-grey-6 text-body2"
                     anchor="top left"
                     self="bottom right"
                     :offset="[10, 15]">
            Сведения
            <br/>
            <span style="font-size: 10pt" class="text-weight-thin">nv05.02|10</span>
            <br/>
            <span class="text-weight-thin"> {{ StoreGetGroupName }}</span
            ><br/>
            <span class="text-weight-thin"> {{ StoreGetUserRole }}</span
            ><br/>
            <span class="text-weight-thin">{{ StoreGetmodelMode }}</span>
          </q-tooltip>
        </div>
        <div class="q-mr-md" style="width: 30px">
          <q-btn size="lg" icon="account_circle_outline">
            <q-popover class="q-mt-sm" anchor="bottom right" self="top right">
              <q-list link dense>
                <q-item>
                  <q-item-main>
                    <q-item-tile label>Сведения</q-item-tile>
                    <q-item-tile sublabel>nv05.22|09rt</q-item-tile>
                    <q-item-tile sublabel>
                      {{ StoreGetGroupName }}
                      {{ StoreGetUserRole }}
                    </q-item-tile>
                    <q-item-tile sublabel>{{ StoreGetmodelMode }}</q-item-tile>
                  </q-item-main>
                </q-item>
                <q-item-separator/>
                <q-item v-show="StoreLogged">
                  <q-item-main>
                    <div @click="bOpenSetting = true">
                      <q-item-tile label>Настройка</q-item-tile>
                      <q-item-tile sublabel>
                        Настройка совместной работы
                        <br/>
                        команды - передача и
                        <br/>
                        прием указателя мыши
                      </q-item-tile>
                    </div>
                  </q-item-main>
                </q-item>
                <q-item-separator/>
                <q-item>
                  <q-item-main>
                    <q-item-tile label>Об OilCase X Final 2022</q-item-tile>
                  </q-item-main>
                </q-item>
                <q-item-separator/>
                <q-item>
                  <q-item-main>
                    <div @click="clickLogout()">
                      <q-item-tile label>Выход</q-item-tile>
                    </div>
                  </q-item-main>
                </q-item>
              </q-list>
            </q-popover>
          </q-btn>
        </div>
      </q-toolbar>
    </q-layout-header>
    <q-layout-drawer v-show="StoreLogged"
                     v-model="leftDrawerOpen"
                     :content-class="$q.theme === 'mat' ? 'bg-grey-2' : null">
      <q-list no-border link inset-delimiter>
        <q-item to="/showUserLog">
          <q-item-main label="Логи" sublabel="Логи решения кейса"/>
        </q-item>
        <q-item to="/showDictMode">
          <q-item-main label="Справочники"
                       sublabel="Настройка справочных сведений"/>
        </q-item>
      </q-list>
    </q-layout-drawer>
    <q-modal v-model="bOpenSetting">
      <q-modal-layout>
        <q-toolbar slot="header">
          <q-toolbar-title>
            <div>Настройки совместной работы</div>
          </q-toolbar-title>
        </q-toolbar>
        <div class="layout-padding" style="padding: 2em !important">
          <q-toggle v-model="bCoordSend"
                    label="Транслировать курсор"/>
          <br/>
          <br/>
          <q-btn class="q-ml-lg q-my-sm"
                 color="positive"
                 @click.prevent="bOpenSetting = false">
            Закрыть
          </q-btn>
        </div>
      </q-modal-layout>
    </q-modal>
    <q-modal v-model="bSendScreen"
             :content-css="{ minWidth: '850px', minHeight: '400px' }">
      <q-modal-layout>
        <q-toolbar slot="header">
          <q-toolbar-title>
            <div>Обращение в службу подержки</div>
          </q-toolbar-title>
          <q-btn flat round dense icon="close" @click.prevent="bSendScreen = false"/>
        </q-toolbar>
        <div class="layout-padding" style="padding: 1em !important">
          <div class="row">
            <div class="col-8">
              <q-input class="q-ma-sm"
                       type="textarea"
                       v-model="txtMessage"
                       float-label="Текст обращения"/>
            </div>

            <div class="col-3 q-ml-md">
              <q-btn class="q-my-sm" @click="imageUrl = tmpScreenImage">
                Скриншот
              </q-btn>
              <br/>
              или выбрать файл
              <br/>
              <br/>
              <input placeholder="Выберите файл скриншота"
                     class="file-input"
                     ref="fileInput"
                     type="file"
                     @input="onSelectFile"/>
              <br/>
              <q-btn class="q-my-md"
                     :disable="txtMessage.length < 1"
                     @click="doSend2Support"
                     color="purple">Отправить
              </q-btn>
            </div>
          </div>
          <img :src="imageUrl" width="800" alt="error"/>
        </div>
      </q-modal-layout>
    </q-modal>
    <div @mousemove="sendPointerPosition">
      <div ref="pointer"
           class="pointer"
           :style="pointerPosition"
           v-if="bCoordSend">
        <span>
          {{ pointerPosition.usname }}
        </span>
      </div>
      <q-page-container>
        <router-view/>
      </q-page-container>
    </div>
  </q-layout>
</template>

<script>
import PortalApi from "src/api/OilCaseApi.js";
import EventBus from "src/event-bus";
import SecureLS from "secure-ls";
import html2canvas from "html2canvas";
import {Notify} from "quasar";
import {OutSideMixin} from "src/scriptslibs/scenescripts.js";
import {MouseHub} from "src/api/mousehub.js";
import Vue from "vue";
import OilcaseApi from "src/api/OilCaseApi.js";

export default {
  name: "MyLayout",

  mixins: [OutSideMixin],

  data() {
    return {
      leftDrawerOpen: false,
      imodelmode: -1,
      fileScreen: null,
      txtMessage: "",
      bOpenSetting: false,
      bSendScreen: false,
      megaObj: {},
      bCoordSend: false,
      bCoordRecieve: false,
      coord1: {},
      pointerPosition: {},
      dScroll: 0,
      tmpScreenImage: "",
      imageUrl: "",
    };
  },

  created() {
    EventBus.$on("capitanEndStep", (_) => {
      PortalApi.DoLoadCase();
      this.$router.push('/endStep');
    });

    EventBus.$on("eventAfterStepEnd", (_) => {
      this.doSendSignalMessage(
        this.connection,
        this.megaObj.orgname,
        "Капитан завершил ход.",
        this.megaObj.id
      );
    });

    EventBus.$on("eventSendSMailToSupport", (eventMess) => {
      if (eventMess === "sendmailtosupport") {
        Notify.create({type: "info", message: "Сообщение отправлено"});
      }
    });
  },

  mounted() {
    console.log('body')
    var appLink = window.location.origin + window.location.pathname;
    this.pointerPosition = this.pointerPosition = {
      left: "0px",
      top: "0px",
      usname: "",
    };

    // this.hub = new MouseHub(process.env.NODE_ENV === "development" ? localStorage.getItem("baseUrl") : appLink)
    // this.hub = new MouseHub(localStorage.getItem("baseUrl"))

    document.addEventListener(
      "scroll",
      this.sendScrollPosition,
      true
    );
  },

  methods: {
    async doSend2Support() {
      try {
        var formData = new FormData();
        formData.append("useid", this.megaObj.userid);
        formData.append(
          "username",
          this.megaObj.orgname +
          "|" +
          this.megaObj.username +
          "|" +
          this.megaObj.userrole
        );
        formData.append("message", this.txtMessage);
        formData.append("sreenData", this.imageUrl);

        await PortalApi.SendSMailToSupport(formData);
      } catch (error) {
      }
    },
    onSelectFile() {
      const input = this.$refs.fileInput;
      const files = input.files;
      this.FileImage = files[0];
      if (files && files[0]) {
        const reader = new FileReader();
        reader.onload = (e) => {
          this.imageUrl = e.target.result;
        };
        reader.readAsDataURL(files[0]);
      }
    },
    doScreenShot() {
      this.imageUrl = null;

      html2canvas(document.querySelector("body")).then((canvas) => {
        this.tmpScreenImage = canvas.toDataURL();
        if (this.tmpScreenImage !== "") {
          this.bSendScreen = true;
        }
      });
    },
    sendScrollPosition: function (event) {
      if (this.bCoordSend && event.target.scrollingElement !== null) {
        this.sendScroll(
          this.connection,
          event.target.scrollingElement.scrollTop,
          this.megaObj.username,
          this.megaObj.orgname
        );
      }
    },
    sendPointerPosition: function (event) {
      if (this.bCoordSend) {
        this.sendCoord(
          this.connection,
          event.clientX,
          event.clientY,
          this.megaObj.username,
          this.megaObj.orgname
        );
      }
    },
    clickLogout() {
      OilcaseApi.DoLogout()
      Vue.cookie.set("oilcase_cookie", null)
      this.$store.dispatch("SET_LOGGED", false);
      this.$cookie.set("oilcase_cookie", "");
      this.$store.state.bLogged = false;
      this.$store.state.sUserName = "";
      this.$store.state.suserid = "";
      localStorage.setItem("geocases_suserid", "");
      localStorage.setItem("oilcase_cookie", "");
      this.megaObj = null
      var scLs = new SecureLS();
      scLs.removeAll();
      this.$router.push("/LoginPage");
    },
  },

  computed: {
    StoreAdminLogged() {
      return (
        this.megaObj !== null &&
        this.megaObj.userrole !== null &&
        this.megaObj.userrole === "admin"
      );
    },
    StoreLogged() {
      return this.megaObj !== null && this.megaObj.userrole !== null;
    },
    StoreGetUserName() {
      if (this.megaObj !== null) {
        return this.megaObj.username;
      } else {
        return "";
      }
    },
    StoreGetUserRole() {
      if (this.megaObj !== null) {
        return this.megaObj.userrole;
      } else {
        return "";
      }
    },
    StoreGetGroupName() {
      if (this.megaObj !== null) {
        return this.megaObj.orgname;
      } else {
        return "";
      }
    },
    StoreGetmodelMode() {
      return "model " + this.imodelmode;
    },
  },
};
</script>

<style lang="stylus">
.mainlink
  color #fff
  text-decoration-line none !important
  font-size 16px
  font-weight 500
</style>
