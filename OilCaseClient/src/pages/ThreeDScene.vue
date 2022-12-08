<template>
  <q-page padding class="row justify">
    <q-inner-loading :visible="loadProc">
      <q-spinner-gears size="50px" color="primary"></q-spinner-gears>
    </q-inner-loading>
    <div class="doc-container" style="margin-top:-20px !important;width:95%">
      <!--      <div v-show="dCommonZLevel !== 0" class="row q-pt-md">-->
      <div class="row q-pt-md">
        <div class="col-9">
          <q-btn class="q-mb-lg text-center" color="primary" size="sm" label="Объекты геологии" @click="clickCheck()"/>
          <q-btn class="q-mb-lg q-ml-lg" size="sm" color="teal" label="Объекты поверхности" @click="doLoadSurf()"/>

          <div ref="canvasContainer"></div>
        </div>
        <div class="col-3">
          <b> Отображение объектов</b>
          <div class="q-mt-md">Скважины</div>
          <q-checkbox class="q-ml-md" v-for="item in this.arrDrillsList" :key="`borehole_${item.id}`"
                      v-model="item.scview"
                      :label="item.name" @input="checkDrill($event)"/>

          <!--          <div class="q-mt-md">2D Профили</div>-->
          <!--          <q-checkbox class="q-ml-md " v-for="item in this.arr2DProfileList" :key="`${item.id}`" v-model="item.scview"-->
          <!--                      :label="item.name" @input="checkDrill($event)"/>-->
          <!--          <div class="q-mt-md">3D Сейсмика</div>-->
          <!--          <q-checkbox class="q-ml-md " v-for="item in this.arr3DProfileList" :key="`${item.id}`" v-model="item.scview"-->
          <!--                      :label="item.name" @input="checkDrill($event)"/>-->
          <div class="q-mt-md">Обустройство</div>
          <q-checkbox class="q-ml-md " v-for="item in this.arrSurfObjList" :key="`object_${item.id}`"
                      v-model="item.scview"
                      :label="item.stext" @input="checkDrill($event)"/>
          <div v-if="arrMapList.length > 0">
            <div class="q-mt-md">Карты</div>
            <q-list dense v-for="item in this.arrMapList" :key="`${item.sKey}`">
              <q-item>
                <q-item-main>
                  <q-radio v-model="curmap" :val="item.sImageModel" :label="item.sText"
                           @input="doMapShowHide(item.sImageModel)"/>
                </q-item-main>
              </q-item>
            </q-list>
          </div>

          <q-btn v-if="false" class="q-mb-lg q-ml-lg" size="sm" color="teal" label="Check" @click="do3DLoad()"/>
          <br/>
          {{ strClickedObj }}
        </div>
      </div>
      <!--      <div v-show="dCommonZLevel === 0" class="q-mt-lg text-center">-->
      <!--        <h5>Нет данных для отображения</h5>-->
      <!--      </div>-->
    </div>
  </q-page>
</template>

<style>
</style>

<script>
import EventBus from 'src/event-bus'
import {OutSideMixin} from 'src/scriptslibs/scenescripts.js'
import OilCaseApi from 'src/api/OilCaseApi'
import * as THREE from 'three'
import {GLTFLoader} from 'three/examples/jsm/loaders/GLTFLoader.js'
import {ObjectsOfArrangement} from "src/api/Properties";
import {itemPoint} from "v-charts/lib/constants";

let vm = {}

export default {
  name: 'ThreeDScene',
  mixins: [OutSideMixin],
  container: null,
  scene: null,
  aspectRatio: null,
  camera: null,
  renderer: null,
  data() {
    return {
      gridCount: 25,
      gridSize: 200,
      loadProc: false,
      drSurfCount: 1,
      strClickedObj: '',
      arrCompasPoints: [],
      dSurfTop: 100,
      scalekoef: 200,
      raycaster: null,
      projector: null,
      pickedObject: null,
      pickedObjectSavedColor: 0,
      dictMapsTypes: [],
      pickableMeshes: [],
      arrMapList: [],
      megaObj: {},
      skipKeys: ['soRoad', 'soElLine', 'soKust', 'soTube'],
      curmap: '',
      dictObjects: ObjectsOfArrangement.Dict,
      lithologicalMinZ: null,
      lithologicalMaxZ: null,
    }
  },
  created() {
    EventBus.$on('CaseDataLoaded', todoItem => {
      // this.doLoadCaseData()
    })
  },
  async mounted() {
    // this.doLoadCaseData()
    await OilCaseApi.GetInfo().then(resp => {
      this.lithologicalMinZ = resp.teamInfo.lithologicalModelInfo.minZ
      this.lithologicalMaxZ = resp.teamInfo.lithologicalModelInfo.maxZ
    })


    OilCaseApi.GetBoreholeProduction().then(resp => {
      console.log(resp)
      resp.forEach(item => {
        let toeIndex = item.trajectoryPoints.length > 2 ? 2 : 1
        this.arrDrillsList.push({
          id: item.id,
          iCell: item.trajectoryPoints[0].x,
          jCell: item.trajectoryPoints[0].y,
          drilldeep: item.trajectoryPoints[1].z,
          toeI: item.trajectoryPoints[toeIndex].x,
          toeJ: item.trajectoryPoints[toeIndex].y,
          toeK: item.trajectoryPoints[toeIndex].z,
          name: item.name,
          scview: true
        })
      })
      console.log('this.arrDrillsList')
      console.log(this.arrDrillsList)
    })

    OilCaseApi.GetBoreholeExploration().then(resp => {
      console.log(resp)
      resp.forEach(item => {
        this.arrDrillsList.push({
          id: item.id,
          iCell: item.trajectoryPoints[0].x,
          jCell: item.trajectoryPoints[0].y,
          drilldeep: item.trajectoryPoints[1].z,
          name: item.name,
          scview: true
        })
      })
      console.log('this.arrDrillsList')
      console.log(this.arrDrillsList)
    })

    OilCaseApi.GetMapObjectOfArrangement().then(resp => {
      console.log(resp)
      resp.forEach(item => {
        let dictItem = this.dictObjects[item.key]
        this.arrSurfObjList.push({
          id: item.id,
          selCelliPo: item.subCellX,
          selCelljPo: item.subCellY,
          iCell: item.cellX,
          jCell: item.cellY,
          name: dictItem.sKey,
          description: dictItem.sText,
          scview: true,
          stext: `${dictItem.sText}. ${item.cellX}:${item.cellY}`,
          objModel: {
            sModel: dictItem.sKey,
            sKey: dictItem.sKey,
            color: dictItem.color
          }
        })
      })
    })


    this.arrMapList = []
    // if (this.megaObj.arrCommon.buymap !== '') {
    //   this.dictMapsTypes = spravDictions.dictMapsTypes
    //   const maparr = this.megaObj.arrCommon.buymap.split(',')
    //   if (maparr.length > 0) {
    //     const df = this.dictMapsTypes.filter(x => maparr.includes(x.sKey))
    //     df.forEach(item => {
    //       if (item.sPref !== 'fm') {
    //         this.arrMapList.push(item)
    //       }
    //     })
    //     this.arrMapList.push({
    //       sKey: 'nomap',
    //       sText: 'Без карты',
    //       sImageModel: 'nomap'
    //     })
    //   }
    // }

    this.container = this.$refs.canvasContainer
    this.prepareScene()
    this.raycaster = new THREE.Raycaster()
    this.pickedObject = null
  },
  computed: {},
  methods: {

    async doMapShowHide(sobjname) {
      try {
        this.arrMapList.forEach(el => {
          let mapobj = this.scene.getObjectByName(el.sImageModel)
          if (mapobj !== null) {
            mapobj.visible = el.sImageModel === sobjname && sobjname !== 'nomap';
          }
        })
      } catch (error) {

      }
    },

    async doLoadSurf() {
      var gridHelper = new THREE.GridHelper(this.gridSize, this.gridCount * 3, 0x888888, '#f0d6ae')
      gridHelper.position.z = this.dSurfTop
      gridHelper.rotation.x = -Math.PI / 2
      this.scene.add(gridHelper)
      var cellSizePo = this.gridSize / this.gridCount / 3

      this.arrMapList.forEach(el => {
        try {
          var txloader = new THREE.TextureLoader()
          var material = new THREE.MeshLambertMaterial({
            map: txloader.load('statics/' + el.sImageModel),
            transparent: true
          })
          material.opacity = 0.8
          var geometry = new THREE.PlaneGeometry(this.gridSize, this.gridSize)
          var mesh = new THREE.Mesh(geometry, material)
          mesh.position.z = this.dSurfTop
          mesh.name = el.sImageModel
          mesh.visible = false
          this.scene.add(mesh)
        } catch (error) {
        }
      })

      this.arrSurfObjList.forEach(el => {
        try {
          console.log(el.objModel.sKey)
          if (!this.skipKeys.includes(el.objModel.sKey)) {
            const strModel = el.objModel.sModel.substring(2) + '.gltf'
            // console.log(strModel, el.objModel.sPref, el.objModel)

            const loader = new GLTFLoader()
            let gridHalf = this.gridSize / 2

            let step = this.gridSize / (this.gridCount * 3)
            let xOffset = (-gridHalf) + ((el.iCell - 1) * 3 + el.selCelliPo - 1) * step
            let yOffset = (gridHalf - step) - ((el.jCell - 1) * 3 + el.selCelljPo - 1) * step

            this.scalekoef = this.gridSize / 4
            loader.load(
              'models3d/' + strModel,
              gltf => {
                this.surfmodel = gltf.scene
                this.surfmodel.scale.set(
                  this.scalekoef,
                  this.scalekoef,
                  this.scalekoef
                )
                this.surfmodel.rotation.set(Math.PI / 2, 0, 0)
                this.surfmodel.position.set(xOffset, yOffset, this.dSurfTop)
                this.surfmodel.name = el.name
                this.surfmodel.userData = el.stext
                const group = new THREE.Object3D()
                group.add(this.surfmodel)
                this.scene.add(group)
              },
              undefined,
              function (err) {
                console.error(err)
              }
            )
          } else {
            let gridHalf = this.gridSize / 2
            let step = this.gridSize / (this.gridCount * 3)
            let xOffset = (-gridHalf + step / 2) + ((el.iCell - 1) * 3 + el.selCelliPo - 1) * step
            let yOffset = (gridHalf - step / 2) - ((el.jCell - 1) * 3 + el.selCelljPo - 1) * step

            if (el.objModel.sKey === 'soTube') {
              let geometry = new THREE.CylinderBufferGeometry(cellSizePo * 0.2, cellSizePo * 0.2, 1)
              let material = new THREE.MeshBasicMaterial({color: el.objModel.color})
              let cylinder = new THREE.Mesh(geometry, material)
              let sibl = this.arrSurfObjList.filter(x => x.objModel.sKey === el.objModel.sKey
                && x.iCell === el.iCell && x.jCell === el.jCell && x.id !== el.id)
              if (sibl.length > 0) {

                if (sibl.filter(x => x.selCelljPo === el.selCelljPo && x.id !== el.id).length > 0) {
                  cylinder.rotation.set(0, 0, Math.PI / 2)
                }
              }
              cylinder.position.set(xOffset, yOffset, this.dSurfTop + 1)
              this.scene.add(cylinder)
            } else if (el.objModel.sKey === 'soKust') {
              console.log('Create kust')
              let geometry = new THREE.BoxBufferGeometry(cellSizePo * 1, cellSizePo * 1, 0.2)
              let material = new THREE.MeshBasicMaterial({color: el.objModel.color})
              let cylinder = new THREE.Mesh(geometry, material)
              cylinder.rotation.set(0, 0, 0)
              cylinder.position.set(xOffset, yOffset, this.dSurfTop)

              this.scene.add(cylinder)
            } else if (el.objModel.sKey === 'soElLine') {
              let geometry = new THREE.CylinderBufferGeometry(cellSizePo * 0.2, cellSizePo * 0.2, 1)
              let material = new THREE.MeshBasicMaterial({color: el.objModel.color})
              let cylinder = new THREE.Mesh(geometry, material)
              let sibl = this.arrSurfObjList.filter(x => x.objModel.sKey === el.objModel.sKey
                && x.iCell === el.iCell && x.jCell === el.jCell && x.id !== el.id)
              if (sibl.length > 0) {
                if (sibl.filter(x => x.selCelljPo === el.selCelljPo && x.id !== el.id).length > 0) {
                  cylinder.rotation.set(0, 0, Math.PI / 2)
                }
              }
              cylinder.position.set(xOffset, yOffset, this.dSurfTop + 3)
              this.scene.add(cylinder)

            } else if (el.objModel.sKey === 'soRoad') {
              let geometry = new THREE.BoxBufferGeometry(cellSizePo * 1, cellSizePo * 1, 0.1)
              let material = new THREE.MeshBasicMaterial({color: el.objModel.color})
              let cylinder = new THREE.Mesh(geometry, material)
              cylinder.rotation.set(0, 0, 0)
              cylinder.position.set(xOffset, yOffset, this.dSurfTop)
              this.scene.add(cylinder)

            }
          }
        } catch (error) {
          console.error(error)
        }
      })

      this.arrDrillsList.forEach(el => {
        if (el.scview) {
          const loader = new GLTFLoader()
          let gridHalf = this.gridSize / 2
          let step = this.gridSize / (this.gridCount * 3)
          let xOffset = (-gridHalf + step) + (el.iCell - 1) * 3 * step
          let yOffset = (gridHalf - (step * 2)) - (el.jCell - 1) * 3 * step


          this.scalekoef = this.gridSize / 4
          loader.load('models3d/Oiler.gltf',
            gltf => {
              this.surfmodel = gltf.scene
              this.surfmodel.scale.set(
                this.scalekoef,
                this.scalekoef,
                this.scalekoef
              )
              this.surfmodel.rotation.set(Math.PI / 2, 0, 0)
              this.surfmodel.position.set(xOffset, yOffset, this.dSurfTop)
              this.surfmodel.name = el.name
              this.surfmodel.userData = el.name + ' Скважина'

              const group = new THREE.Object3D()
              group.add(this.surfmodel)
              this.scene.add(group)
            },
            undefined,
            function (err) {
              console.error(err)
            }
          )
        }
      })

    },

    async checkDrill(value) {
      try {
        this.$forceUpdate()
      } catch (error) {
        console.log(error)
      }
    },

    async clickCheck() {
      try {
        vm = this
        this.loadProc = true
        let susername = this.megaObj.userid || -1

        if (this.scene.children.length > 6) {
          while (this.scene.children.length > 6) {
            this.scene.remove(this.scene.children[6])
          }
          this.drSurfCount += 1
        }

        let radius = 2
        let height = 18.0
        let radialSegments = 21
        let geometry = new THREE.ConeBufferGeometry(
          radius,
          height,
          radialSegments
        )

        let tumaterial = new THREE.MeshBasicMaterial({color: 0x0000ff})
        let plane = new THREE.Mesh(geometry, tumaterial)
        plane.position.set(105, 5, 0)
        this.scene.add(plane)

        this.arrDrillsList.forEach(element => {
          if (element.scview) {
            this.buildWell5(
              element.iCell,
              element.jCell,
              element.idx * 20,
              element.drilldeep,
              element,
              this.lithologicalMinZ,
              this.lithologicalMaxZ,
            )
          }
        })

        this.arr2DProfileList.forEach(element => {
          if (element.scview) {
            element.seismType.forEach(sel => {
              this.build2DProfileSeism(
                element.iCell,
                element.jCell,
                element.profType,
                sel,
                susername
              )
            })
          }
        })

        this.loadProc = false
      } catch (error) {
        console.log(error)
      }
    },

    onMouseClick(event) {
      let mouse = {x: 0, y: 0}

      mouse.x = (event.offsetX / this.container.clientWidth) * 2 - 1
      mouse.y = -(event.offsetY / this.container.clientHeight) * 2 + 1

      this.raycaster.setFromCamera(mouse, this.camera)

      let intersects = this.raycaster.intersectObjects(
        this.scene.children, true
      )
      if (intersects.length > 0) {
        this.strClickedObj = ''
        let obj = intersects[0].object
        obj.traverseAncestors(a => {
          if (a.userData.length > 0) this.strClickedObj = a.userData
        })
      }
    }
  },
  components: {},
  beforeDestroy() {
    EventBus.$off('CaseDataLoaded')
  }
}
</script>
