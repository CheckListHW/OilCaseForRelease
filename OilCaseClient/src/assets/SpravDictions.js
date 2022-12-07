export default {
  dictWellMethodTypes: [
    {
      sKey: 'wt1',
      sText: 'Добывающая',
      mode: 0
    },
    {
      sKey: 'wt2',
      sText: 'Нагнетательная',
      mode: 0
    },
    {
      sKey: 'wt3',
      sText: 'Консервация',
      mode: 0
    },
    {
      sKey: 'wt4',
      sText: 'Вывести из консервации в добычу',
      mode: 0
    }, {
      sKey: 'wt5',
      sText: 'Вывести из консервации в нагнетание',
      mode: 0
    },
    {
      sKey: 'wt6',
      sText: 'Финальная консервация',
      mode: 0
    }
  ],

  dictMapsTypes: [
    {
      sKey: 'rm',
      sZatSut: 30,
      sZatMoney: 500,
      sText: 'Рельеф местности',
      sPref: 'm',
      part: 'map1',
      sImageModel: 'map1.png',
      mode: 0,
      status: 0,
    },
    {
      sKey: 'cv',
      sZatSut: 30,
      sZatMoney: 750,
      sText: 'Общий обзор местности',
      sPref: 'm',
      part: 'map2',
      sImageModel: 'map2.png',
      mode: 0,
      status: 0,

    },
    {
      sKey: 'gd',
      sZatSut: 30,
      sZatMoney: 1000,
      sText: 'Прочность грунта',
      sPref: 'm',
      part: 'map3',
      sImageModel: 'map3.png',
      mode: 0,
      status: 0,

    },
    {
      sKey: 'bd',
      sZatSut: 30,
      sZatMoney: 1000,
      sText: 'Карта усложнений строительства',
      sPref: 'm',
      part: 'map4',
      sImageModel: 'map4.png',
      mode: 0,
      status: 0,
    },
    {
      sKey: 'gmg',
      sZatSut: 30,
      sZatMoney: 1000,
      sText: 'Гравиметрическая/ магнитная/геохимическая съемка',
      sPref: 'm',
      part: 'map5',
      sImageModel: 'map5.png',
      mode: 0,
      status: 0,
    },
    {
      sKey: 'fmap1',
      sZatSut: 0,
      sZatMoney: 20000,
      sText: 'Право пользования лицензионным участком А на 10 лет',
      sPref: 'fm',
      part: 'fmap1',
      sImageModel: '',
      mode: 0,
      status: 0,
    },
    {
      sKey: 'fmap2',
      sZatSut: 0,
      sZatMoney: 10000,
      sText: 'Право пользования лицензионным участком Б на 10 лет',
      sPref: 'fm',
      part: 'fmap2',
      sImageModel: '',
      mode: 0,
      status: 0,
    },
    {
      sKey: 'fmap3',
      sZatSut: 0,
      sZatMoney: 10000,
      sText: 'Право пользования лицензионным участком В на 10 лет',
      sPref: 'fm',
      part: 'fmap3',
      sImageModel: '',
      mode: 0,
      status: 0,
    },
    {
      sKey: 'fmap4',
      sZatSut: 0,
      sZatMoney: 25000,
      sText: 'Право пользования лицензионным участком Г на 10 лет',
      sPref: 'fm',
      part: 'fmap4',
      sImageModel: '',
      mode: 0,
      status: 0,
    },
  ],

  dictSurfObjects: [
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
  ],

  DictWellIssls: [
    {
      sgrKey: 'gisParam1',
      sgrLbl: 'GR',
      sgrText: 'Гамма-каротаж',
      sgrEdinc: 'grVal',
      part: 1
    },
    {
      sgrKey: 'gisParam2',
      sgrLbl: 'PS',
      sgrText: 'Самопроизвольной поляризации',
      sgrEdinc: 'grPS',
      part: 1
    },
    {
      sgrKey: 'gisParam4',
      sgrLbl: 'NPHI',
      sgrText: 'Нейтронная пористость',
      sgrEdinc: 'grIK',
      part: 1
    },
    {
      sgrKey: 'gisParam5',
      sgrLbl: 'RDEP',
      sgrText: 'Удельное электрическое сопротивление',
      sgrEdinc: 'grAK',
      part: 1
    },
    {
      sgrKey: 'gisParam6',
      sgrLbl: 'GGKP',
      sgrText: 'Нейтронный гамма-каротаж',
      sgrEdinc: 'grGGK',
      part: 1
    },
    {
      sgrKey: 'gisParam7',
      sgrLbl: 'Lith',
      sgrText: 'Литология',
      sgrEdinc: 'grLit',
      part: 0
    },
    {
      sgrKey: 'gisParam8',
      sgrLbl: 'Poro_Core',
      sgrText: 'Пористость по керну',
      sgrEdinc: 'grPor',
      part: 2
    },
    {
      sgrKey: 'gisParam20',
      sgrLbl: 'PERM',
      sgrText: 'Проницаемость по керну',
      sgrEdinc: 'permKern',
      part: 2
    },
    {
      sgrKey: 'gisParam9',
      sgrLbl: 'Sw_core',
      sgrText: 'Водонасыщенность по керну',
      sgrEdinc: 'grPN',
      part: 2
    },
    {
      sgrKey: 'gisParam10',
      sgrLbl: 'photo',
      sgrText: 'Фотографии керна',
      sgrEdinc: 'grKern',
      part: 3
    },

    {
      sgrKey: 'gisParam21',
      sgrLbl: 'OPROBO',
      sgrText: 'Опробование скважины',
      sgrEdinc: 'grWellProbe',
      part: 3
    },
    {
      sgrKey: 'rigisParam1',
      sgrLbl: 'PORO_RIGIS',
      sgrText: 'Пористость по РИГИС',
      sgrEdinc: 'rigis_poro',
      part: 4
    },
    {
      sgrKey: 'rigisParam2',
      sgrLbl: 'PERM_RIGIS',
      sgrText: 'Проницаемость по РИГИС',
      sgrEdinc: 'rigis_perm',
      part: 4
    },
    {
      sgrKey: 'rigisParam3',
      sgrLbl: 'LITH_RIGIS',
      sgrText: 'Литология по РИГИС',
      sgrEdinc: 'rigis_lith',
      part: 4
    },
    {
      sgrKey: 'rigisParam4',
      sgrLbl: 'Sw_RIGIS',
      sgrText: 'Водонасыщенность по РИГИС',
      sgrEdinc: 'rigis_sw',
      part: 4
    }
  ]
}
