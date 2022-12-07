const {MapObjectOfArrangement} = require("./MapObjectOfArrangement");
const MapSize = 25
const CellSize = 3

const SeismicCost = {
  'seismR': 5000,
  'seismM': 1000,
  'seismT': 35000
}

const EditMode = {
  Borehole: "Borehole",
  Researches: "Researches"
}

const BoreholeStatus = {
  Work: "Work",
}

const BoreholeType = {
  Production: "Production", //Эксплуатационная скважина expl
  Exploration: "Exploration" //Разведывательная скважина razv
}

const PurchaseType = {
    Well: "Well",
    HorizontalSeismic: "HorizontalSeismic",
    VerticalSeismic: "VerticalSeismic",
    ObjectOfArrangement: "ObjectOfArrangement",
}


const MinDrillDeep = -2000
const MaxDrillDeep = -2100

const StartDate = new Date("2025-01-01")
const EndDate = new Date("2025-01-01")

module.exports = {
  Properties: {
    MapXSize: MapSize,
    MapYSize: MapSize,
    CellXSize: CellSize,
    CellYSize: CellSize,
    SeismicCost: SeismicCost,
    MinDrillDeep: MinDrillDeep,
    MaxDrillDeep: MaxDrillDeep,
    StartDate: StartDate,
    EndDate: EndDate,
    EditMode: EditMode,
    BoreholeType: BoreholeType,
    BoreholeStatus: BoreholeStatus,
    PurchaseType: PurchaseType
  },
  ObjectsOfArrangement:{
    Objects: [
      {
        sKey: 'soTube',
        sZatSut: 30,
        sZatMoney: 100,
        sText: 'Трубопровод',
        sPref: 't',
        color: '#7cc783',
        part: 2
      },
      {
        sKey: 'soElLine',
        sZatSut: 30,
        sZatMoney: 100,
        sText: 'Линия Электропередач',
        sPref: 'le',
        color: '#86bcde',
        part: 2
      },
      {
        sKey: 'soRoad',
        sZatSut: 30,
        sZatMoney: 100,
        sText: 'Дорожное полотно',
        sPref: 'r',
        color: '#545452',
        part: 2
      },
      {
        sKey: 'soKust',
        sZatSut: 30,
        sZatMoney: 100,
        sText: 'Кустовая площадка',
        sPref: 'kp',
        color: '#e8d08d',
        part: 3
      },
      {
        sKey: 'soGTES',
        sZatSut: 30,
        sZatMoney: 1500,
        sText: 'ГТЭС (Газотурбинная Электростанция)',
        sPref: 'gtes',
        sModel: 'GTES.gltf',
        part: 4
      },
      {
        sKey: 'soAGZU',
        sZatSut: 30,
        sZatMoney: 1000,
        sText: 'АГЗУ (Автоматизированная Замерная Установка)',
        sPref: 'agzu',
        sModel: 'AGZU.gltf',
        part: 4
      },
      {
        sKey: 'soDNS',
        sZatSut: 30,
        sZatMoney: 1400,
        sText: 'ДНС (Дожимная Насосная Станция)',
        sPref: 'dns',
        sModel: 'NPS.gltf',
        part: 4
      },
      {
        sKey: 'soNKS',
        sZatSut: 30,
        sZatMoney: 1400,
        sText: 'НКС (Насосно-Компрессорная Станция)',
        sPref: 'nks',
        sModel: 'NS.gltf',
        part: 4
      },
      {
        sKey: 'soUPSV',
        sZatSut: 30,
        sZatMoney: 2200,
        sText: 'Мини УПСВ (Установка Предварительного Сброса Воды)',
        sPref: 'upsv',
        sModel: 'UPSV.gltf',
        part: 4
      },
      {
        sKey: 'soSEP2f',
        sZatSut: 30,
        sZatMoney: 1800,
        sText: 'Сепаратор 2х фазный',
        sPref: 'sep2f',
        sModel: 'SEPAR2ph.gltf',
        part: 4
      },
      {
        sKey: 'soSEP3f',
        sZatSut: 30,
        sZatMoney: 2200,
        sText: 'Сепаратор 3х фазный',
        sPref: 'sep3f',
        sModel: 'SEPAR3ph.gltf',
        part: 4
      },
      {
        sKey: 'soUPN',
        sZatSut: 30,
        sZatMoney: 3500,
        sText: 'Мини УПН (Установка Подготовки Нефти)',
        sPref: 'upn',
        sModel: 'UPN.gltf',
        part: 4
      },
      {
        sKey: 'soUKPG',
        sZatSut: 30,
        sZatMoney: 3500,
        sText: 'Мини УКПГ (Установка Комплексной Подготовки Газа)',
        sPref: 'ukpg',
        sModel: 'UKPG.gltf',
        part: 4
      },
      {
        sKey: 'soCPS',
        sZatSut: 30,
        sZatMoney: 2000,
        sText: 'Мини ЦПС (Центральный Пункт Сбора)',
        sPref: 'cps',
        sModel: 'CPS.gltf',
        part: 4
      },
      {
        sKey: 'soNPZ',
        sZatSut: 30,
        sZatMoney: 5000,
        sText: 'Мини НПЗ (Нефтеперерабатывающий Завод)',
        sPref: 'npz',
        sModel: 'NPZ.gltf',
        part: 4
      },
      {
        sKey: 'soGPZ',
        sZatSut: 30,
        sZatMoney: 5000,
        sText: 'Мини ГПЗ (Газоперерабатывающий Завод)',
        sPref: 'gpz',
        sModel: 'GPZ.gltf',
        part: 4
      },
      {
        sKey: 'soUPV',
        sZatSut: 30,
        sZatMoney: 2200,
        sText: 'Мини УПВ (Установка Подготовки Пластовой Воды)',
        sPref: 'upv',
        sModel: 'UPV.gltf',
        part: 4
      },
      {
        sKey: 'soFakel',
        sZatSut: 30,
        sZatMoney: 500,
        sText: 'Факел',
        sPref: 'fakel',
        sModel: 'FAKEL.gltf',
        part: 5
      },
      {
        sKey: 'soTO',
        sZatSut: 30,
        sZatMoney: 3000,
        sText: 'Мини Терминал отгрузки',
        sPref: 'to',
        sModel: 'Terminal.gltf',
        part: 5
      },
      {
        sKey: 'soFPSO',
        sZatSut: 30,
        sZatMoney: 4000,
        sText: 'FPSO (Floating Production Storage and Offloading)',
        sPref: 'fpso',
        sModel: 'FPSO.gltf',
        part: 5
      },
      {
        sKey: 'soTanker',
        sZatSut: 30,
        sZatMoney: 3000,
        sText: 'Мини танкер',
        sPref: 'tanker',
        sModel: 'Tanker.gltf',
        part: 5
      },
      {
        sKey: 'soHelic',
        sZatSut: 30,
        sZatMoney: 1200,
        sText: 'Вертолетная площадка',
        sPref: 'helic',
        part: 5
      },
      {
        sKey: 'soLM',
        sZatSut: 30,
        sZatMoney: 1200,
        sText: 'Жилой блок',
        sPref: 'lm',
        sModel: 'House.gltf',
        part: 5
      },
      {
        sKey: 'soTU',
        sZatSut: 30,
        sZatMoney: 1200,
        sText: 'Транспортный цех',
        sPref: 'tu',
        sModel: 'TD.gltf',
        part: 5
      },
    ]
  }
};
