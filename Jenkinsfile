pipeline {
    agent any

    stages {
        stage('Build CLI') {
            steps {
                bat 'dotnet restore Financier.CLI'
                bat 'dotnet build --configuration Release Financier.CLI'
            }
        }
        stage('Build Desktop') {
            steps {
                bat 'dotnet restore Financier.Desktop'
                bat 'dotnet build --configuration Release Financier.Desktop'
            }
        }
        stage('Test Core') {
            steps {
                bat 'dotnet test Financier.Core.Tests'
            }
        }
        stage('Test Desktop') {
            steps {
                bat 'dotnet test Financier.Desktop.Tests'
            }
        }
        stage('Deploy CLI') {
            steps {
                bat 'if exist D:\\Development\\Financier\\Deploy\\CLI rmdir /s /q D:\\Development\\Financier\\Deploy\\CLI'
                bat 'xcopy /E /I /Y Financier.CLI\\bin\\Release\\netcoreapp3.0 D:\\Development\\Financier\\Deploy\\CLI'
            }
        }
        stage('Deploy Desktop') {
            steps {
                bat 'if exist D:\\Development\\Financier\\Deploy\\Desktop rmdir /s /q D:\\Development\\Financier\\Deploy\\Desktop'
                bat 'xcopy /E /I /Y Financier.Desktop\\bin\\Release\\netcoreapp3.0 D:\\Development\\Financier\\Deploy\\Desktop'
            }
        }
    }
}