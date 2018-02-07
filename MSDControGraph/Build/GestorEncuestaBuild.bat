@echo off
cls

:Menu
color 74
color 30 
echo ================================== 
echo = 
echo = 	   HOLA SANTINO..... 
echo =  
echo ================================== 
echo. 
echo. 
echo Fecha actual:                 %DATE% 
echo Hora actual:                  %TIME%    
echo Nombre del equipo:            %COMPUTERNAME% 
echo Nombre del usuario:           %USERNAME% 
echo.
Title Menu Gestor de Encuesta 
		echo SELECCIONE OPCION
		echo.
		echo 1. COPIAR A ProEvoServer
		echo 2. COPIAR A ProEvoServer
		echo 3. COPIAR A ProEvoServer
		echo 4. SALIR
		echo.
		echo -*/-*/-*/-*/-*/-*/-*/-*/-*/-*/-*/
		echo.

	set /p var=
	if %var%==1 goto :Primero
	if %var%==2 goto :Segundo
	if %var%==3 goto :Tercero
	if %var%==4 goto exit
	if %var% GTR 4 echo Error
	goto :Menu

	:Primero
	cls 
	color a
	Echo OPCION DE COPIADO A WWW-AppServer
	Echo.
		ROBOCOPY C:\Users\sromero.NWSN\Tools\ProyectosPlanta3\trunk\ControlCorrectivo \\10.30.51.51\var\www\html\sromero\Control /COPY:DAT /MIR /FFT /Z /XA:SH /R:0 /TEE /XJD
	Echo VOLVER AL MENU ?
	Pause>Nul
	cls
	goto :Menu
	
	:Segundo
	cls 
	color a
	Echo OPCION DE COPIADO A WWW.LINUX.PROEVOSERVER
	ECHO.
	
	SETLOCAL	
	SET _fuente= \\10.30.51.51\sromero\var\www\html\sromero\Control
	SET _destino= C:\Users\sromero.NWSN\Tools\ProyectosPlanta3\trunk\ControlCorrectivo
	SET _que = /COPYALL /B /SEC /MIR
	SET _opciones = R:0 /W:0 /LOG:log.txt /NFL /NDL
	
	NET USE \\10.30.51.51\ipc$ /U:sromero ushu2014
	ROBOCOPY %_FUENTE% %_DESTINO% %_QUE% %_OPCIONES%
	Echo VOLVER AL MENU ?
	Pause>Nul
	cls
	goto :Menu
