pipeline {
    agent any // Ex�cute le pipeline sur n'importe quel agent Jenkins disponible

    stages {
        // �tape 1 : R�cup�ration du code depuis GitHub
        stage('Checkout') {
            steps {
                checkout scm // Clone le d�p�t Git d�fini dans la configuration Jenkins
            }
        }

        // �tape 2 : Restauration des d�pendances .NET
        stage('Restore Dependencies') {
            steps {
                bat 'dotnet restore' // Sur Windows, utilisez "bat". Sur Linux, utilisez "sh".
            }
        }

        // �tape 3 : Compilation du projet
        stage('Build') {
            steps {
                bat 'dotnet build --configuration Release'
            }
        }

        // �tape 4 : Ex�cution des tests TODO A DECOMMENTER
      //  stage('Test') {
      //      steps {
      //          bat 'dotnet test'
      //      }
     //   }

        // �tape 5 : Publication des artefacts
        stage('Publish') {
            steps {
                bat 'dotnet publish --configuration Release --output ./publish'
            }
        }

        // �tape 6 : D�ploiement (� personnaliser selon votre besoin)
        stage('Deploy') {
            steps {
                // Exemple : Copie des fichiers sur un serveur distant via SSH/SCP
                bat 'scp -r ./publish C:\ChessCoreProd'
                
                // Ou d�ploiement sur Azure/AWS avec des commandes CLI
                // bat 'az webapp deploy ...'
            }
        }
    }
}