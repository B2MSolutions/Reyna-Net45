@echo off
IF [%1]==[] (
	echo Missing build number
	set ERRORLEVEL=1

) ELSE (
	echo Using build number [%1]

	C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe Build\build.proj /p:BuildNumber="%1" /m /nr:false 
)