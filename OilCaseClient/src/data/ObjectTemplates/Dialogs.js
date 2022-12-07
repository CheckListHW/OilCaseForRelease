import {QSpinnerGrid} from "quasar";

export class Dialogs {
  static AddCrossSectionVertical(days, x) {
    return {
      title: 'Добавить профиль?',
      message: `Добавить профиль i${x} Север-Юг? Интерпретация по глубине стоимость 1000 OilCoin, ${days} сут`,
      ok: 'Добавить',
      cancel: 'Отменить',
    }
  }

  static AddCrossSectionHorizontal(days, y) {
    return {
      title: 'Добавить профиль?',
      message: `Добавить профиль j${y} Запад-Восток? Интерпретация по глубине стоимость 1000 OilCoin, ${days} сут`,
      ok: 'Добавить',
      cancel: 'Отменить',
    }
  }

  static AddObjectsOfArrangement(cellX, cellY, subCellX, subCellY, objects) {
    return {
      title: 'Добавление объектов обустройства',
      message: `Участок I ${cellX} J ${cellY} ячейка i ${subCellX} j ${subCellY}?`,
      ok: 'Добавить',
      cancel: 'Отменить',
      options: {
        type: 'radio',
        model: 'soKust',
        items: subCellX !== 2 || subCellY !== 2
          ? objects.filter(x => x.part !== 3)
          : objects
      }
    }
  }

  static AgreeToViewObjectsOfArrangement(x, y) {
    return {
      title: 'Подтверждение',
      message: `Просмотр объектов обустройства на участке i ${x} j ${y}?`,
      ok: 'Открыть',
      cancel: 'Отмена'
    }
  }

  static WarningAboutAddingWell() {
    return {
      title: 'Внимание!', message: 'Скважины добавляются на кустовую площадку (Обустройсво->Кустовая площадка)'
    }
  }

  static AgreeDeleteSeismic(name) {
    return {
      title: 'Подтверждение',
      message: `Удалить профиль ${name} ?`,
      ok: 'Удалить',
      cancel: 'Отменить'
    }
  }

  static AgreeDeleteBorehole(name) {
    return {
      title: 'Подтверждение',
      message: `Удалить скважину ${name} ?`,
      ok: 'Удалить',
      cancel: 'Отменить'
    }
  }

  static AgreeDelete3DSeismic(name) {
    return {
      title: 'Подтверждение',
      message: `Удалить 3D сейсмику ${name} ?`,
      ok: 'Удалить',
      cancel: 'Отменить'
    }
  }

  static AgreeDeleteObjectOfArrangement(name) {
    return {
      title: 'Подтверждение',
      message: `Удалить объект обустройства ${name} ?`,
      ok: 'Удалить',
      cancel: 'Отменить'
    }
  }

  static ResearchNotCanceled() {
    return {
      title: 'Предупреждение',
      message: 'Заказанные ранее исследования не отменяются!',
      ok: 'Ок'
    }
  }

  static ResearchExisted() {
    return {
      title: 'Предупреждение',
      message: 'Уже есть заказанные ранее исследования!',
      ok: 'Ок'
    }
  }

  static BuyMap(mapObjects) {
    return {
      title: 'Карты и лицензионные участки',
      message: 'Выберите карту ',
      ok: 'Добавить',
      cancel: 'Отменить',
      options: {
        type: 'checkbox',
        model: [],
        items: mapObjects
      }
    }
  }

  static SaveChanges() {
    return {
      title: 'Подтверждение',
      message: 'Сохранить изменения?',
      ok: 'Сохранить',
      cancel: 'Нет'
    }
  }

  static SpecifyDateOfStatus() {
    return {
      title: 'Внимание',
      message: 'Необходимо указать дату статуса'
    }
  }

  static IncorrectParametersHorizontalTrunk() {
    return {
      title: 'Внимание',
      message: 'Неверные параметры окончания горизонтального ствола'
    }
  }


}

export class Notifications {
  static SpecifyResearch() {
    return {
      color: 'orange', message: 'Необходимо указать исследования', timeout: 1000
    }
  }

  static SpecifyObjectObjectOfArrangement() {
    return {
      color: 'orange',
      message: 'Необходимо указать вид объекта',
      timeout: 1000
    }
  }
}

export class UserActions {
  static ActionTemplate(ActionType, ActionData) {
    return {
      ActionType: ActionType, ActionData: ActionData,
    }
  }

  static AddCrossSectionHorizontal(id, x) {
    return UserActions.ActionTemplate('Добавление 2D (Запад-Восток)',
      `Название профиля 2d ${id} позиция jCell: ${x}`)
  }

  static AddCrossSectionVertical(id, y) {
    return UserActions.ActionTemplate('Добавление 2D (Север-Юг)',
      `Название профиля 2d ${id} позиция iCell: ${y}`)
  }

  static DeleteSeismic(name) {
    return UserActions.ActionTemplate('Удаление 2D профиля', `Название ${name}`)
  }

  static DeleteBorehole(name) {
    return UserActions.ActionTemplate('Удаление скважины', `Название ${name}`)
  }

  static DeleteObjectsOfArrangement(name) {
    return UserActions.ActionTemplate('Удаление объект обустройства', `Название ${name}`)
  }

  static AddResearchBorehole(gameStep, boreholeName, researchType) {
    return this.ActionTemplate('Исследования в скважине',
      `Ход ${gameStep} cкважина ${boreholeName} исследования: ${researchType}`)
  }

  static StepFinish(gameStep) {
    return this.ActionTemplate('Завершение хода', `Ход ${gameStep}`)
  }

  static InventoryClarification(gameStep, volume) {
    return UserActions.ActionTemplate('Уточнение запасов',
      `Ход ${gameStep} значение ${volume}`)
  }

  static AddBorehole(id, x, y){
  return UserActions.ActionTemplate('Добавление скважины ',
    `Название  w ${id} позиция iCell: ${x} jCell: ${y}`)
  }

  static AddObjectsOfArrangement(objectName, cellX, cellY, subCellX, subCellY){
    return UserActions.ActionTemplate('Добавление объекта',
      `Название ${objectName} позиция x: ${cellX}  y: ${cellY}, sub map: x: ${subCellX}  y: ${subCellY}`)
  }

  static AddResearches(researches, cellX, cellY){
    return UserActions.ActionTemplate('Изменение выбранных иследований скважины.',
      `В скважине с позицией x: ${cellX}  y: ${cellY} набор исследований изменен.
      Текущий набор исследований ${researches.join(', ')}`)
  }

  static AddLevelAtBorehole(boreholeName, levelName, levelMark){
    return UserActions.ActionTemplate(
      'Добавление уровня в скважине',
      `Скважина: ${boreholeName} название уровня: ${levelName}  отметка уровня: ${levelMark}`
    )
  }
}

export class SpinnerOptions {
  static DataProcessing() {
    return {
      spinner: QSpinnerGrid,
      spinnerColor: 'blue-9',
      spinnerSize: 200,
      message: 'Выполняется обработка данных...',
      messageColor: 'warning'
    }
  }
}

