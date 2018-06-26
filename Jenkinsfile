pipeline {
    agent any

    stages {
        stage('Build') {
            steps {
                bat 'dotnet restore Financier.CLI'
                bat 'dotnet build --configuration Release Financier.CLI'
                bat '\"C:\\Program Files (x86)\\NuGet\\nuget.exe\" restore Financier.sln'
                bat '\"C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Community\\MSBuild\\15.0\\Bin\\msbuild.exe\" Financier.Desktop\\Financier.Desktop.csproj /p:Configuration=Release'
                bat '\"C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Community\\MSBuild\\15.0\\Bin\\msbuild.exe\" Financier.Desktop.Tests\\Financier.Desktop.Tests.csproj /p:Configuration=Release'
            }
        }
        stage('Test') {
            steps {
                bat 'dotnet test Financier.Core.Tests'
                bat '\"C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Community\\Common7\\IDE\\CommonExtensions\\Microsoft\\TestWindow\\vstest.console.exe" "Financier.Desktop.Tests\\bin\\Release\\Financier.Desktop.Tests.dll"'
            }
        }
        stage('Deploy') {
            steps {
                bat 'if exist D:\\Development\\Financier\\Deploy\\CLI rmdir /s /q D:\\Development\\Financier\\Deploy\\CLI'
                bat 'xcopy /E /I /Y Financier.CLI\\bin\\Release\\netcoreapp2.0 D:\\Development\\Financier\\Deploy\\CLI'
                bat 'if exist D:\\Development\\Financier\\Deploy\\Desktop rmdir /s /q D:\\Development\\Financier\\Deploy\\Desktop'
                bat 'xcopy /E /I /Y Financier.Desktop\\bin\\Release D:\\Development\\Financier\\Deploy\\Desktop'
            }
        }
    }
}