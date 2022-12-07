const signalR = require("@aspnet/signalr");
const SecureLS = require("secure-ls");
const {Notify} = require("quasar");
const EventBus = require("../event-bus");
import {UserData} from "./DataForm";


export class MouseHub {
  constructor(link) {
    link += "MouseHub/"
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(link)
      .configureLogging(signalR.LogLevel.Error)
      .build()

    this.connection.start()
      .then(() => {
        this.connection.invoke('SendUserEnter', UserData.team, UserData.name)
          .catch(function (err) {
            console.error(err)
          })
      })


    this.connection.on("SendTeamCoordxy", function (x, y, user) {
      if (this.bCoordSend) {
        this.coord = {
          x: x,
          y: y,
          user: user
        }
        this.pointerPosition.usname = user;
        this.pointerPosition.left = this.coord.x + "px";
        if (this.dScroll !== null) {
          this.pointerPosition.top = this.coord.y + this.dScroll + "px";
        } else {
          this.pointerPosition.top = this.coord.y + "px";
        }
      }
    });

    this.connection.on("SendTeamCoordScroll", function (scry, _) {
      if (this.bCoordSend) {
        this.dScroll = scry;
        document.documentElement.scrollTop = scry;
        this.pointerPosition.scry = scry + "px";
      }
    });

    this.connection.on("SendMessage", function (msg, user) {
      return console.log(msg)
    });

    this.connection.on("SendTeamMessage", function (msg, _) {
      if ((new SecureLS()).get('ocData').userrole !== 'CAPTAIN') {
        Notify.create({type: "info", message: msg});
        EventBus.$emit('capitanEndStep', 'makestep')
      }
    });
  }

  doSendSignalMessage(group, message, userid) {
    this.connection.invoke('SendTeamMessage', userid, group, message)
      .catch(function (err) {
        return console.error(err.toString())
      })
  }

  sendScroll(scroll, username, group) {
    this.connection.invoke('SendTeamCoordScroll', scroll, username, group)
      .catch(function (err) {
        return console.error(err.toString())
      })
  }

  sendCoord(x, y, username, group) {
    this.connection.invoke('SendTeamCoordxy', x, y, username, group).catch(function (err) {
      return console.error(err.toString())
    })
  }

}
