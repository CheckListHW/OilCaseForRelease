export class UserData {
  static get name() {
    return localStorage.getItem('username')
  }

  static set name(value) {
    localStorage.setItem('username', value);
  }

  static get role() {
    return localStorage.getItem('userRole')
  }

  static set role(value) {
    console.log('static set-'+value+'- role ')
    localStorage.setItem('userRole', value);
  }

  static get team() {
    return localStorage.getItem('team')
  }

  static set team(value) {
    localStorage.setItem('team', value);
  }

  static clear(){
    localStorage.clear()
  }
}



