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
				bat 'dotnet build Financier.CLI'
				
            }
        }
        stage('Test') {
            steps {
                bat 'dotnet test Financier.Core.Tests'
            }
        }
        stage('Deploy') {
            steps {
                echo 'Deploying....'
            }
        }
    }
}