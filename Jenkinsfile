pipeline {
    agent any

    environment {
        // Nom de l'image Docker (ex: monapp:latest)
        DOCKER_IMAGE = "monapp:latest"
    }

    stages {
        // �tape 1 : R�cup�ration du code
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        // �tape 2 : Build .NET
        stage('Build .NET') {
            steps {
                bat 'dotnet build --configuration Release'
            }
        }

        // �tape 3 : Publication des fichiers
        stage('Publish') {
            steps {
                bat 'dotnet publish --configuration Release --output ./publish'
            }
        }

        // �tape 4 : Build de l'image Docker
        stage('Build Docker Image') {
            steps {
                script {
                    // Construire l'image Docker
                    bat "docker build -t %DOCKER_IMAGE% ."
                }
            }
        }

        // �tape 5 : D�ploiement du conteneur Docker
        stage('Deploy Docker') {
            steps {
                script {
                    // Arr�ter et supprimer le conteneur existant (si n�cessaire)
                  //  bat 'docker stop chessCoreJenkins || echo "Aucun conteneur � arr�ter"'
                   // bat 'docker rm chessCoreJenkins || echo "Aucun conteneur � supprimer"'
                    
                    // D�marrer un nouveau conteneur
                    bat "docker run -d -p 8282:82 --name chessCoreJenkins %DOCKER_IMAGE%"
                }
            }
        }
    }
}