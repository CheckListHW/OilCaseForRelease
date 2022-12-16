@REM npx quasar build

call copy .\src\fonts\helvetiker_regular.typeface.json .\dist\spa-mat\fonts\helvetiker_regular.typeface.json /a
call docker build -t kos4v/oilcasexclient .
call docker push kos4v/oilcasexclient
