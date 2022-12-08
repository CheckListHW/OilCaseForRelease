/* eslint-disable eqeqeq */
<template>
  <q-page padding class="row justify" v-on:scroll.passive="sendScrollPosition">
    <div class="vertical-top">
      <div style="flex-wrap: wrap;" class="wrap q-mt-lg">
        <h4 style="margin-top:-10px">
          Текущий ход: {{ iCurGameStep }}
          <q-btn rounded class="q-mx-lg" size="lg" no-caps color="indigo-9" label="Завершить ход" v-show="isCaptain"
                 :disable="Boolean(!bAllowNextStepDay & !bAllowNextStepDay)" @click="bNextStep = true">
            <q-tooltip v-show="Boolean(!bAllowNextStepDay & !bAllowNextStepDay)">Временная блокировка действий
            </q-tooltip>
          </q-btn>
          <span class="q-ml-lg text-deep-orange-6" style="font-size:12pt" v-show="!isCaptain">
            Ход завершается капитаном.
          </span>
        </h4>
        <span class="q-ml-md text-red" v-if="strNextErr !== ''">{{ strNextErr }}</span>
        <div>
          Общие затраты: финансовые
          <b class="text-primary"> {{ totalCostMoney | formatFinance }} </b> из {{ mnyAllMax | formatFinance }}
          <div v-if="false" class="q-mx-md" style="width:60px; background: #ff5566">
            check color
          </div>
          <span class="q-ml-md"> временные</span>
          <b class="text-primary"> {{ totalCostTime | formatSut }} ({{ endDate.toLocaleDateString() }})</b>
          из {{ sutAllMax | formatSut }} (10 лет)

          <small class="q-px-md"></small>
        </div>
        <div class="row q-mt-lg">
          <div class="col-6">
            <q-btn-toggle class="q-mb-md float-left" size="md" v-model="bsCellMode" toggle-color="primary" :options="[
              { label: 'Обустройство', value: purchaseType.ObjectOfArrangement },
              { label: 'Скважины', value: purchaseType.Well },
              { label: '2D (Север-Юг)', value: purchaseType.VerticalSeismic },
              { label: '2D (Запад-Восток)', value: purchaseType.HorizontalSeismic }
            ]"/>
            <q-btn class="q-mb-md float-left" @click="bactionSheet = true" icon="settings">
              <q-tooltip> Настройка отображение карты</q-tooltip>
            </q-btn>
            <div style="border: 1px solid #96979e;" class="float-left">
              <v-stage ref="stage" :config="configKonva" @mousedown="handleStageMouseDown" @mousemove="handleMouseMove">
                <v-layer ref="layerCellLines">
                  <v-line v-for="item in drawPointMap" :key="item.id" :config="item"></v-line>
                  <v-image :config="configImg1" v-if="sImageMode === 'map1'" ref="map1"></v-image>
                  <v-image :config="configImg2" v-else-if="sImageMode === 'map2'" ref="map2"></v-image>
                  <v-image :config="configImg3" v-else-if="sImageMode === 'map3'" ref="map3"></v-image>
                  <v-image :config="configImg4" v-else-if="sImageMode === 'map4'" ref="map4"></v-image>
                  <v-image :config="configImg5" v-else-if="sImageMode === 'map5'" ref="map5"></v-image>
                </v-layer>
                <v-layer ref="layer3dProfiles" id="lay3D" v-show="if3DListLength">
                  <v-rect v-for="item in arr3DProfileList" :key="item.id" :config="item"></v-rect>
                  <v-text v-for="item in arr3DProfileList" :key="item.label.id" :config="item.label"></v-text>
                </v-layer>
                <v-layer ref="layerDisableArea" v-show="ifDisableAreaLength">
                  <v-rect v-for="item in arrDisableArea" :key="item.id" :config="item"></v-rect>
                </v-layer>
                <v-layer ref="layer2dProfiles" id="lay2D" v-show="if2DlistLength">
                  <v-line v-for="item in arr2DProfileList" :key="item.id" :config="item"></v-line>
                  <v-text v-for="item in arr2DProfileList" :key="item.lblrect.label.id" :config="item.lblrect.label">
                  </v-text>
                </v-layer>
                <v-layer ref="layerCursorHint">
                  <v-text :config="txtCursorHint" v-show="bModeCursorHint"/>
                </v-layer>
                <v-layer ref="layerSurfObjs" id="laySurf" v-show="if1SurfObjLength">
                  <v-rect v-for="item in arrSurfObjList" :key="item.id" :config="item.lblrect"></v-rect>
                  <v-text v-for="item in arrSurfObjList" :key="item.lblrect.id" :config="item.lblrect.label"></v-text>
                </v-layer>
                <v-layer ref="layerDrills" id="layWells" v-show="if1DlistLength">
                  <v-circle v-for="item in arrDrillsList" :key="item.id" :config="item"></v-circle>
                  <v-text v-for="item in arrDrillsList" :key="item.label.id" :config="item.label"></v-text>
                </v-layer>
              </v-stage>
            </div>
            <q-action-sheet v-model="bactionSheet" title="Настройки" :actions="actionsSheetItems"/>
          </div>
          <div class="col-6" v-if="bViewCellPO" style="min-width:700px">
            <div ref="cntxmenu" class="cntxmenuclass" v-if="bPOTooltip" :style="cntxmenuposition">
              {{ strPOTooltip }}
            </div>
            <div class="row q-mt-sm q-mb-sm">
              <div class="col-8">
                <div class="q-headline">
                  Объекты обустройства для участка I {{ iCellPO }} J
                  {{ jCellPO }}
                </div>
              </div>
              <div class="col-2">
                ячейка i {{ subCellX }} j {{ subCellY }}
              </div>
              <div class="col">
                <q-btn class="q-mr-md" icon="settings" v-if="false">
                  <q-tooltip>
                    Настройка отображение карты
                  </q-tooltip>
                </q-btn>
                <q-btn class="q-mr-md" outline icon="close" @click="bViewCellPO = false">
                </q-btn>
              </div>
            </div>
            <div style="border: 1px solid #96979e; z-index:-1;" class="float-left">
              <v-stage ref="stage" :config="configKonva" @mousedown="handleStageMouseDownPO"
                       @mousemove="handleMouseMovePO">
                <v-layer ref="layerCellLines">
                  <v-line v-for="item in drawPointCell" :key="item.id" :config="item"></v-line>
                  <v-image :config="configImg1" v-if="sImageMode === 'map1'" ref="map1"></v-image>
                  <v-image :config="configImg2" v-else-if="sImageMode === 'map2'" ref="map2"></v-image>
                  <v-image :config="configImg3" v-else-if="sImageMode === 'map3'" ref="map3"></v-image>
                  <v-image :config="configImg4" v-else-if="sImageMode === 'map4'" ref="map4"></v-image>
                  <v-image :config="configImg5" v-else-if="sImageMode === 'map5'" ref="map5"></v-image>
                </v-layer>
                <v-layer ref="layerSurfObjs" id="laySurf" v-show="if1SurfObjLength">
                  <v-rect v-for="item in getCurZonePO" :key="item.id" :config="item.lblrectPO"
                          @mouseover="showPOTooltip(item.lblrectPO)" @mouseout="bPOTooltip = false">
                    @mouseover="showPOTooltip(item.lblrectPO)" @mouseout="bPOTooltip = false">
                  </v-rect>
                  <v-rect v-for="item in getCurZonePO" :key="'s-'+item.id" :config="item.wellPO"></v-rect>
                  <v-text v-for="item in getCurZonePO" :key="item.lblrectPO.id" :config="item.lblrectPO.label"
                          @mouseover="showPOTooltip(item.lblrectPO)"></v-text>
                </v-layer>
              </v-stage>
            </div>
          </div>
        </div>

        <div class="row q-mt-lg">
          <div class="col-5">
            <q-chip v-show="if1MapLength" square color="blue-grey">Карты</q-chip>
          </div>
          <div class="col-2">
            <q-btn color="primary" dense label="Дополнительные опции" outline @click="doBuyMap()"/>
          </div>
        </div>
        <q-list v-show="if1MapLength" highlight inset-separator v-for="item in this.arrMapList" :key="`${item.sKey}`">
          <div class="row">
            <div class="col-9">
              <span class="q-mx-md">{{ item.sText }}</span>
            </div>
            <div class="col-3">
              <span class="q-mx-md"> {{ item.sZatMoney | formatFinance }}</span>
              <span>{{ item.sZatSut | formatSut }}</span>
            </div>
          </div>
        </q-list>

        <q-chip class="q-mt-xl" square color="deep-orange-5" v-show="if1DlistLength">Скважины</q-chip>
        <q-list v-show="if1DlistLength" highlight inset-separator v-for="item in this.arrDrillsList"
                :key="`${item.id}`">
          <q-item>
            <q-item-main>
              <div class="row">
                <div class="col-10">
                  {{ item.name }} ячейка i {{ item.iCell }} j {{ item.jCell }}
                  <span class="q-ml-md" v-if="item.addedDate !== undefined">{{ item.addedDate | dateDate }}</span>
                  <div v-if="item.modelWell === wellType.Production">
                    Эксплуатационная {{ item.wellstatusText }}
                    <br/>Глубина перехода в горизонтальный ствол/пятка/heel в м {{ item.drilldeep }}
                    <br/>Точка окончания горизонтального ствола/носок/toe
                    I: {{ item.toeI }} J: {{ item.toeJ }} K: {{ item.toeK }}
                    <div class="q-mt-md" v-if="item.arrStatus !== null">Статусы скважины
                      <div class="q-mt-sm" v-for="itemstatus in item.arrStatus" :key="`${itemstatus}`">
                        {{ itemstatus.statusDate | dateDate }} {{ itemstatus.statusText }}
                      </div>
                    </div>
                  </div>
                  <div v-else>Разведочная Глубина бурения {{ item.drilldeep }}</div>
                  <p class="q-mt-md q-ml-md">
                    Финансовые затраты
                    <q-chip square small color="primary">{{
                        item.onGameStep > 0
                          ? getWellMoneyDrillAll(calcDrillDeep(getDrillPoints(), modelWell))
                          : 0 | formatFinance
                      }}
                    </q-chip>

                    <br/>
                    <span v-show="item.onGameStep > 0">
                      базовая:
                      <b>{{ mnyWellBase | formatFinance }}</b> + бурения:
                      <b>{{
                          getWellMoneyDrill(calcDrillDeep(getDrillPoints(), modelWell)) | formatFinance
                        }}</b>
                      +
                    </span>
                    исследования:
                    <b>{{ getCostMoneyResearch(item) | formatFinance }}</b>

                    <br/>Временные затраты
                    <q-chip square small color="secondary">{{
                        item.onGameStep > 0
                          ? getWellDayDrillAll(calcDrillDeep(getDrillPoints(), modelWell))
                          : (item.researches.length * sutWellIssl) | formatSut
                      }}
                    </q-chip>
                    <br/>
                    <span v-show="item.onGameStep > 0">
                      базовые: <b>{{ sutWellBase }}</b>
                      суток + дополнительные:
                      <b>{{ getWellDayDrill(calcDrillDeep(getDrillPoints(), modelWell)) | formatSut }}</b>
                      + 1</span>
                    исследования: <b>{{ getCostDayResearch(item) | formatSut }}</b>
                  </p>

                </div>
                <div class="col-2">
                  <div class="row gutter-md">
                    <div class="col-10">
                      <q-btn v-if="item.modelWell === wellType.Exploration"
                             class="float-right  q-mr-md" color="indigo"
                             outline icon="line_style" @click.prevent="addIssl2Well(item)"
                             v-show="item.onGameStep === iCurGameStep">
                        <q-tooltip>добавление исследований</q-tooltip>
                      </q-btn>
                    </div>
                    <div class="col-2">
                      <q-btn color="red-4" outline icon="delete" @click.prevent="delRecord(item)" class="float-right"
                             v-show="item.onGameStep === iCurGameStep">
                        <q-tooltip>удаление скважины {{ item.onGameStep }} {{ iCurGameStep }}</q-tooltip>
                      </q-btn>
                    </div>
                  </div>
                  <div class="row">
                    <div class="col">
                      <q-btn v-if="item.modelWell === wellType.Production" color="secondary" class="float-right q-mt-md"
                             icon-right="directions" outline @click.prevent="doSetWellStatus(item)">
                        <q-tooltip>Изменить статус скважины</q-tooltip>
                        статус
                      </q-btn>
                    </div>
                  </div>
                </div>
              </div>

              <b v-show="item.researches.length > 0">Исследования</b>
              <br/>
              <span class="q-mt-lg" v-for="itemResearch in item.researches" :key="`${itemResearch.key}`">
                {{ itemResearch.text }}
                <br/>
              </span>
            </q-item-main>
          </q-item>
        </q-list>

        <q-chip square color="green-4  q-mt-xl" v-show="if2DlistLength">2D профили</q-chip>
        <q-list v-show="if2DlistLength" highlight inset-separator v-for="item in this.arr2DProfileList"
                :key="`${item.id}`">
          <q-item>
            <q-item-main>
              {{ item.name }} профиль i {{ item.iCell }} j
              {{ item.jCell }} сейсмические исследования:
              <b> {{ getSeismTypeText(item.seismType) }} </b>
              <span class="q-ml-md" v-if="item.addedDate !== undefined">
                {{ item.addedDate | dateDate }}
              </span>
              <q-btn color="red-4" outline icon="delete" @click.prevent="delRecord(item)" class="float-right" v-show="
              item.onGameStep === iCurGameStep || !Array.isArray(item.seismType)">
                <q-tooltip>Удаление профиля</q-tooltip>
              </q-btn>
              <q-chip v-show="!Array.isArray(item.seismType)" square color="red">Неверный профиль! Необходимо удалить!
              </q-chip>
              <br/>Финансовые затраты
              <q-chip square small color="primary">{{ getSeismicTypeMoney(item.seismType) }}
              </q-chip>
              Временные затраты
              <q-chip small square color="secondary">{{
                  sut2D | formatSut
                }}
              </q-chip>
            </q-item-main>
          </q-item>
        </q-list>

        <!--        <q-chip square color="deep-purple-3 q-mt-xl" v-show="if3DListLength">3D сейсмика</q-chip>-->
        <!--        <q-list v-show="if3DListLength" highlight inset-separator v-for="item in this.arr3DProfileList"-->
        <!--                :key="`${item.id}`">-->
        <!--          <q-item>-->
        <!--            <q-item-main>-->
        <!--              {{ item.name }} куб il {{ item.iCelll }} ir {{ item.iCellr }} jt-->
        <!--              {{ item.jCellt }} jb {{ item.jCellb }} сейсмические исследования:-->
        <!--              <b> {{ getSeismTypeText(item.seismType) }} </b>-->
        <!--              <q-btn color="red-4" outline icon="delete" @click.prevent="delRecord(item)" class="float-right"-->
        <!--                     v-show="item.onGameStep === iCurGameStep"></q-btn>-->
        <!--              <br/>Финансовые затраты-->
        <!--              <q-chip square small color="primary">{{ getSeismicTypeMoney(item.seismType) }}</q-chip>-->
        <!--              Временные затраты-->
        <!--              <q-chip square small color="secondary">{{ sut3D | formatSut }}</q-chip>-->
        <!--            </q-item-main>-->
        <!--          </q-item>-->
        <!--        </q-list>-->

        <q-chip square color="brown q-mt-xl" v-show="if1SurfObjLength">Обустройство</q-chip>

        <q-list v-show="if1SurfObjLength" highlight inset-separator v-for="item in this.arrSurfObjList"
                :key="`${item.id}`">
          <q-item>
            <q-item-main>
              {{ item.name }} - <b>{{ item.description }}</b> участок I
              {{ item.iCell }} J {{ item.jCell }} ячейка i
              {{ item.subCellX }} j {{ item.subCellY }}

              <span class="q-ml-md" v-if="item.addedDate !== undefined">
                {{ item.addedDate | dateDate }}
              </span>
              <q-btn color="red-4" outline icon="delete" @click.prevent="delRecord(item)" class="float-right"
                     v-show="item.onGameStep === iCurGameStep"></q-btn>
              <br/>
              Финансовые затраты
              <q-chip square small color="primary">{{
                  item.objModel.sZatMoney | formatFinance
                }}
              </q-chip>
              Временные затраты
              <q-chip square small color="secondary">{{
                  item.objModel.sZatSut | formatSut
                }}
              </q-chip>
            </q-item-main>
          </q-item>
        </q-list>
      </div>


      <q-modal v-model="bEditModel" :content-css="{ minWidth: '820px', minHeight: '820px' }">
        <q-modal-layout>
          <q-toolbar slot="header">
            <q-toolbar-title>
              <div v-show="sEditMode === editModes.Borehole">Добавление скважины</div>
              <div v-show="sEditMode === editModes.Researches">Добавление исследования</div>
            </q-toolbar-title>
          </q-toolbar>
          <div v-show="sEditMode === editModes.Borehole" class="layout-padding" style="padding:2em!important">
            Добавление новой скважины в ячейке
            <b>i: {{ pCurPoint.icell }} j: {{ pCurPoint.jcell }}</b>
            <p class="q-mt-lg">Вид скважины
              <q-btn-toggle outline v-model="modelWell" toggle-color="primary" :options="[
                  { label: 'Разведочная', value: wellType.Exploration },
                  { label: 'Эксплуатационная', value: wellType.Production }
                ]"/>
            </p>

            <div v-if="modelWell === wellType.Exploration">
              <p class="q-mt-lg">глубина бурения скважины</p>
              <q-slider v-model="drillDeep" :min=maxDrillDeep :max="minDrillDeep" :step="5" label-always snap
                        markers
                        color="green"/>
            </div>
            <div v-if="modelWell === wellType.Production">
              <p class="q-mt-lg">Параметры бурения горизонтальной скважины</p>
              Глубина перехода в горизонтальный ствол/пятка/heel в м
              <q-slider v-model="drillDeep" :min=maxDrillDeep :max="minDrillDeep" :step="5" label-always snap
                        markers
                        color="green"/>
              <div> Точка окончания горизонтального ствола/носок/toe
                <div class="row gutter-md">
                  <div class="col">
                    <q-input type="number" v-model="toeI" stack-label="I"/>
                  </div>
                  <div class="col">
                    <q-input type="number" v-model="toeJ" stack-label="J"/>
                  </div>
                  <div class="col">
                    <q-input type="number" v-model="toeK" stack-label="глубина"/>
                  </div>
                </div>
              </div>
            </div>

            <p class="q-mt-lg q-title">
              Финансовые затраты
              <q-chip square color="primary">{{
                  getWellMoneyDrillAll(calcDrillDeep(getDrillPoints(), modelWell)) | formatFinance
                }}
              </q-chip>
            </p>
            базовая стоимость: <b>{{ mnyWellBase | formatFinance }}</b> +
            стоимость бурения: <b>{{
              getWellMoneyDrill(calcDrillDeep(getDrillPoints(), modelWell)) | formatFinance
            }}</b>
            <p class="q-mt-lg q-title">
              Временные затраты
              <q-chip square color="secondary">{{
                  getWellDayDrillAll(calcDrillDeep(getDrillPoints(), modelWell)) | formatSut
                }}
              </q-chip>
              <span class="q-ml-md text-bold">
                  {{
                  getDrillEndDate(sutWellBase, getWellDayDrillAll(calcDrillDeep(getDrillPoints(), modelWell))) | dateDate
                }}
                </span>
            </p>
            базовые: <b>{{ sutWellBase }}</b>
            суток + дополнительные: <b>{{ getWellDayDrill(calcDrillDeep(getDrillPoints(), modelWell)) | formatSut }}</b>
          </div>

          <div v-show="sEditMode === editModes.Researches" class="layout-padding"
               style="padding-bottom:0.2em!important;padding-top:1em!important;padding-left:2em!important;">
            Исследования в скважине
            <q-chip square color="purple-3">{{ drCurSelWell.name }}</q-chip>
            глубина бурение
            <q-chip square color="deep-purple-3">{{ selIssl1Interval.min }}</q-chip>
            <br/>
            <div class="row">
              <div class="col-6">
                Укажите виды исследований ГИС:
              </div>

              <div class="col-6">
                <q-checkbox v-if="selWellOilPropsIssl.length === 0" v-model="checkValueSelectedAllResearch"
                            @input="selectAllResearch()"
                            label="Выбрать все"/>
              </div>
            </div>

            <div class="row">
              <q-option-group type="checkbox" color="primary" v-model="selectedResearches"
                              :options="getResearchesOptions"></q-option-group>
            </div>

            <div class="row q-mt-md">
              <p> Финансовые затраты:
                <q-chip square color="primary">{{ getCostMoneySelectedResearch| formatFinance }}</q-chip>
              </p>
            </div>

            <div class="row q-mt-md">
              <p> Временные затраты:
                <q-chip square color="secondary">{{ getCostDaySelectedResearch | formatSut }}</q-chip>
              </p>
            </div>

          </div>


          <q-btn :disable="apiloadvisible" class="q-mr-lg q-my-sm float-right" color="teal"
                 @click.prevent="updateRecord(pCurPoint.icell, pCurPoint.jcell)">Сохранить
          </q-btn>

          <q-btn class="q-ml-lg q-my-sm " color="orange" :disable="apiloadvisible"
                 @click.prevent="cancelRecord()">Отмена
          </q-btn>

        </q-modal-layout>
      </q-modal>


      <q-modal v-model="bEditWellStatus" :content-css="{ minWidth: '850px', minHeight: '780px' }">
        <q-modal-layout>
          <q-toolbar slot="header">
            <q-toolbar-title>
              <div>Изменение статуса скважины в ячейке i: {{ drCurSelWell.iCell }} j: {{ drCurSelWell.jCell }}</div>
            </q-toolbar-title>
          </q-toolbar>
          <div class="layout-padding" style="padding:2em!important">
            <p class="q-mt-sm">Вид скважины Эксплуатационная</p>
            Глубина перехода в горизонтальный ствол в м
            <q-chip>{{ drCurSelWell.drilldeep }}</q-chip>
            <br/>
            Точка окончания горизонтального ствола
            <br/>
            I
            <q-chip>{{ drCurSelWell.toeI }}</q-chip>
            J
            <q-chip>{{ drCurSelWell.toeJ }}</q-chip>
            глубина
            <q-chip> {{ drCurSelWell.toeK }}</q-chip>
            <br/>
            {{ endDate.toLocaleDateString() }}
            <q-option-group type="radio" color="primary" v-model="drCurSelWell.wellstatus"
                            :options="getWellMethodTypes"></q-option-group>

          </div>
          <q-btn :disable="apiloadvisible" class="q-ml-lg q-my-sm " color="teal" @click.prevent="updateWellStatus()">
            Сохранить
          </q-btn>
          <q-btn class="q-mr-lg q-my-sm float-right" color="orange" :disable="apiloadvisible"
                 @click.prevent="bEditWellStatus = false">Отмена
          </q-btn>
        </q-modal-layout>
      </q-modal>
      <q-modal v-model="bNextStep" :content-css="{ minWidth: '600px', minHeight: '450px' }">
        <q-modal-layout>
          <q-toolbar slot="header">
            <q-toolbar-title>
              <div>Завершение хода</div>
            </q-toolbar-title>
          </q-toolbar>
          <div class="layout-padding" style="padding:2em!important">
            <h6 style="margin-top:10px;margin-bottom:15px;">
              Вы подтверждаете завершение хода?
            </h6>
            <p>
              Пожалуйста, укажите запасы нефти всего лицензионного участка в
              пластовых условиях,
            </p>
            <p>
              которые <b>НА ДАННОМ ЭТАПЕ</b> вы считаете максимально близкими к
              реальным.
            </p>

            <q-input v-model="zapasocen" label="объем запасов"
                     stack-label="объем запасов в метрах кубических в пластовых условиях"/>

            <p class="q-mt-md">
              Ответ не влияет на бальную оценку и собирается только для
              статистики.
            </p>
            <h6 style="margin-top:10px;">Спасибо!</h6>

            <q-btn class="q-ml-lg q-my-sm " color="positive" @click.prevent="makeStep()">Завершить ход</q-btn>
            <q-btn class="q-mr-lg q-my-sm float-right" color="orange" @click.prevent="bNextStep = false">Отмена</q-btn>
          </div>
        </q-modal-layout>
      </q-modal>
      <q-inner-loading :visible="apiloadvisible">
        <q-spinner-gears size="50px" color="primary"></q-spinner-gears>
      </q-inner-loading>
    </div>
  </q-page>
</template>

<script>
import {OutSideMixin} from 'src/scriptslibs/scenescripts.js'

import {date} from 'quasar'
import EventBus from 'src/event-bus'
import moment from 'moment'

import OilcaseApi from "src/api/OilCaseApi.js";
import {UserData} from "../api/DataForm"
import {MapObjectOfArrangement} from "../api/MapObjectOfArrangement";
import {Properties, ObjectsOfArrangement, DictMaps} from "src/api/Properties";
import {Dialogs, Notifications, SpinnerOptions, UserActions} from "../data/ObjectTemplates/Dialogs";
import {ObjectCreator} from "../data/ObjectTemplates/ObjectCreator";
import Vue from "vue";
import oilcaseApi from "src/api/OilCaseApi.js";
import points from "echarts/src/layout/points";

const width = 800
const height = 800
const {addToDate} = date

export default {
  mixins: [OutSideMixin],
  data() {
    return {
      editModes: Properties.EditMode,
      purchaseType: Properties.PurchaseType,
      wellType: Properties.BoreholeType,

      mapObjects: new MapObjectOfArrangement(),
      typesOfResearches: new Map(),
      selectedResearches: [],

      testImg1: new Image(),
      testImg2: new Image(),
      testImg3: new Image(),
      testImg4: new Image(),
      testImg5: new Image(),

      minDrillDeep: Properties.MinDrillDeep,
      maxDrillDeep: Properties.MaxDrillDeep,
      drillDeep: Properties.MinDrillDeep,

      startDate: Properties.StartDate,
      endDate: Properties.EndDate,

      modelWell: Properties.BoreholeType.Exploration,
      bsCellMode: Properties.PurchaseType.ObjectOfArrangement,
      checkValueSelectedAllResearch: false,
      if3DListLength: true,

      actionsSheetItems: [
        {
          label: 'включить курсор ячейки',
          color: 'secondary',
          icon: 'check_box_outline_blank',
          handler: () => {
            this.toggleHint()
          }
        },
        {
          label: 'отображение геологии',
          color: 'tertiary',
          icon: 'check_box',
          handler: () => {
            this.toggleGeo()
          }
        },
        {
          label: 'поверхностное обустройство',
          color: 'primary',
          icon: 'check_box',
          handler: () => {
            this.toggleSurface()
          }
        }
      ],

      bEditWellStatus: false,
      bDictFlag: false,
      cntxmenuposition: {x: '45px', y: '150px'},
      strPOTooltip: '',
      secLS: null,
      bAllIssl: false,
      iCurGameStep: 1,
      seismType: "",
      strNextErr: '',
      bAllowNextStepDay: true,
      bAllowNextStepMoney: true,
      arrDictParams: [],
      bPOTooltip: false,

      mnyAllMax: 200000,
      sutAllMax: 3650,
      mnyWellBase: 20000,
      mnyWellMetr: 2,

      mnyWellIssl1: 50,
      mnyWellIssl2: 100,
      mnyWellIssl3: 100,
      mnyWellIssl4: 500,
      mnyWellKernFoto: 100,


      mny2D: 1000,
      mny3D: 1000,
      sut2D: 90,
      sut3D: 100,

      zapasocen: 0,
      sutWellBase: 30.0,
      sutWellMetr: 0.013,
      sutWellIssl: 2.0,
      selIssl1Interval: {
        max: -2100,
        min: -2000
      },
      bViewCellPO: false,
      strTemp: '',
      iCellPO: 0,
      jCellPO: 0,
      bNextStep: false,
      bModeCursorHint: false,
      bModeViewSurf: false,
      bModeViewGeo: false,
      bEditModel: false,
      sEditMode: Properties.EditMode.Borehole,
      drCurSelWell: {},
      drCurSelIssl: {},
      iUserID: '',
      bactionSheet: false,
      subCellX: 0,
      subCellY: 0,
      selCelli: 0,
      selCellj: 0,
      selWellIssl: [],
      selWellIsslorig: [],
      selWellOilPropsIssl: [],
      selWellIsslorig2: [],
      selWellOilPropsIssl2: [],
      selWellIsslorig3: [],
      selWellOilPropsIssl3: [],
      selWellIsslorig4: [],
      selWellOilPropsIssl4: [],
      selWellIssl2: [],
      selWellIssl4: [],
      selWellIssl3: [],
      drawPointMap: [],
      drawPointCell: [],
      arrDisableArea: [],
      dictSurfObjects: [],
      dictMapsTypes: DictMaps,
      dictWellMethodTypes: [],
      megaObj: {},

      textXY: '',


      toeI: 1, toeJ: 1, toeK: 1,
      bAllow: -1,
      strTempFromApi: '',
      txtCursorHint: {
        x: 0,
        y: 0,
        text: '',
        fontSize: 12
      },

      apiloadvisible: false,
      pCurPoint: {
        icell: 0,
        jcell: 0,
        x: 0,
        y: 0,
        name: '',
        label: {
          text: '',
          fontSize: 18,
          fill: '#fff'
        }
      },

      configKonva: {
        width: width,
        height: height
      },
      selIssl: [],
      DictWellIssls: [],
      sImageMode: '',
    }
  },

  async mounted() {
    this.testImg1.src = 'statics/Map1.png'
    this.testImg2.src = 'statics/Map2.png'
    this.testImg3.src = 'statics/Map3.png'
    this.testImg4.src = 'statics/Map4.png'
    this.testImg5.src = 'statics/Map5.png'
  },

  async created() {
    console.log("Token", Vue.cookie.get('oilcase_cookie'))
    this.drawMap()

    await OilcaseApi.GetInfo().then(resp => {
      this.iCurGameStep = resp.teamInfo.gameStep
    })

    await OilcaseApi.GetTypesOfResearch().then(resp => {
      this.typesOfResearches = new Map(resp.map(item => [item.name, item]))
      console.log(this.typesOfResearches)
    })

    OilcaseApi.GetMapObjectOfArrangement().then(resp => {
      resp.forEach(cell => {
        this.drawSubCellMap(cell.id, cell.key, cell.cellX, cell.cellY, cell.subCellX, cell.subCellY, cell.gameStep)
      })
    })

    OilcaseApi.GetSeismic().then(resp => {
      resp.forEach(i => {
          if (i.endCellY === i.startCellY) {
            this.addHorizontalSeismic(this.getMaxIdx(this.arr2DProfileList), i.gameStep, i.endCellY)
          }
          if (i.endCellX === i.startCellX) {
            this.addVerticalSeismic(this.getMaxIdx(this.arr2DProfileList), i.gameStep, i.endCellX)
          }
        }
      )
    })

    OilcaseApi.GetBoreholeExploration().then(resp => {
      resp.forEach(item => {
        let CellX = item.trajectoryPoints[0].x
        let CellY = item.trajectoryPoints[0].y
        let drillDeep = item.trajectoryPoints[1].z
        let name = item.name

        let modelWell = Properties.BoreholeType.Exploration
        let gameStep = item.gameStep
        let toeI, toeK, toeJ

        if (item.trajectoryPoints.length > 2) {
          toeI = item.trajectoryPoints[2].x
          toeK = item.trajectoryPoints[2].y
          toeJ = item.trajectoryPoints[2].z
        }

        let borehole = this.addBorehole(item.id, CellX, CellY, name,
          modelWell, gameStep, drillDeep, toeI, toeJ, toeK)
        this.addResearch(item.logNames, borehole)
      })
    })

    OilcaseApi.GetBoreholeProduction().then(resp => {
      resp.forEach(item => {
        let CellX = item.trajectoryPoints[0].x
        let CellY = item.trajectoryPoints[0].y
        let drillDeep = item.trajectoryPoints[1].z
        let name = item.name
        let modelWell = Properties.BoreholeType.Production
        let gameStep = item.gameStep
        let toeI, toeK, toeJ

        if (item.trajectoryPoints.length > 2) {
          toeI = item.trajectoryPoints[2].x
          toeK = item.trajectoryPoints[2].y
          toeJ = item.trajectoryPoints[2].z
        }
        let status = item.statusId

        this.addBorehole(item.id, CellX, CellY,
          name, modelWell, gameStep,
          drillDeep,
          toeI, toeJ, toeK, item.statusId)
      })
    })

    OilcaseApi.GetMaps().then(resp => {
      resp.forEach(item => {
        this.addMap(item.id, item.name)
      })
    })


  },

  beforeDestroy() {
    EventBus.$off('CaseDataLoaded')
    EventBus.$off('SaveCaseFinished')
  },

  computed: {
    configImg1: function () {
      return {
        image: this.testImg1,
        opacity: 0.7,
        width: width,
        height: height
      }
    },

    configImg2: function () {
      return {
        image: this.testImg2,
        opacity: 0.7,
        width: width,
        height: height
      }
    },

    configImg3: function () {
      return {
        image: this.testImg3,
        opacity: 0.7,
        width: width,
        height: height
      }
    },

    configImg4: function () {
      return {
        image: this.testImg4,
        opacity: 0.7,
        width: width,
        height: height
      }
    },

    configImg5: function () {
      return {
        image: this.testImg5,
        opacity: 0.7,
        width: width,
        height: height
      }
    },

    isCaptain() {
      console.log("UserData", UserData.role)
      return UserData.role === 'Captain'
    },

    getCurZonePO() {
      return this.arrSurfObjList.filter(
        x => x.iCell === this.pCurPoint.icell && x.jCell === this.pCurPoint.jcell
      )
    },

    getCostDaySelectedResearch() {
      return this.selectedResearches.reduce((sum, i) => sum + parseFloat(i.costDay), 0)
    },

    getCostMoneySelectedResearch() {
      return this.selectedResearches.reduce((sum, i) => sum + parseFloat(i.costMoney), 0)
    },

    totalCostMoney() {
      let vm = this
      var wellCo = 0

      this.arrDrillsList.forEach(item => {
        let points = [[item.iCell, item.jCell, 0],
          [item.iCell, item.jCell, item.drilldeep]]
        if (item.toeI !== undefined && item.toeJ !== undefined && item.toeK !== undefined) {
          points.push([item.toeI, item.toeJ, item.toeK])
        } else {
          points.push([item.iCell, item.jCell, item.drilldeep])
        }
        var itemDrilldeep = this.calcDrillDeep(points, item.modelWell)

        wellCo += this.getWellMoneyDrillAll(itemDrilldeep) + this.getCostMoneyResearch(item)
      })

      var prof2DCo = Properties.SeismicCost['seismM'] * this.arr2DProfileList.length

      var sumSurf = this.arrSurfObjList.reduce((pSum, a) => pSum + a.objModel.sZatMoney, 0);
      var sumMaps = this.arrMapList.reduce((pSum, a) => pSum + a.sZatMoney, 0);
      var totalCost = wellCo + prof2DCo + sumSurf + sumMaps

      if (totalCost > this.mnyAllMax) {
        if (this.bAllowNextStepMoney === true) {
          this.bAllowNextStepMoney = false
          this.$q.notify({
            color: 'negative',
            icon: 'warning',
            message: 'Внимание! У вас закончились деньги. Попробуйте сократить количество работ',
            position: 'top',
            timeout: 5000 + 3000
          })
        }
      } else {
        this.bAllowNextStepMoney = true
      }
      return totalCost
    },

    totalCostTime() {
      let vm = this
      var sutItog
      var wellSut = 0

      this.arrDrillsList.forEach(item => {
        let points = [[item.iCell, item.jCell, 0],
          [item.iCell, item.jCell, item.drilldeep]]
        if (item.toeI !== undefined && item.toeJ !== undefined && item.toeK !== undefined) {
          points.push([item.toeI, item.toeJ, item.toeK])
        } else {
          points.push([item.iCell, item.jCell, item.drilldeep])
        }
        var itemDrilldeep = this.calcDrillDeep(points, item.modelWell)
        wellSut += this.getWellDayDrillAll(itemDrilldeep) + this.getCostDayResearch(item)

      })
      var povobSut = 0
      this.arrSurfObjList.forEach(item => {
        povobSut += item.objModel.sZatSut;
      })


      let steps = []
      this.arr2DProfileList.forEach(item => {
        steps.push(item.onGameStep)
      })

      let uniqueSteps = new Set(steps)
      var prof2DSut = uniqueSteps.size * this.sut2D

      let costMaps = 0
      this.arrMapList.forEach(map => {
        costMaps += map.sZatSut
      })

      sutItog = prof2DSut + povobSut + wellSut + costMaps

      if (sutItog > this.sutAllMax) {
        if (this.bAllowNextStepDay === true) {
          this.bAllowNextStepDay = false
          this.$q.notify({
            color: 'negative',
            icon: 'warning',
            message:
              'Внимание! Работы и исследования не будут закончены в срок. Попробуйте сократить их количество',
            position: 'top',
            timeout: 5000 + 3000
          })
        }
      } else {
        this.bAllowNextStepDay = true
      }
      vm.endDate = addToDate(vm.startDate, {days: sutItog});
      return sutItog
    },

    ifDisableAreaLength() {
      return this.arrDisableArea.length > 0
    },

    if2DlistLength() {
      return this.arr2DProfileList.length > 0
    },

    if1DlistLength() {
      return this.arrDrillsList.length > 0
    },

    if1MapLength() {
      return this.arrMapList.length > 0
    },

    if1SurfObjLength() {
      return this.arrSurfObjList.length > 0
    },

    getResearchesOptions() {
      return Array.from(this.typesOfResearches.entries()).map(value => {
        let item = value[1]
        return {
          label: `${item.name} - ${item.description} (дней ${item.costDay}, OilCoin ${item.costMoney}).`,
          value: item
        }
      })
    },

    getWellMethodTypes() {
      return this.dictWellMethodTypes.map(item => {
        return {
          label:
          item.sText,
          value: item.sKey,
          part: item.sKey
        }
      })
    },

    getSurfObjects() {
      return ObjectsOfArrangement.Objects.map(item => {
        return {
          label: `${item.sText} (Стоимость ${item.sZatMoney} OilCoin, ${item.sZatSut} сут.)`,
          value: item.sKey,
          part: item.part
        }
      })
    },

    getMapsObjects() {
      let mapListKeys = this.arrMapList.map(x => x.sKey);
      return this.dictMapsTypes.filter(x => !mapListKeys.includes(x.sKey)).map(item => {
        return {
          label: `${item.sText} (Стоимость ${item.sZatMoney} OilCoin, ${item.sZatSut} сут.)`,
          value: item.sKey
        }
      })
    }
  },

  methods: {
    getDrillPoints() {
      return [
        [this.pCurPoint.icell, this.pCurPoint.jcell, 0],
        [this.pCurPoint.icell, this.pCurPoint.jcell, this.drillDeep],
        [this.toeI, this.toeJ, this.toeK]
      ]
    },

    getSeismicTypeMoney(typeSeismic) {
      return Properties.SeismicCost[typeSeismic]
    },

    getCostDayResearch(borehole) {
      return borehole.researches.reduce((sum, i) => sum + parseFloat(this.typesOfResearches.get(i.name).costDay), 0)
    },

    getCostMoneyResearch(borehole) {
      return borehole.researches.reduce((sum, i) => sum + parseFloat(this.typesOfResearches.get(i.name).costMoney), 0)
    },

    drawSubCellMap(objectId, key, cellX, cellY, subCellX, subCellY, gameStep) {
      let vm = this
      let data = key

      let cXSize = Properties.CellXSize, cYSize = Properties.CellYSize

      let objectOfArrangement = ObjectsOfArrangement.Objects.find(x => x.sKey === data)

      let surfNewObj = ObjectCreator.ObjectOfArrangement(objectId, gameStep, data,
        cellX, cellY,
        subCellX, subCellY,
        this.getXForDraw(cellX), this.getYForDraw(cellY),
        (subCellX - 0.5) * (width / cXSize), (subCellY - 0.5) * (height / cYSize),
        width, height,
        vm.endDate, objectOfArrangement)

      vm.arrSurfObjList.push(surfNewObj)
    },

    hideLayerByName(name) {
      this.$refs.stage.getStage()
        .find(name)[0]
        .hide()
    },

    showLayerByName(name) {
      this.$refs.stage.getStage()
        .find(name)[0]
        .show()
    },

    reDrawLayer(layerNumber) {
      this.$refs.stage.getStage()
        .getLayers()[layerNumber]
        .draw()
    },

    drawMap() {
      let vm = this
      let basePoint = {
        stroke: '#679ce0',
        strokeWidth: 1
      }

      let baseScale = width / Properties.MapXSize;
      for (let n = 1; n <= Properties.MapXSize; n++) {
        vm.drawPointMap.push(Object.assign({}, basePoint, {
          id: vm.drawPointMap.length,
          points: [baseScale * n, 0, baseScale * n, height],
        }))
      }

      baseScale = height / Properties.MapYSize;
      for (let n = 1; n <= Properties.MapYSize; n++) {
        vm.drawPointMap.push(Object.assign({}, basePoint, {
          id: vm.drawPointMap.length,
          points: [0, baseScale * n, width, baseScale * n],
        }))
      }

      baseScale = width / Properties.CellXSize
      for (let n = 1; n <= Properties.CellXSize; n++) {
        vm.drawPointCell.push(Object.assign({}, basePoint, {
          id: vm.drawPointCell.length,
          points: [baseScale * n, 0, baseScale * n, height],
        }))
      }

      baseScale = height / Properties.CellYSize
      for (let n = 1; n <= Properties.CellYSize; n++) {
        vm.drawPointCell.push(Object.assign({}, basePoint, {
          id: vm.drawPointCell.length,
          points: [0, baseScale * n, width, baseScale * n],
        }))
      }
    },

    distanceBetweenTwoPoints(firstPoint, secondPoint) {
      let x = (firstPoint[0] - secondPoint[0]) * 400
      let y = (firstPoint[1] - secondPoint[1]) * 400
      let z = firstPoint[2] - secondPoint[2]
      return Math.sqrt(Math.pow(x, 2) + Math.pow(y, 2) + Math.pow(z, 2))
    },

    calcDrillDeep(points, thisModelWell) {
      return this.distanceBetweenTwoPoints(points[0], points[1])
        + (thisModelWell === Properties.BoreholeType.Production ? 0 : this.distanceBetweenTwoPoints(points[1], points[2]))
    },

    getDrillEndDate(sutWellBase, sutWellDeep) {
      return addToDate(this.endDate, {days: (sutWellBase + sutWellDeep)});
    },

    getWellMoneyDrill(deep) {
      return Math.ceil(Math.abs(Number(deep)) * 3);
    },

    getWellMoneyDrillAll(deep) {
      return this.getWellMoneyDrill(deep) + this.mnyWellBase
    },

    getWellDayDrill(deep) {
      return Math.ceil(Math.abs(Number(deep)) / 90 / 24);
    },

    getWellDayDrillAll(deep) {
      return this.getWellDayDrill(deep) + this.sutWellBase
    },

    addMap(id, mapName) {
      let data = [mapName]
      this.dictMapsTypes.filter(x => data.includes(x.sKey)).forEach(item => {
        item["status"] = 1
        item['idx'] = id
        this.arrMapList.push(item)
        if (item.sPref !== 'fm') {
          this.actionsSheetItems.push(ObjectCreator.SheetItemON(item.sText, () => {
            this.toggleMap(item.sKey)
          }))
        }

        if (this.actionsSheetItems.filter(x => x.type === 'mapoff').length === 0) {
          this.actionsSheetItems.push(ObjectCreator.SheetItemOff(() => {
            this.toggleMap('off')
          }))
        }
      })
    },

    doBuyMap() {
      this.$q
        .dialog(Dialogs.BuyMap(this.getMapsObjects))
        .then(data => {
          data.forEach(item => {
            OilcaseApi.PostMaps(item).then(resp => {
              this.addMap(resp, item)
            })
          })

        }).catch((e) => {
        console.log(e)
      })
    },

    sumZatrIssl(item) {
      return (item.selWellIssl ? item.selWellIssl.length : 0) * this.mnyWellIssl1 +
        (item.selWellIssl2 ? item.selWellIssl2.length : 0) * this.mnyWellIssl2 +
        (item.selWellIssl3 ? item.selWellIssl3.length : 0) * this.mnyWellIssl3 +
        (item.selWellIssl4 ? item.selWellIssl4.length : 0) * this.mnyWellIssl4
    },

    getMaxIdx(array) {
      return (array.length > 0 ? array.reduce((prev, current) => prev.idx > current.idx ? prev : current).idx : 0) + 1
    },

    toggleMap(mapKey) {
      if (mapKey === 'off') {
        this.sImageMode = 'off'
      } else {
        const curmap = this.arrMapList.find(x => x.sKey === mapKey)
        this.sImageMode = curmap.part
      }
    },

    toggleGeo() {
      if (!this.bModeViewGeo) {
        this.actionsSheetItems[1].label = 'включить отображение геологии'
        this.actionsSheetItems[1].icon = 'check_box_outline_blank'
        this.hideLayerByName('#lay3D')
        this.hideLayerByName('#lay2D')
        this.hideLayerByName('#layWells')
      } else {
        this.actionsSheetItems[1].label = 'отключить отображение геологии'
        this.actionsSheetItems[1].icon = 'check_box'
        this.showLayerByName('#lay3D')
        this.showLayerByName('#lay2D')
        this.showLayerByName('#layWells')
      }
      this.bModeViewGeo = !this.bModeViewGeo
    },

    toggleSurface() {
      if (!this.bModeViewSurf) {
        this.actionsSheetItems[2].label = 'включить поверхностное обустройство'
        this.actionsSheetItems[2].icon = 'check_box_outline_blank'
        this.hideLayerByName('#laySurf')
      } else {
        this.actionsSheetItems[2].label = 'отключить поверхностное обустройство'
        this.actionsSheetItems[2].icon = 'check_box'
        this.showLayerByName('#laySurf')
      }
      this.bModeViewSurf = !this.bModeViewSurf
    },

    toggleHint() {
      if (this.bModeCursorHint) {
        this.actionsSheetItems[0].label = 'включить курсор ячейки'
        this.actionsSheetItems[0].icon = 'check_box_outline_blank'
        this.reDrawLayer(4)
      } else {
        this.actionsSheetItems[0].label = 'отключить курсор ячейки'
        this.actionsSheetItems[0].icon = 'check_box'
      }
      this.bModeCursorHint = !this.bModeCursorHint
    },

    doSetWellStatus(param) {
      this.bEditWellStatus = true;
      this.drCurSelWell = param;
      this.drCurSelWell.statusdate = this.endDate;
    },

    SpinnerShow(options) {
      this.$q.loading.show(options)
    },

    makeStep() {
      OilcaseApi.CompleteMove().then(_ => {
        this.addUserActionToLogAsync(UserActions.InventoryClarification(this.iCurGameStep, this.zapasocen))
        this.addUserActionToLogAsync(UserActions.StepFinish(this.iCurGameStep))
        location.reload();
      })
    },

    selectAllResearch() {
      this.selectedResearches = this.checkValueSelectedAllResearch ? Array.from(this.typesOfResearches.values()) : []
    },

    addIssl2Well(item) {
      this.selectedResearches = item.researches.map(item => this.typesOfResearches.get(item.name))

      this.sEditMode = Properties.EditMode.Researches
      this.bAllIssl = false

      this.drCurSelWell = item
      this.selIssl1Interval.min = item.drilldeep
      this.selIssl1Interval.max = this.minDrillDeep
      this.drillDeep = item.drilldeep
      console.log(item)
      this.pCurPoint.icell = item.iCell
      this.pCurPoint.jcell = item.jCell

      this.selWellIssl3 = item.selWellIssl3
      this.bAllIssl = item.researches.length === 9

      this.selIssl = 'issll2dT1'
      this.bEditModel = true
      this.drCurSelIssl = null
    },

    delRecord(item) {
      if (item.name.startsWith('w')) {
        this.$q
          .dialog(Dialogs.AgreeDeleteBorehole(item.name))
          .then(() => {
            var remIndex = this.indexWhere(this.arrDrillsList, arritem => arritem.name === item.name)
            this.arrDrillsList.splice(remIndex, 1)
            this.addUserActionToLogAsync(UserActions.DeleteBorehole(item.name))
            OilcaseApi.DeleteBorehole()
          })
          .catch((e) => {
            console.log(e)
          })
      }
      if (item.name.startsWith('2d')) {
        this.$q
          .dialog(Dialogs.AgreeDeleteSeismic(item.name))
          .then(() => {
            OilcaseApi.DeleteSeismic(item.idx)
            var remIndex = this.indexWhere(this.arr2DProfileList, arritem => arritem.name === item.name)
            this.arr2DProfileList.splice(remIndex, 1)
            this.addUserActionToLogAsync(UserActions.DeleteSeismic(item.name))
            this.reDrawLayer(3)
          })
          .catch((e) => {
            console.log(e)
          })
      }
      if (item.id.startsWith('surf')) {
        this.$q
          .dialog(Dialogs.AgreeDeleteObjectOfArrangement(item.name))
          .then(() => {
            OilcaseApi.DeleteMapObjectOfArrangement(item.idx).then(
              response => {
                if (response === true) {
                  var remIndex = this.indexWhere(this.arrSurfObjList, arritem => arritem.id === item.id)
                  this.arrSurfObjList.splice(remIndex, 1)
                  this.addUserActionToLogAsync(UserActions.DeleteObjectsOfArrangement(item.name))
                  this.reDrawLayer(4)
                }
              }
            )
          }).catch((e) => {
          console.log(e)
        })
      }
    },

    indexWhere(array, conditionFn) {
      const item = array.find(conditionFn)
      return array.indexOf(item)
    },

    handleMouseMove(event) {
      let pointerPos = event.currentTarget.pointerPos;
      this.selCelli = Math.trunc(pointerPos.x / (width / Properties.MapXSize)) + 1
      this.selCellj = Math.trunc(pointerPos.y / (height / Properties.MapYSize)) + 1

      if (this.bModeCursorHint) {
        this.txtCursorHint.x = event.currentTarget.pointerPos.x + (this.selCelli > 23 ? -40 : 0)
        this.txtCursorHint.y = event.currentTarget.pointerPos.y + (this.selCellj < 2 ? 20 : -40)
        this.txtCursorHint.text = `\ni ${this.selCelli}\nj ${this.selCellj}`
      }
    },

    handleMouseMovePO(event) {
      let pointerPos = event.currentTarget.pointerPos;
      this.subCellX = Math.trunc(pointerPos.x / (width / Properties.CellXSize)) + 1
      this.subCellY = Math.trunc(pointerPos.y / (height / Properties.CellYSize)) + 1
      this.textXY = event.currentTarget.pointerPos.x + " " + event.currentTarget.pointerPos.y;
    },

    showPOTooltip(event) {
      let vm = this;
      try {
        const scene = vm.$refs.stage
        vm.cntxmenuposition = {
          left: scene.$el.offsetLeft + event.x + 10 + 'px',
          top: scene.$el.offsetTop + event.y + 30 + 'px'
        }

        const sibl = this.arrSurfObjList.filter(
          e =>
            e.iCell === event.iCell &&
            e.jCell === event.jCell &&
            e.subCellX === event.subCellX &&
            e.subCellY === event.subCellY &&
            e.id !== event.id
        )

        vm.strPOTooltip = sibl.length > 1 ? sibl.map(x => x.description).join(' ') : event.tooltip
        vm.bPOTooltip = true

      } catch (ex) {
        console.log(ex)
      }

    },

    handleStageMouseDownPO(e) {
      this.$q
        .dialog(Dialogs.AddObjectsOfArrangement(this.iCellPO, this.jCellPO, this.subCellX, this.subCellY, this.getSurfObjects))
        .then(data => {
          if (data.length === 0) {
            this.$q.notify(Notifications.SpecifyObjectObjectOfArrangement())
            return
          }

          OilcaseApi.PostMapObjectOfArrangement(data, this.pCurPoint.icell, this.pCurPoint.jcell,
            this.subCellX, this.subCellY).then(resp => {
            console.log()
            this.drawSubCellMap(resp, data, this.pCurPoint.icell, this.pCurPoint.jcell,
              this.subCellX, this.subCellY, this.iCurGameStep)

            this.mapObjects.setCell(this.pCurPoint.icell, this.pCurPoint.jcell)
              .setSubCell(this.subCellX, this.subCellY, data, this.iCurGameStep)
          })

          this.addUserActionToLogAsync(UserActions.AddObjectsOfArrangement(data,
            this.pCurPoint.icell, this.pCurPoint.jcell, this.subCellX, this.subCellY))
        }).catch((e) => {
        console.log(e)
      })
    },

    getXForDraw(CellX) {
      return (CellX - 0.5) * (width / Properties.MapXSize)
    },

    getYForDraw(CellY) {
      return (CellY - 0.5) * (height / Properties.MapYSize)
    },

    async handleStageMouseDown(e) {
      let vm = this
      vm.bViewCellPO = false

      let currentPoint = e.target.nodeType !== 'Stage' ? e.currentTarget.pointerPos : e.target.pointerPos

      let cellX = Math.trunc(currentPoint.x / (width / Properties.MapXSize)) + 1;
      let cellY = Math.trunc(currentPoint.y / (height / Properties.MapYSize)) + 1;

      vm.pCurPoint.name = 'w' + vm.getMaxIdx(vm.arrDrillsList)
      vm.pCurPoint.icell = cellX
      vm.pCurPoint.jcell = cellY

      switch (vm.bsCellMode) {
        case Properties.PurchaseType.VerticalSeismic:
          vm.$q.dialog(Dialogs.AddCrossSectionVertical(vm.sut2D, cellX)).then(_ => {
            let newId2DProfile = vm.getMaxIdx(vm.arr2DProfileList)
            this.addVerticalSeismic(newId2DProfile, this.iCurGameStep, vm.pCurPoint.icell)
            vm.addUserActionToLogAsync(UserActions.AddCrossSectionVertical(newId2DProfile, vm.pCurPoint.icell))
            OilcaseApi.PostSeismic(vm.pCurPoint.icell, 1, vm.pCurPoint.icell, Properties.MapYSize)
            vm.reDrawLayer(3)
          }).catch((e) => {
            console.log(e)
          })
          break;

        case Properties.PurchaseType.HorizontalSeismic:
          vm.$q.dialog(Dialogs.AddCrossSectionHorizontal(vm.sut2D, cellY)).then(_ => {
            let newId2DProfile = vm.getMaxIdx(vm.arr2DProfileList)
            this.addHorizontalSeismic(newId2DProfile, this.iCurGameStep, vm.pCurPoint.jcell)
            vm.addUserActionToLogAsync(UserActions.AddCrossSectionHorizontal(newId2DProfile, vm.pCurPoint.jcell))
            OilcaseApi.PostSeismic(1, vm.pCurPoint.jcell, Properties.MapXSize, vm.pCurPoint.jcell)
            vm.reDrawLayer(3)
          }).catch((e) => {
            console.log(e)
          })
          break;

        case Properties.PurchaseType.ObjectOfArrangement:
          vm.$q.dialog(Dialogs.AgreeToViewObjectsOfArrangement(cellX, cellY)).then(_ => {
            vm.iCellPO = cellX
            vm.jCellPO = cellY
            vm.bViewCellPO = true
          }).catch((e) => {
            console.log(e)
          })
          break;

        case Properties.PurchaseType.Well:
          if (vm.arrDrillsList.filter(x => x.iCell === cellX && x.jCell === cellY).length === 0) {
            if (vm.arrSurfObjList.filter(x => x.iCell === cellX && x.jCell === cellY && x.profType === 'soKust').length === 0) {
              vm.$q.dialog(Dialogs.WarningAboutAddingWell()).catch((e) => {
                console.log(e)
              })
            } else {
              vm.bEditModel = true
              vm.sEditMode = Properties.EditMode.Borehole
              vm.selIssl1Interval.min = -2000
              vm.selIssl1Interval.max = -2100
              vm.toeI = cellX
              vm.toeJ = cellY
              vm.toeK = -1 * vm.maxDrillDeep
              vm.drillDeep = -2100
            }
          }
      }
    },

    getValueInInterval(value, minValue, maxValue) {
      return minValue <= value && value <= maxValue ? value
        : (value < minValue ? minValue : maxValue)
    },

    //что-то совсем не понятное
    async updateWellStatus(_) {
      let vm = this
      if (vm.drCurSelWell.statusdate === undefined) {
        this.$q.dialog(Dialogs.SpecifyDateOfStatus());
        return false;
      }
      this.$q.dialog(Dialogs.SaveChanges())
        .then(() => {
          if (vm.drCurSelWell.arrStatus === undefined) {
            vm.drCurSelWell.arrStatus = []
          }
          var statusText = vm.getWellMethodTypes.find(x => x.value === vm.drCurSelWell.wellstatus);

          vm.drCurSelWell.arrStatus.push({
            statusID: vm.drCurSelWell.wellstatus,
            statusText: statusText.label,
            statusDate: vm.drCurSelWell.statusdate
          })

          var thsurf = this.arrSurfObjList.find(x => x.iCell === vm.drCurSelWell.iCell
            && x.jCell === vm.drCurSelWell.jCell)

          if (thsurf.lblrectPO.tooltipW === null) {
            thsurf.lblrectPO.tooltipW = thsurf.lblrectPO.tooltip;
          }

          thsurf.lblrectPO.tooltip = thsurf.lblrectPO.tooltipW + '\r\n' + statusText.label;

          this.megaObj.arrSurfObjList = JSON.stringify(this.arrSurfObjList)

          var cuWellIdx = vm.arrDrillsList.findIndex(x => x.id === vm.drCurSelWell.id)
          vm.arrDrillsList[cuWellIdx] = vm.drCurSelWell;
          vm.drCurSelWell.wellstatusText = statusText.label;
          this.$q.notify({type: 'positive', message: 'Статус скважины добавлен'})
          this.bEditWellStatus = false;
        })
        .catch(() => {
          this.$q.notify('отменено')
        })
    },

    async updateRecord(CellX, CellY) {
      if (this.toeI > Properties.MapXSize || this.toeJ > Properties.MapYSize) {
        this.$q.dialog(Dialogs.IncorrectParametersHorizontalTrunk());
        return false;
      }
      this.$q
        .dialog(Dialogs.SaveChanges()).then(() => {
        this.apiloadvisible = true
        switch (this.sEditMode) {
          case Properties.EditMode.Borehole:
            let trajectoryPoints = [ObjectCreator.TrajectoryPoint(CellX, CellY, this.minDrillDeep),
              ObjectCreator.TrajectoryPoint(CellX, CellY, this.drillDeep)]
            switch (this.modelWell) {
              case Properties.BoreholeType.Exploration:
                OilcaseApi.PostBoreholeExploration(trajectoryPoints).then(resp => {
                  this.addBorehole(resp, this.pCurPoint.icell, this.pCurPoint.jcell,
                    this.pCurPoint.name, this.modelWell, this.iCurGameStep,
                    this.drillDeep,
                    this.toeI, this.toeJ, this.toeK)
                  this.addUserActionToLogAsync(UserActions.AddBorehole(resp, CellX, CellY))
                })
                break;
              case Properties.BoreholeType.Production:
                trajectoryPoints.push(ObjectCreator.TrajectoryPoint(this.toeI, this.toeJ, this.toeK))
                OilcaseApi.PostBoreholeProduction(trajectoryPoints).then(resp => {
                  this.addBorehole(resp, this.pCurPoint.icell, this.pCurPoint.jcell,
                    this.pCurPoint.name, this.modelWell, this.iCurGameStep,
                    this.drillDeep, this.toeI, this.toeJ, this.toeK)
                  this.addUserActionToLogAsync(UserActions.AddBorehole(resp, CellX, CellY))
                })
            }

            break;
          case Properties.EditMode.Researches:
            let borehole = this.arrDrillsList.filter(item => {
              return item.jCell === CellX & item.jCell === CellY
            })[0]
            console.log(CellX, CellY)
            console.log(this.arrDrillsList)
            console.log(borehole)
            OilcaseApi.PatchBoreholeExploration(borehole.idx, Array.from(this.selectedResearches, x => x.name))
            this.addResearch(this.selectedResearches, this.drCurSelWell)
            this.addUserActionToLogAsync(UserActions.AddResearches(this.selectedResearches, CellX, CellY))
        }
        this.apiloadvisible = false
        this.bEditModel = false
      })
        .catch(e => {
          console.log(e)
          this.$q.notify('отменено')
        })
    },

    addBorehole(boreholeId, CellX, CellY, name, modelWell, gameStep, drillDeep, toeI, toeJ, toeK, boreholeStatus) {
      let borehole = ObjectCreator.Borehole(
        boreholeId, this.getXForDraw(CellX), this.getYForDraw(CellY),
        CellX, CellY,
        name, gameStep,
        this.drillDeep, modelWell,
        toeI, toeJ, toeK, boreholeStatus)
      this.arrDrillsList.push(borehole)
      return borehole
    },

    addResearch(arrayOfResearch, borehole) {
      borehole.researches = []
      arrayOfResearch.forEach(item => {
        borehole.researches.push({
          name: item.name,
          key: `${item.name}-${borehole.idx}`,
          text: `${item.name}-${this.typesOfResearches.get(item.name).description}`,
          onGameStep: parseInt(borehole.onGameStep)
        })
      })
    },

    addHorizontalSeismic(id, gameStep, cellY) {
      this.arr2DProfileList.push(ObjectCreator.Profile2DHorizontal(id, gameStep, cellY, width, this.getYForDraw(cellY)))
    },

    addVerticalSeismic(id, gameStep, cellX) {
      this.arr2DProfileList.push(ObjectCreator.HorizontalSeismic(id, gameStep, cellX, height, this.getXForDraw(cellX)))
    },

    async cancelRecord() {
      this.bEditModel = false
    },

    async addUserActionToLogAsync(action) {
      await OilcaseApi.DoLogCase(this.iUserID, action.ActionType, action.ActionData)
    },

    // checkAdIssl(value, selWellOilPropsIssl, selWellIssl, selWellIsslorig) {
    //   const isFounded = selWellOilPropsIssl.every(ai => selWellIssl.includes(ai))
    //   if (isFounded) {
    //     selWellIsslorig = selWellIssl
    //   } else
    //     this.$q
    //       .dialog(Dialogs.ResearchNotCanceled()).then(() => {
    //       selWellIssl = selWellIsslorig
    //     }).catch((e) => {
    //       console.log(e)
    //     })
    // },
    //
    // checkAdIssl1(value) {
    //   this.checkAdIssl(value, this.selWellOilPropsIssl, this.selWellIssl, this.selWellIsslorig)
    // },
    //
    // checkAdIssl2(value) {
    //   this.checkAdIssl(value, this.selWellOilPropsIssl2, this.selWellIssl2, this.selWellIsslorig2)
    // },
    //
    // checkAdIssl3(value) {
    //   this.checkAdIssl(value, this.selWellOilPropsIssl3, this.selWellIssl3, this.selWellIsslorig3)
    // },
    //
    // checkAdIssl4(value) {
    //   this.checkAdIssl(value, this.selWellOilPropsIssl4, this.selWellIssl4, this.selWellIsslorig4)
    // },

  },

  watch: {
    toeI: function (newToeI) {
      this.toeI = this.getValueInInterval(newToeI, 1, Properties.MapXSize)
    },

    toeJ: function (newToeJ) {
      this.toeJ = this.getValueInInterval(newToeJ, 1, Properties.MapYSize)
    },

    toeK: function (newToeK) {
      this.toeK = this.getValueInInterval(newToeK, this.maxDrillDeep, this.minDrillDeep)
    },
  },

  filters: {
    formatFinance: function (str) {
      return `${!str ? 0 : str.toLocaleString('ru-RU')} OilCoin`
    },

    formatSut: function (str) {
      return `${!str ? 0 : Math.ceil(Number((str)))} сут.`
    },

    dateDate: function (str) {
      return !str ? "(n/a)" : moment(String(new Date(str))).format("DD.MM.YYYY");
    }
  }
}
</script>

<style lang="stylus"></style>
