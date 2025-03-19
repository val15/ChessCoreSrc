pipeline {
    agent any // Exécute le pipeline sur n'importe quel agent Jenkins disponible

    stages {
        // Étape 1 : Récupération du code depuis GitHub
        stage('Checkout') {
            steps {
                checkout scm // Clone le dépôt Git défini dans la configuration Jenkins
            }
        }

        // Étape 2 : Restauration des dépendances .NET
        stage('Restore Dependencies') {
            steps {
                bat 'dotnet restore' // Sur Windows, utilisez "bat". Sur Linux, utilisez "sh".
            }
        }

        // Étape 3 : Compilation du projet
        stage('Build') {
            steps {
                bat 'dotnet build --configuration Release'
            }
        }

        // Étape 4 : Exécution des tests TODO A DECOMMENTER
      //  stage('Test') {
      //      steps {
      //          bat 'dotnet test'
      //      }
     //   }

        // Étape 5 : Publication des artefacts
        stage('Publish') {
            steps {
                bat 'dotnet publish --configuration Release --output ./publish'
            }
        }

        // Étape 6 : Déploiement (à personnaliser selon votre besoin)
        stage('Deploy') {
            steps {
                // Exemple : Copie des fichiers sur un serveur distant via SSH/SCP
                bat 'scp -r ./publish C:\ChessCoreProd'
                
                // Ou déploiement sur Azure/AWS avec des commandes CLI
                // bat 'az webapp deploy ...'
            }
        }
    }
}