pipeline {
    agent any

    environment {
        // Nom de l'image Docker (ex: monapp:latest)
        DOCKER_IMAGE = "monapp:latest"
    }

    stages {
        // Étape 1 : Récupération du code
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        // Étape 2 : Build .NET
        stage('Build .NET') {
            steps {
                bat 'dotnet build --configuration Release'
            }
        }

        // Étape 3 : Publication des fichiers
        stage('Publish') {
            steps {
                bat 'dotnet publish --configuration Release --output ./publish'
            }
        }

        // Étape 4 : Build de l'image Docker
        stage('Build Docker Image') {
            steps {
                script {
                    // Construire l'image Docker
                    bat "docker build -t %DOCKER_IMAGE% ."
                }
            }
        }

        // Étape 5 : Déploiement du conteneur Docker
        stage('Deploy Docker') {
            steps {
                script {
                    // Arrêter et supprimer le conteneur existant (si nécessaire)
                  //  bat 'docker stop chessCoreJenkins || echo "Aucun conteneur à arrêter"'
                   // bat 'docker rm chessCoreJenkins || echo "Aucun conteneur à supprimer"'
                    
                    // Démarrer un nouveau conteneur
                    bat "docker run -d -p 8282:82 --name chessCoreJenkins %DOCKER_IMAGE%"
                }
            }
        }
    }
}