pipeline {
    agent any

    stages {
	    stage('Checkout') {
            steps {
                checkout scm
            }
        }
        stage('Build') {
            steps {
				bat 'dotnet restore Financier.CLI'
				bat 'dotnet build --configuration Release Financier.CLI'
				
            }
        }
        stage('Test') {
            steps {
                bat 'dotnet test Financier.Core.Tests'
            }
        }
        stage('Deploy') {
            steps {
                bat 'rmdir /s /q D:\\Development\\Financier\\Deploy\\CLI'
				bat 'xcopy /E /I /Y Financier.CLI\\bin\\Release\\netcoreapp2.0 D:\\Development\\Financier\\Deploy\\CLI'
            }
        }
    }
}