"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" /t:restore
"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" /p:Configuration=Release Financier.sln

rmdir /s /q D:\Development\Financier\Deploy\CLI
xcopy /E /I /Y Financier.CLI\bin\Release\netcoreapp2.0 D:\Development\Financier\Deploy\CLI

rmdir /s /q D:\Development\Financier\Deploy\Desktop
xcopy /E /I /Y Financier.Desktop\bin\Release D:\Development\Financier\Deploy\Desktop